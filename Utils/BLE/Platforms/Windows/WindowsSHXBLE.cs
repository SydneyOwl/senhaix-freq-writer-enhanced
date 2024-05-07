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
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Serial;

namespace shx.Utils.BLE.Platforms.Windows;

public enum MsgType
{
    NotifyTxt,
    BleDevice,
    BleSendData,
    BleRecData
}

public class WindowsShxble : IBluetooth
{
    public Updater StatusUpdate;

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
    public delegate void DeviceWatcherChangedEvent(BluetoothLEDevice bluetoothLeDevice);

    /// <summary>
    ///     接受数据委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void RecDataEvent(GattCharacteristic sender, byte[] data);

    /// <summary>
    ///     特性通知类型通知启用
    /// </summary>
    private const GattClientCharacteristicConfigurationDescriptorValue CharacteristicNotificationType =
        GattClientCharacteristicConfigurationDescriptorValue.Notify;

    private bool _asyncLock;

    private int _connStep = BleConst.StatusReady;

    public List<GattCharacteristic> Characteristics = new();

    private BluetoothLEAdvertisementWatcher _watcher;

    public WindowsShxble()
    {
        // Dispose();
        DeviceList = new List<BluetoothLEDevice>();
        DeviceMacList = new List<string>();
    }


    /// <summary>
    ///     当前连接的服务
    /// </summary>
    public GattDeviceService CurrentService { get; private set; }

    public BluetoothLEDevice CurrentShxDevice;

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
    private string CurrentDeviceMac { get; set; }

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
        _watcher = new BluetoothLEAdvertisementWatcher();

        _watcher.ScanningMode = BluetoothLEScanningMode.Active;

        // only activate the watcher when we're recieving values >= -80
        _watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

        // stop watching if the value drops below -90 (user walked away)
        _watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

        // register callback for when we see an advertisements
        _watcher.Received += OnAdvertisementReceived;

        // wait 5 seconds to make sure the device is really out of range
        _watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
        _watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

        // starting watching for advertisements
        _watcher.Start();

        // Console.WriteLine("自动发现设备中..");
    }

    /// <summary>
    ///     停止搜索蓝牙
    /// </summary>
    public void StopBleDeviceWatcher()
    {
        if (_watcher != null)
            _watcher.Stop();
    }

    /// <summary>
    ///     主动断开连接
    /// </summary>
    /// <returns></returns>
    public void Dispose()
    {
        CurrentDeviceMac = null;
        CurrentService?.Dispose();
        CurrentDevice?.Dispose();
        CurrentDevice = null;
        CurrentService = null;
        CurrentWriteCharacteristic = null;
        CurrentNotifyCharacteristic = null;
        DeviceMacList = new List<string>();
        DeviceList = new List<BluetoothLEDevice>();
        MySerialPort.GetInstance().WriteBle = null;
        _watcher?.Stop();
        _watcher = null;
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
    /// <param name="device"></param>
    public void StartMatching(BluetoothLEDevice device)
    {
        CurrentDevice = device;
    }

    /// <summary>
    ///     发送数据接口
    /// </summary>
    /// <returns></returns>
    public Task Write(byte[] data)
    {
        if (CurrentWriteCharacteristic != null)
            CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data),
                GattWriteOption.WriteWithoutResponse).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    // Console.WriteLine("WRITE");
                    var a = asyncInfo.GetResults();
                }
            };
        return Task.Run(() => { });
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
    public void SelectDeviceFromIdAsync(string mac)
    {
        CurrentDeviceMac = mac;
        CurrentDevice = null;
        BluetoothAdapter.GetDefaultAsync().Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var mBluetoothAdapter = asyncInfo.GetResults();
                var bytes1 = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress); //ulong转换为byte数组
                Array.Reverse(bytes1);
                var macAddress = BitConverter.ToString(bytes1, 2, 6).Replace('-', ':').ToLower();
                var id = "BluetoothLE#BluetoothLE" + macAddress + "-" + mac;
                Matching(id);
            }
        };
    }

    /// <summary>
    ///     获取操作
    /// </summary>
    /// <returns></returns>
    public void SetOpteron(GattCharacteristic gattCharacteristic)
    {
        var bytes1 = BitConverter.GetBytes(CurrentDevice.BluetoothAddress);
        Array.Reverse(bytes1);
        CurrentDeviceMac = BitConverter.ToString(bytes1, 2, 6).Replace('-', ':').ToLower();

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
    private void Matching(string id)
    {
        try
        {
            BluetoothLEDevice.FromIdAsync(id).Completed = (asyncInfo, asyncStatus) =>
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
        if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && CurrentDeviceMac != null)
        {
            StatusUpdate(false);
            if (!_asyncLock)
            {
                _asyncLock = true;
                // Console.WriteLine("设备已断开");
                Dispose();
            }
        }
        else
        {
            // Console.WriteLine("connected");
            StatusUpdate(true);
            if (!_asyncLock)
            {
                _asyncLock = true;
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
        characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CharacteristicNotificationType)
            .Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var status = asyncInfo.GetResults();
                if (status == GattCommunicationStatus.Unreachable)
                {
                    // Console.WriteLine("设备不可用");
                    if (CurrentNotifyCharacteristic != null && !_asyncLock)
                        EnableNotifications(CurrentNotifyCharacteristic);

                    return;
                }

                _asyncLock = false;
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

    public string CalMac(ulong or)
    {
        var bytes1 = BitConverter.GetBytes(or);
        Array.Reverse(bytes1);
        var address = BitConverter.ToString(bytes1, 2, 6).Replace('-', ':').ToLower();
        return address;
    }

    // actions..
    public void TriggerDeviceWatcherChanged(BluetoothLEDevice currentDevice)
    {
        var address = CalMac(currentDevice.BluetoothAddress);
        if (DeviceMacList.Contains(address)) return;

        DeviceMacList.Add(address);
    }

    public void TriggerCharacteristicFinish(int size)
    {
        if (size <= 0)
        {
            _connStep = BleConst.StatusConnFailed;
            Dispose();
            return;
        }

        _connStep = BleConst.StatusConnSuccess;
    }

    public void TriggerRecdata8800(GattCharacteristic sender, byte[] data)
    {
        foreach (var b in data)
        {
            var tmp = b;
            MySerialPort.GetInstance().RxData.Enqueue(tmp);
        }
    }

    public void TriggerRecdataGt12(GattCharacteristic sender, byte[] data)
    {
        HidTools.GetInstance().RxBuffer = data;
        HidTools.GetInstance().FlagReceiveData = true;
        // Console.WriteLine("READ1!");
    }

    public void TriggerCharacteristicAdded(GattCharacteristic gatt)
    {
        // Console.WriteLine(
        //     "handle:[0x{0}]  char properties:[{1}]  UUID:[{2}]",
        //     gatt.AttributeHandle.ToString("X4"),
        //     gatt.CharacteristicProperties.ToString(),
        //     gatt.Uuid);
        Characteristics.Add(gatt);
    }

    private bool IsBtSupported()
    {
        foreach (var allNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            if (allNetworkInterface.Description.Contains("Bluetooth") ||
                allNetworkInterface.Description.Contains("bluetooth") ||
                allNetworkInterface.Description.Contains("蓝牙")) //)
                return true;

        return false;
    }

    public void ConnectDevice(BluetoothLEDevice device)
    {
        Characteristics.Clear();
        StopBleDeviceWatcher();
        StartMatching(device);
        FindService();
    }

    public async Task<bool> GetBleAvailabilityAsync()
    {
        return IsBtSupported();
    }

    public async Task<bool> ScanForShxAsync()
    {
        DeviceWatcherChanged += TriggerDeviceWatcherChanged;
        CharacteristicAdded += TriggerCharacteristicAdded;
        CharacteristicFinish += TriggerCharacteristicFinish;
        StartBleDeviceWatcher();
        await Task.Delay(7000);
        StopBleDeviceWatcher();
        for (var i = 0; i < DeviceList.Count; i++)
            if (DeviceList[i].Name.Equals(BleConst.BtnameShx8800))
            {
                CurrentShxDevice = DeviceList[i];
                return true;
            }

        return false;
    }

    public Task ConnectShxDeviceAsync()
    {
        return Task.Run(() => { });
    }

    public async Task<bool> ConnectShxRwServiceAsync()
    {
        return true;
    }

    public async Task<bool> ConnectShxRwCharacteristicAsync()
    {
        ConnectDevice(CurrentShxDevice);
        var timeCount = 0;
        // 连接中... 成功！ 失败！
        while (_connStep == BleConst.StatusReady && timeCount++ < 200) Thread.Sleep(50);
        if (_connStep != BleConst.StatusConnSuccess) return false;
        var gattCharacteristic =
            Characteristics.Find(x => x.Uuid.ToString().Contains(BleConst.RwCharacteristicUuid));
        if (gattCharacteristic == null) return false;
        SetOpteron(gattCharacteristic);
        return true;
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = Write;
        Recdate += TriggerRecdata8800;
    }

    public void RegisterHid()
    {
        HidTools.GetInstance().WriteBle = Write;
        Recdate += TriggerRecdataGt12;
        StatusUpdate(true);
    }

    public void SetStatusUpdater(Updater up)
    {
        StatusUpdate = up;
    }
}
#endif