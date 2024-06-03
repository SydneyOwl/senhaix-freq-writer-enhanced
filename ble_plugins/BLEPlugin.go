package main

import (
	"BLEPlugin/logger"
	"context"
	"encoding/json"
	"fmt"
	"github.com/gookit/slog"
	"github.com/gorilla/websocket"
	"github.com/spf13/cobra"
	"os"
	"strconv"
	"strings"
	"time"
	"tinygo.org/x/bluetooth"
)

const (
	RwServiceUuid        = "ffe0"
	RwCharacteristicUuid = "ffe1"
)

var (
	rpcAddress = "127.0.0.1"
	rpcPort    = 8563
	verbose    = false
	vverbose   = false

	adapterEnabled    = false
	devList           = make([]bluetooth.ScanResult, 0)
	adapter           = bluetooth.DefaultAdapter
	targetDevice      bluetooth.ScanResult
	bleConnection     *bluetooth.Device
	bleService        bluetooth.DeviceService
	bleCharacteristic bluetooth.DeviceCharacteristic

	bleRecvChan = make(chan []byte, 10)
	bleSendChan = make(chan []byte, 10)
	doneChan    = make(chan struct{})

	ctx        context.Context
	cancelFunc context.CancelFunc
)

type BLEDevice struct {
	DeviceName    string `json:"DeviceName"`
	DeviceMacAddr string `json:"DeviceMacAddr"`
	DeviceID      string `json:"DeviceID"`
}

type RequestJson struct {
	Method string `json:"method"`
	Arg    string `json:"arg"`
}
type ResponseJSON struct {
	Response string `json:"response"`
	Error    string `json:"error"`
}

func GetBleAvailability() (bool, error) {
	DisposeBluetooth()
	if !adapterEnabled {
		if err := adapter.Enable(); err != nil {
			slog.Warnf("出错: %s\n", err.Error())
			return false, err
		}
		adapterEnabled = true
	}
	for len(bleSendChan) > 0 {
		<-bleSendChan
	}
	for len(bleRecvChan) > 0 {
		<-bleRecvChan
	}
	return true, nil
}

// ScanForShx Returns a json list actually..
func ScanForShx() (string, error) {
	errChan := make(chan error)
	removeRepeat := make(map[string]bool)
	devList = make([]bluetooth.ScanResult, 0)
	go func(errChan chan error, devList *[]bluetooth.ScanResult) {
		err := adapter.Scan(func(adapter *bluetooth.Adapter, device bluetooth.ScanResult) {
			slog.Debugf("找到设备: %s, %d, %s", device.Address.String(), device.RSSI, device.LocalName())
			if !removeRepeat[device.Address.String()] {
				slog.Noticef("设备: %s, %d, %s", device.Address.String(), device.RSSI, device.LocalName())
				removeRepeat[device.Address.String()] = true
				*devList = append(*devList, device)
			}
		})
		if err != nil {
			errChan <- err
		}
	}(errChan, &devList)

	select {
	case err := <-errChan:
		slog.Warnf("扫描错误：%s", err.Error())
		return "[]", err
	case <-time.After(5 * time.Second):
		_ = adapter.StopScan()
		bleDevice := make([]BLEDevice, 0)
		for i, v := range devList {
			bleDevice = append(bleDevice, BLEDevice{
				DeviceName:    v.LocalName(),
				DeviceMacAddr: v.Address.String(),
				DeviceID:      strconv.Itoa(i),
			})
		}
		jsonData, _ := json.Marshal(bleDevice)
		return string(jsonData), nil
	}
}

func SetDevice(seq int) {
	targetDevice = devList[seq]
}
func ConnectShxDevice() (bool, error) {
	conn, err := adapter.Connect(targetDevice.Address, bluetooth.ConnectionParams{
		ConnectionTimeout: bluetooth.NewDuration(5 * time.Second),
	})
	if err != nil {
		slog.Warnf("连接失败：%s", err)
		return false, err
	}
	slog.Infof("连接成功！")
	bleConnection = conn
	return true, nil
}
func ConnectShxRwService() (bool, error) {
	services, err := bleConnection.DiscoverServices(nil)
	if err != nil {
		slog.Errorf("无法发现服务:%v", err)
		return false, err
	}
	for _, v := range services {
		if strings.Contains(v.String(), RwServiceUuid) {
			slog.Infof("已找到服务！")
			bleService = v
			return true, nil
		}
	}
	slog.Errorf("未找到服务！")
	return false, err
}
func ConnectShxRwCharacteristic() (bool, error) {
	chs, err := bleService.DiscoverCharacteristics(nil)
	if err != nil {
		slog.Errorf("无法发现特征:%v", err)
		return false, err
	}
	for _, v := range chs {
		if strings.Contains(v.String(), RwCharacteristicUuid) {
			bleCharacteristic = v
			slog.Infof("已发现特征！")
			_ = bleCharacteristic.EnableNotifications(NotifyCallback)
			return true, nil
		}
	}
	slog.Errorf("未发现特征！")
	return false, nil
}

func NotifyCallback(data []byte) {
	bleRecvChan <- data
}

func WriteData(data []byte) {
	bleSendChan <- data
}
func DisposeBluetooth() {
	if cancelFunc != nil {
		cancelFunc()
	}
	if bleConnection != nil {
		_ = bleConnection.Disconnect()
	}
	//ctx, cancelFunc = context.WithCancel(context.Background())
}
func TerminatePlugin() {
	slog.Info("收到终止命令")
	os.Exit(0)
}

func HandlerReturnBoolValue(value bool, err error, c *websocket.Conn) {
	errStr := ""
	if err != nil {
		errStr = err.Error()
	}
	resp := "False"
	if value {
		resp = "True"
	}
	err = c.WriteJSON(ResponseJSON{
		Response: resp,
		Error:    errStr,
	})
	if err != nil {
		slog.Errorf("出错：%v", err)
		close(doneChan)
	}
}
func HandlerReturnStringValue(value string, err error, c *websocket.Conn) {
	errStr := ""
	if err != nil {
		errStr = err.Error()
	}
	err = c.WriteJSON(ResponseJSON{
		Response: value,
		Error:    errStr,
	})
	if err != nil {
		slog.Errorf("出错：%v", err)
		close(doneChan)
	}
}

func HandlerReturnBinaryValue(value []byte, c *websocket.Conn) {
	err := c.WriteMessage(websocket.BinaryMessage, value)
	if err != nil {
		slog.Errorf("出错：%v", err)
		close(doneChan)
	}
}

func StartWriteBack(ctx context.Context, conn *websocket.Conn) {
	go func(cx context.Context, c *websocket.Conn) {
		slog.Trace("回写二进制响应启动！")
		defer slog.Trace("回写二进制响应退出！")

		for {
			select {
			case data := <-bleRecvChan:
				HandlerReturnBinaryValue(data, c)
			case <-cx.Done():
				return
			}
		}
	}(ctx, conn)
}

func StartBleWrite(ctx context.Context, bleSendChan chan []byte) {
	go func(cx context.Context, bleChan chan []byte) {
		slog.Tracef("写BLE Routine启动")
		for {
			select {
			case <-cx.Done():
				slog.Tracef("写BLE Routine退出")
				return
			case sFrame := <-bleChan:
				if len(sFrame) == 0 {
					continue
				}
				_, _ = bleCharacteristic.WriteWithoutResponse(sFrame)
			}
		}
	}(ctx, bleSendChan)
}

// StartRPC 使用JSONRPC规范
func StartRPC(addr string) {
	c, _, err := websocket.DefaultDialer.Dial(addr, nil)
	if err != nil {
		slog.Fatalf("无法连接到服务器：%v", err)
		return
	}
	defer func(c *websocket.Conn) {
		err := c.Close()
		if err != nil {
			// just ignore
		}
	}(c)
	slog.Info("RPC服务启动！")

	go func() {
		slog.Trace("RPC main 启动！")
		defer close(doneChan)
		for {
			mt, data, err := c.ReadMessage()
			if err != nil {
				slog.Errorf("读取错误：%v", err)
				return
			}
			// 需写入的数据
			if mt == websocket.BinaryMessage {
				WriteData(data)
				HandlerReturnBoolValue(true, nil, c)
				continue
			}
			var requestData = RequestJson{}
			if err := json.Unmarshal(data, &requestData); err != nil {
				slog.Errorf("出错：%s", err.Error())
				return
			}
			slog.Debug("Calling..." + requestData.Method)
			switch requestData.Method {
			case "GetBleAvailability":
				result, err := GetBleAvailability()
				HandlerReturnBoolValue(result, err, c)
			case "ScanForShx":
				result, err := ScanForShx()
				HandlerReturnStringValue(result, err, c)
			case "SetDevice":
				seq, _ := strconv.Atoi(requestData.Arg)
				SetDevice(seq)
				HandlerReturnStringValue("", nil, c)
			case "ConnectShxDevice":
				result, err := ConnectShxDevice()
				HandlerReturnBoolValue(result, err, c)
			case "ConnectShxRwService":
				result, err := ConnectShxRwService()
				HandlerReturnBoolValue(result, err, c)
			case "ConnectShxRwCharacteristic":
				ctx, cancelFunc = context.WithCancel(context.Background())
				result, err := ConnectShxRwCharacteristic()
				StartWriteBack(ctx, c)
				StartBleWrite(ctx, bleSendChan)
				HandlerReturnBoolValue(result, err, c)
			case "DisposeBluetooth":
				DisposeBluetooth()
				HandlerReturnStringValue("", nil, c)
			case "TerminatePlugin":
				HandlerReturnStringValue("", nil, c)
				TerminatePlugin()
			default:
				HandlerReturnStringValue("调用无效", nil, c)
			}
		}
	}()
	<-doneChan
	slog.Info("RPC Client退出")
}
func main() {
	var noColorOutput = false
	var BaseCmd = &cobra.Command{
		Use:   "BLE RPC Server",
		Short: "BLE RPC",
		Long:  `BLE RPC Server - Connect shx8x00 and c#`,
		Run: func(cmd *cobra.Command, args []string) {
			logger.InitLog(verbose, vverbose, noColorOutput)
			StartRPC(fmt.Sprintf("ws://%s:%d/rpc", rpcAddress, rpcPort))
		},
	}
	BaseCmd.PersistentFlags().IntVar(&rpcPort, "port", 8563, "RPC Server listening port")
	BaseCmd.PersistentFlags().BoolVar(&noColorOutput, "no-color", false, "No color output in console")
	BaseCmd.PersistentFlags().StringVar(&rpcAddress, "address", "127.0.0.1", "RPC Server listening address")
	BaseCmd.PersistentFlags().BoolVar(&verbose, "verbose", false, "Print Debug Level logs")
	BaseCmd.PersistentFlags().BoolVar(&vverbose, "vverbose", false, "Print Debug/Trace Level logs")
	cobra.MousetrapHelpText = ""
	if err := BaseCmd.Execute(); err != nil {
		fmt.Printf("程序无法启动: %v", err)
		os.Exit(-1)
	}
}
