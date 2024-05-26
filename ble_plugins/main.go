package main

import (
	"context"
	"encoding/base64"
	"encoding/json"
	"github.com/gin-gonic/gin"
	"github.com/gookit/slog"
	"io"
	"net/http"
	"strconv"
	"strings"
	"time"
	"tinygo.org/x/bluetooth"
)

const (
	RwServiceUuid        = "ffe0"
	RwCharacteristicUuid = "ffe1"
	BtnameShx8800        = "walkie-talkie"
)

var (
	rpcAddress = "127.0.0.1"
	rpcPort    = 8563

	adapterEnabled    = false
	devList           = make([]bluetooth.ScanResult, 0)
	adapter           = bluetooth.DefaultAdapter
	targetDevice      bluetooth.ScanResult
	bleConnection     *bluetooth.Device
	bleService        bluetooth.DeviceService
	bleCharacteristic bluetooth.DeviceCharacteristic

	bleRecvChan = make(chan []byte, 10)
	bleSendChan = make(chan []byte, 10)

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
	ctx, cancelFunc = context.WithCancel(context.Background())
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
				slog.Infof("写Routine启动")
				for {
					select {
					case <-cx.Done():
						slog.Infof("写Routine退出")
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
}

func test() {

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

func main() {
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
		default:
			c.JSON(http.StatusOK, gin.H{
				"response": "",
				"error":    "无效的调用",
			})
		}
	})
	slog.Fatal(r.Run("127.0.0.1:8563"))
}