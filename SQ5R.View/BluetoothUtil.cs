#if NET461
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Security.Cryptography;
using DevComponents.DotNetBar.Controls;

namespace SQ5R.View;

public class BleCore
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

    private static BleCore instance;
    private bool asyncLock;


    public List<GattCharacteristic> characteristics = new();

    public int connStep = BTConsts.STATUS_READY;

    private DataGridViewX dgvx;

    private CheckBox disableSSIDFilter;

    private CheckBox disableWeakSignalFilter;

    public Queue<byte> rxData = new(1024);

    private BluetoothLEAdvertisementWatcher Watcher;

    private BleCore()
    {
        Dispose();
        DeviceList = new List<BluetoothLEDevice>();
        DeviceMacList = new List<string>();
    }

    /// <summary>
    ///     当前连接的服务
    /// </summary>
    public GattDeviceService CurrentService { get; private set; }

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

    public static BleCore BleInstance()
    {
        if (instance == null) instance = new BleCore();

        return instance;
    }

    /// <summary>
    ///     搜索蓝牙设备
    /// </summary>
    public void StartBleDeviceWatcher()
    {
        Watcher = new BluetoothLEAdvertisementWatcher();

        Watcher.ScanningMode = BluetoothLEScanningMode.Active;

        if (disableWeakSignalFilter.Checked)
        {
            // only activate the watcher when we're recieving values >= -80
            Watcher.SignalStrengthFilter.InRangeThresholdInDBm = -120;

            // stop watching if the value drops below -90 (user walked away)
            Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -120;
        }
        else
        {
            // only activate the watcher when we're recieving values >= -80
            Watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // stop watching if the value drops below -90 (user walked away)
            Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;
        }

        // register callback for when we see an advertisements
        Watcher.Received += OnAdvertisementReceived;

        // wait 5 seconds to make sure the device is really out of range
        Watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
        Watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

        // starting watching for advertisements
        Watcher.Start();

        Console.WriteLine("自动发现设备中..");
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
        Console.WriteLine("主动断开连接");
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
    public void Write(byte[] data)
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
                Console.WriteLine("GattServices size=" + services.Count);
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

        Console.WriteLine(gattCharacteristic.CharacteristicProperties);

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
                    Console.WriteLine(bleDevice);
                }

                if (asyncStatus == AsyncStatus.Started) Console.WriteLine(asyncStatus.ToString());

                if (asyncStatus == AsyncStatus.Canceled) Console.WriteLine(asyncStatus.ToString());

                if (asyncStatus == AsyncStatus.Error) Console.WriteLine(asyncStatus.ToString());
            };
        }
        catch (Exception e)
        {
            var msg = "没有发现设备" + e;
            Console.WriteLine(msg);
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
                MessageBox.Show("设备已断开！");
                Dispose();
            }
        }
        else
        {
            if (!asyncLock)
            {
                asyncLock = true;
                Console.WriteLine("设备已连接");
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
        Console.WriteLine("收通知对象=" + CurrentDevice.Name + ":" + CurrentDevice.ConnectionStatus);
        characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE)
            .Completed = (asyncInfo, asyncStatus) =>
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                var status = asyncInfo.GetResults();
                if (status == GattCommunicationStatus.Unreachable)
                {
                    Console.WriteLine("设备不可用");
                    if (CurrentNotifyCharacteristic != null && !asyncLock)
                        EnableNotifications(CurrentNotifyCharacteristic);

                    return;
                }

                asyncLock = false;
                Console.WriteLine("设备连接状态" + status);
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
        if (DeviceMacList.Contains(address) ||
            (!disableSSIDFilter.Checked && !currentDevice.Name.Contains(BTConsts.BTNAME_SHX8800))) return;

        DeviceMacList.Add(address);
        dgvx.Rows.Add(DeviceMacList.Count.ToString(), currentDevice.Name, "无数据", address);
    }

    public void setView(DataGridViewX dgvx)
    {
        this.dgvx = dgvx;
    }

    public void setCheckbox(CheckBox ssid, CheckBox weakSignal)
    {
        disableSSIDFilter = ssid;
        disableWeakSignalFilter = weakSignal;
    }

    // static public void Main(String[] args)
    // {
    //     var bleCore = new BleCore();
    //     bleCore.DeviceWatcherChanged += DeviceWatcherChangedd;
    //     bleCore.StartBleDeviceWatcher();
    //     while (true)
    //     {
    //         Thread.Sleep(10);
    //     }
    //     bleCore.Dispose();
    // }
    public void TriggerCharacteristicFinish(int size)
    {
        Console.WriteLine("call me");
        if (size <= 0)
        {
            Console.WriteLine("fail");
            connStep = BTConsts.STATUS_CONN_FAILED; //);
            Dispose();
            return;
        }

        Console.WriteLine("success");
        connStep = BTConsts.STATUS_CONN_SUCCESS;
    }

    public void TriggerRecdata(GattCharacteristic sender, byte[] data)
    {
        foreach (var b in data)
        {
            var tmp = b;
            rxData.Enqueue(tmp);
        }
    }

    public void TriggerCharacteristicAdded(GattCharacteristic gatt)
    {
        Console.WriteLine(
            "handle:[0x{0}]  char properties:[{1}]  UUID:[{2}]",
            gatt.AttributeHandle.ToString("X4"),
            gatt.CharacteristicProperties.ToString(),
            gatt.Uuid);
        characteristics.Add(gatt);
    }


    public void ConnectDevice(BluetoothLEDevice Device)
    {
        characteristics.Clear();
        StopBleDeviceWatcher();
        StartMatching(Device);
        FindService();
    }
}
#endif