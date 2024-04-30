#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.Serial;

namespace shx.Utils.BLE.Platforms.Windows;

public enum MsgType
{
    NotifyTxt,
    BleDevice,
    BleSendData,
    BleRecData,
}

public class WindowsSHXBLE : IBluetooth
{
    /// <summary>
    ///     获取特征委托
    /// </summary>
    public delegate void CharacteristicAddedEvent(GattCharacteristic gattCharacteristic);

    /// <summary>
    ///     获取服务委托
    /// </summary>
    public delegate void CharacteristicFinishEvent(int size);

    /// <summary>
    ///     定义搜索蓝牙设备委托
    /// </summary>
    public delegate void DeviceWatcherChangedEvent(BluetoothLEDevice bluetoothLEDevice);

    /// <summary>
    ///     接受数据委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void RecDataEvent(GattCharacteristic sender, byte[] data);

    /// <summary>
    ///     特性通知类型通知启用
    /// </summary>
    private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE =
        GattClientCharacteristicConfigurationDescriptorValue.Notify;

    private bool asyncLock;

    private int connStep = BLE_CONST.STATUS_READY;

    public List<GattCharacteristic> characteristics = new();
    
    private BluetoothLEAdvertisementWatcher Watcher;

    public WindowsSHXBLE()
    {
        // Dispose();
        DeviceList = new List<BluetoothLEDevice>();
        DeviceMacList = new List<string>();
    }

    
    /// <summary>
    ///     当前连接的服务
    /// </summary>
    public GattDeviceService CurrentService { get; private set; }

    public BluetoothLEDevice CurrentSHXDevice;

    /// <summary>
    ///     当前连接的蓝牙设备
    /// </summary>
    public BluetoothLEDevice CurrentDevice { get; private set; }

    /// <summary>
    ///     写特征对象
    /// </summary>
    public GattCharacteristic CurrentWriteCharacteristic { get; private set; }

    /// <summary>
    ///     通知特征对象
    /// </summary>
    public GattCharacteristic CurrentNotifyCharacteristic { get; private set; }

    /// <summary>
    ///     存储检测到的设备
    /// </summary>
    public List<BluetoothLEDevice> DeviceList { get; private set; }

    public List<string> DeviceMacList { get; private set; }

    /// <summary>
    ///     当前连接的蓝牙Mac
    /// </summary>
    private string CurrentDeviceMAC { get; set; }

    /// <summary>
    ///     搜索蓝牙事件
    /// </summary>
    public event DeviceWatcherChangedEvent DeviceWatcherChanged;

    /// <summary>
    ///     获取服务事件
    /// </summary>
    public event CharacteristicFinishEvent CharacteristicFinish;

    /// <summary>
    ///     获取特征事件
    /// </summary>
    public event CharacteristicAddedEvent CharacteristicAdded;

    /// <summary>
    ///     接受数据事件
    /// </summary>
    public event RecDataEvent Recdate;

    /// <summary>
    ///     搜索蓝牙设备
    /// </summary>
    public void StartBleDeviceWatcher()
    {
        Watcher = new BluetoothLEAdvertisementWatcher();

        Watcher.ScanningMode = BluetoothLEScanningMode.Active;
        
        // only activate the watcher when we're recieving values >= -80
        Watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

        // stop watching if the value drops below -90 (user walked away)
        Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

        // register callback for when we see an advertisements
        Watcher.Received += OnAdvertisementReceived;

        // wait 5 seconds to make sure the device is really out of range
        Watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
        Watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

        // starting watching for advertisements
        Watcher.Start();

        // Console.WriteLine("自动发现设备中..");
    }

    /// <summary>
    ///     停止搜索蓝牙
    /// </summary>
    public void StopBleDeviceWatcher()
    {
        if (Watcher != null)
            Watcher.Stop();
    }

    /// <summary>
    ///     主动断开连接
    /// </summary>
    /// <returns></returns>
    public void Dispose()
    {
        CurrentDeviceMAC = null;
        CurrentService?.Dispose();
        CurrentDevice?.Dispose();
        CurrentDevice = null;
        CurrentService = null;
        CurrentWriteCharacteristic = null;
        CurrentNotifyCharacteristic = null;
        DeviceMacList = new List<string>();
        DeviceList = new List<BluetoothLEDevice>();
        MySerialPort.getInstance().WriteBLE = null;
        Watcher.Stop();
        Watcher = null;
        // ForceNewBleInstance();
        // Console.WriteLine("主动断开连接");
        // if (meg != null)
        // {
        //     meg.Text = "蓝牙（未连接）";
        // }
    }

    /// <summary>
    ///     匹配
    /// </summary>
    /// <param name="Device"></param>
    public void StartMatching(BluetoothLEDevice Device)
    {
        CurrentDevice = Device;
    }

    /// <summary>
    ///     发送数据接口
    /// </summary>
    /// <returns></returns>
    public Task Write(byte[] data)
    {
        if (CurrentWriteCharacteristic != null)
            CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data),
                GattWriteOption.WriteWithResponse).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    var a = asyncInfo.GetResults();
                }
            };
        return Task.Run(() => {});
    }

    /// 获取蓝牙服务
    /// </summary>
    public void FindService()
    {
        CurrentDevice.GetGattServicesAsync().Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var services = asyncInfo.GetResults().Services;
                // Console.WriteLine("GattServices size=" + services.Count);
                foreach (var ser in services) FindCharacteristic(ser);

                CharacteristicFinish?.Invoke(services.Count);
            }
        };
    }

    /// <summary>
    ///     按MAC地址直接组装设备ID查找设备
    /// </summary>
    public void SelectDeviceFromIdAsync(string MAC)
    {
        CurrentDeviceMAC = MAC;
        CurrentDevice = null;
        BluetoothAdapter.GetDefaultAsync().Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var mBluetoothAdapter = asyncInfo.GetResults();
                var _Bytes1 = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress); //ulong转换为byte数组
                Array.Reverse(_Bytes1);
                var macAddress = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();
                var Id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
                Matching(Id);
            }
        };
    }

    /// <summary>
    ///     获取操作
    /// </summary>
    /// <returns></returns>
    public void SetOpteron(GattCharacteristic gattCharacteristic)
    {
        var _Bytes1 = BitConverter.GetBytes(CurrentDevice.BluetoothAddress);
        Array.Reverse(_Bytes1);
        CurrentDeviceMAC = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();

        // Console.WriteLine(gattCharacteristic.CharacteristicProperties);

        // if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Write)
        // {
        CurrentWriteCharacteristic = gattCharacteristic;
        // }
        //
        // if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Notify)
        // {
        CurrentNotifyCharacteristic = gattCharacteristic;
        // }

        // if ((uint)gattCharacteristic.CharacteristicProperties == 26)
        // {
        // }
        //
        // if (gattCharacteristic.CharacteristicProperties ==
        //     (GattCharacteristicProperties.Write | GattCharacteristicProperties.Notify))
        // {
        //     Console.WriteLine("here!");
        //     this.CurrentWriteCharacteristic = gattCharacteristic;
        //     this.CurrentNotifyCharacteristic = gattCharacteristic;
        CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
        CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;
        CurrentDevice.ConnectionStatusChanged += CurrentDevice_ConnectionStatusChanged;
        EnableNotifications(CurrentNotifyCharacteristic);
        // }
    }

    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher,
        BluetoothLEAdvertisementReceivedEventArgs eventArgs)
    {
        BluetoothLEDevice.FromBluetoothAddressAsync(eventArgs.BluetoothAddress).Completed =
            (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    if (asyncInfo.GetResults() == null)
                    {
                        //Console.WriteLine("没有得到结果集");
                    }
                    else
                    {
                        var currentDevice = asyncInfo.GetResults();

                        if (DeviceList.FindIndex(x => { return x.Name.Equals(currentDevice.Name); }) < 0)
                        {
                            DeviceList.Add(currentDevice);
                            DeviceWatcherChanged?.Invoke(currentDevice);
                        }
                    }
                }
            };
    }

    /// <summary>
    ///     获取特性
    /// </summary>
    private void FindCharacteristic(GattDeviceService gattDeviceService)
    {
        CurrentService = gattDeviceService;
        CurrentService.GetCharacteristicsAsync().Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var services = asyncInfo.GetResults().Characteristics;
                foreach (var c in services) CharacteristicAdded?.Invoke(c);
            }
        };
    }

    /// <summary>
    ///     搜索到的蓝牙设备
    /// </summary>
    /// <returns></returns>
    private void Matching(string Id)
    {
        try
        {
            BluetoothLEDevice.FromIdAsync(Id).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    var bleDevice = asyncInfo.GetResults();
                    DeviceList.Add(bleDevice);
                    // Console.WriteLine(bleDevice);
                }

                // if (asyncStatus == AsyncStatus.Started) Console.WriteLine(asyncStatus.ToString());
                //
                // if (asyncStatus == AsyncStatus.Canceled) Console.WriteLine(asyncStatus.ToString());
                //
                // if (asyncStatus == AsyncStatus.Error) Console.WriteLine(asyncStatus.ToString());
            };
        }
        catch (Exception e)
        {
            // var msg = "没有发现设备" + e;
            // Console.WriteLine(msg);
            StartBleDeviceWatcher();
        }
    }


    private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && CurrentDeviceMAC != null)
        {
            if (!asyncLock)
            {
                asyncLock = true;
                Dispose();
            }
        }
        else
        {
            if (!asyncLock)
            {
                asyncLock = true;
                // Console.WriteLine("设备已连接");
                // meg.Text = "蓝牙（已连接）";
            }
        }
    }

    /// <summary>
    ///     设置特征对象为接收通知对象
    /// </summary>
    /// <param name="characteristic"></param>
    /// <returns></returns>
    private void EnableNotifications(GattCharacteristic characteristic)
    {
        // Console.WriteLine("收通知对象=" + CurrentDevice.Name + ":" + CurrentDevice.ConnectionStatus);
        characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE)
            .Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var status = asyncInfo.GetResults();
                if (status == GattCommunicationStatus.Unreachable)
                {
                    // Console.WriteLine("设备不可用");
                    if (CurrentNotifyCharacteristic != null && !asyncLock)
                        EnableNotifications(CurrentNotifyCharacteristic);

                    return;
                }

                asyncLock = false;
                // Console.WriteLine("设备连接状态" + status);
            }
        };
    }

    /// <summary>
    ///     接受到蓝牙数据
    /// </summary>
    private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        byte[] data;
        CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
        Recdate?.Invoke(sender, data);
    }

    public string calMac(ulong or)
    {
        var _Bytes1 = BitConverter.GetBytes(or);
        Array.Reverse(_Bytes1);
        var address = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();
        return address;
    }

    // actions..
    public void TriggerDeviceWatcherChanged(BluetoothLEDevice currentDevice)
    {
        var address = calMac(currentDevice.BluetoothAddress);
        if (DeviceMacList.Contains(address) ) return;

        DeviceMacList.Add(address);
    }

    public void TriggerCharacteristicFinish(int size)
    {
        if (size <= 0)
        {
            connStep = BLE_CONST.STATUS_CONN_FAILED;
            Dispose();
            return;
        }

        connStep = BLE_CONST.STATUS_CONN_SUCCESS;
    }

    public void TriggerRecdata(GattCharacteristic sender, byte[] data)
    {
        foreach (var b in data)
        {
            var tmp = b;
            MySerialPort.getInstance().RxData.Enqueue(tmp);
        }
    }

    public void TriggerCharacteristicAdded(GattCharacteristic gatt)
    {
        // Console.WriteLine(
        //     "handle:[0x{0}]  char properties:[{1}]  UUID:[{2}]",
        //     gatt.AttributeHandle.ToString("X4"),
        //     gatt.CharacteristicProperties.ToString(),
        //     gatt.Uuid);
        characteristics.Add(gatt);
    }

    private bool isBtSupported()
    {
        foreach (var allNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            if (allNetworkInterface.Description.Contains("Bluetooth") ||
                allNetworkInterface.Description.Contains("bluetooth") ||
                allNetworkInterface.Description.Contains("蓝牙")) //)
                return true;

        return false;
    }

    public void ConnectDevice(BluetoothLEDevice Device)
    {
        characteristics.Clear();
        StopBleDeviceWatcher();
        StartMatching(Device);
        FindService();
    }

    public async Task<bool> GetBLEAvailabilityAsync()
    {
        return isBtSupported();
    }

    public async Task<bool> ScanForSHXAsync()
    {
        DeviceWatcherChanged += TriggerDeviceWatcherChanged;
        CharacteristicAdded += TriggerCharacteristicAdded;
        CharacteristicFinish += TriggerCharacteristicFinish;
        Recdate += TriggerRecdata;
        StartBleDeviceWatcher();
        await Task.Delay(7000);
        StopBleDeviceWatcher();
        for (var i = 0; i < DeviceList.Count; i++)
        {
            if (DeviceList[i].Name.Equals(BLE_CONST.BTNAME_SHX8800))
            {
                CurrentSHXDevice = DeviceList[i];
                return true;
            }
        }

        return false;
    }

    public Task ConnectSHXDeviceAsync()
    {
        return Task.Run(() => { });
    }

    public async Task<bool> ConnectSHXRWServiceAsync()
    {
        return true;
    }

    public async Task<bool> ConnectSHXRWCharacteristicAsync()
    {
        ConnectDevice(CurrentSHXDevice);
        var timeCount = 0;
        // 连接中... 成功！ 失败！
        while (connStep == BLE_CONST.STATUS_READY && timeCount++ < 200)
        {
            Thread.Sleep(50);
        }
        if (connStep != BLE_CONST.STATUS_CONN_SUCCESS)
        {
            return false;
        }
        var gattCharacteristic =
characteristics.Find(x => x.Uuid.ToString().Contains(BLE_CONST.RW_CHARACTERISTIC_UUID));
        if (gattCharacteristic == null)
        {
            return false;
        }
        SetOpteron(gattCharacteristic);
        return true;
    }

    public void RegisterSerial()
    {
        MySerialPort.getInstance().WriteBLE = Write;
    }
}
#endif