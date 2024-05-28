package main

import (
	"BLEPlugin/logger"
	"context"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"github.com/gin-gonic/gin"
	"github.com/gookit/slog"
	"github.com/spf13/cobra"
	"io"
	"net/http"
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
	rpcAddress      = "127.0.0.1"
	rpcPort         = 8563
	verbose         = false
	vverbose        = false
	enableKeepAlive = false

	adapterEnabled    = false
	devList           = make([]bluetooth.ScanResult, 0)
	adapter           = bluetooth.DefaultAdapter
	targetDevice      bluetooth.ScanResult
	bleConnection     *bluetooth.Device
	bleService        bluetooth.DeviceService
	bleCharacteristic bluetooth.DeviceCharacteristic

	bleRecvChan = make(chan []byte, 10)
	bleSendChan = make(chan []byte, 10)
	// 10秒内没有发送心跳包自动退出
	bleKeepAliveChan = make(chan struct{}, 5)

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
			// Start Routine: write
			go func(cx context.Context, bleChan chan []byte) {
				slog.Tracef("写Routine启动")
				for {
					select {
					case <-cx.Done():
						slog.Tracef("写Routine退出")
						return
					case sFrame := <-bleChan:
						if len(sFrame) == 0 {
							continue
						}
						_, _ = bleCharacteristic.WriteWithoutResponse(sFrame)
					}
				}
			}(ctx, bleSendChan)
			return true, nil
		}
	}
	slog.Errorf("未发现特征！")
	return false, nil
}

func NotifyCallback(data []byte) {
	bleRecvChan <- data
}

func ReadCachedData() []byte {
	select {
	case data := <-bleRecvChan:
		return data
	case <-ctx.Done():
		return nil
	case <-time.After(time.Second * 5):
		return nil
	}
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
	ctx, cancelFunc = context.WithCancel(context.Background())
}

func HandlerReturnBoolValue(value bool, err error, c *gin.Context) {
	errStr := ""
	if err != nil {
		errStr = err.Error()
	}
	resp := "False"
	if value {
		resp = "True"
	}
	c.JSON(http.StatusOK, gin.H{
		"response": resp,
		"error":    errStr,
	})
}
func HandlerReturnStringValue(value string, err error, c *gin.Context) {
	errStr := ""
	if err != nil {
		errStr = err.Error()
	}
	c.JSON(http.StatusOK, gin.H{
		"response": value,
		"error":    errStr,
	})
}

func StartKeepAliveService() {
	if enableKeepAlive {
		bleKeepAliveChan <- struct{}{}
		go func(cn chan struct{}) {
			slog.Debug("keepalive goroutine started！")
			for {
				select {
				case <-cn:
					continue
				case <-time.After(time.Second * 10):
					slog.Notice("10秒内未收到心跳包，程序退出！")
					os.Exit(0)
				}
			}
		}(bleKeepAliveChan)
	}
}

// StartRPC 使用JSONRPC规范
func StartRPC(addr string) {
	slog.Info("RPC服务启动！")
	StartKeepAliveService()
	gin.SetMode(gin.ReleaseMode)
	gin.DefaultWriter = io.Discard
	if verbose || vverbose {
		gin.DefaultWriter = os.Stdout
	}
	r := gin.Default()
	r.POST("/", func(c *gin.Context) {
		bodyBytes, _ := io.ReadAll(c.Request.Body)
		var requestData = RequestJson{}
		if err := json.Unmarshal(bodyBytes, &requestData); err != nil {
			slog.Errorf("出错：%s", err.Error())
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    err.Error(),
			})
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
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "",
			})
		case "ConnectShxDevice":
			result, err := ConnectShxDevice()
			HandlerReturnBoolValue(result, err, c)
		case "ConnectShxRwService":
			result, err := ConnectShxRwService()
			HandlerReturnBoolValue(result, err, c)
		case "ConnectShxRwCharacteristic":
			result, err := ConnectShxRwCharacteristic()
			HandlerReturnBoolValue(result, err, c)
		case "ReadCachedData":
			result := ReadCachedData()
			bs64 := ""
			if result != nil {
				bs64 = base64.StdEncoding.EncodeToString(result)
			}
			HandlerReturnStringValue(bs64, nil, c)
		case "WriteData":
			var dec []byte
			if requestData.Arg != "" {
				dec, _ = base64.StdEncoding.DecodeString(requestData.Arg)
			}
			WriteData(dec)
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "",
			})
		case "DisposeBluetooth":
			DisposeBluetooth()
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "",
			})
		case "KeepAlive":
			bleKeepAliveChan <- struct{}{}
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "",
			})
		default:
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "无效的调用",
			})
		}
	})
	slog.Fatal(r.Run(addr))
}
func main() {
	ctx, cancelFunc = context.WithCancel(context.Background())
	var BaseCmd = &cobra.Command{
		Use:   "BLE RPC Server",
		Short: "BLE RPC",
		Long:  `BLE RPC Server - Connect shx8x00 and c#`,
		Run: func(cmd *cobra.Command, args []string) {
			logger.InitLog(verbose, vverbose)
			StartRPC(fmt.Sprintf("%s:%d", rpcAddress, rpcPort))
		},
	}
	BaseCmd.PersistentFlags().IntVar(&rpcPort, "port", 8563, "RPC Server listening port")
	BaseCmd.PersistentFlags().StringVar(&rpcAddress, "address", "127.0.0.1", "RPC Server listening address")
	BaseCmd.PersistentFlags().BoolVar(&verbose, "verbose", false, "Print Debug Level logs")
	BaseCmd.PersistentFlags().BoolVar(&vverbose, "vverbose", false, "Print Debug/Trace Level logs")
	BaseCmd.PersistentFlags().BoolVar(&enableKeepAlive, "enable-keepalive", false, "enable keepalive(process exit if no keepalive packet is received within 10s)")
	cobra.MousetrapHelpText = ""
	if err := BaseCmd.Execute(); err != nil {
		fmt.Printf("程序无法启动: %v", err)
		os.Exit(-1)
	}
}
