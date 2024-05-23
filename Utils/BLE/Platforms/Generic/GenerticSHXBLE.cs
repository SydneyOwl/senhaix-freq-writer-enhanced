#if false
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Bluetooth;
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.Serial;

namespace SenhaixFreqWriter.Utils.BLE.Platforms.Generic;

// DEPRECATED!

public class GenerticShxble : IBluetooth
{
    private GattCharacteristic _shxCharacteristic;
    private BluetoothDevice _shxDevice;

    private GattService _shxService;

    public Task<bool> GetBleAvailabilityAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Task.Run(() => false);
        return Bluetooth.GetAvailabilityAsync();
    }

    public async Task<bool> ScanForShxAsync()
    {
        var filter = new BluetoothLEScanFilter
        {
            Name = BleConst.BtnameShx8800
        };
        try
        {
            // 过滤名称
            _shxDevice = await Bluetooth.RequestDeviceAsync(new RequestDeviceOptions { Filters = { filter } });
        }
        catch
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var discoveredDevices = await Bluetooth.ScanForDevicesAsync(new RequestDeviceOptions
            {
                Filters = { filter }
            }, cts.Token);
            foreach (var discoveredDevice in discoveredDevices)
                if (discoveredDevice.Name.Equals(BleConst.BtnameShx8800))
                {
                    _shxDevice = discoveredDevice;
                    break;
                }
        }

        return _shxDevice != null;
    }

    public Task ConnectShxDeviceAsync()
    {
        return _shxDevice.Gatt.ConnectAsync();
    }

    public async Task<bool> ConnectShxRwServiceAsync()
    {
        _shxService = await _shxDevice.Gatt.GetPrimaryServiceAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BleConst.RwServiceUuid.ToUpper(), 16)));
        return _shxService != null;
    }

    public async Task<bool> ConnectShxRwCharacteristicAsync()
    {
        _shxCharacteristic = await _shxService.GetCharacteristicAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BleConst.RwCharacteristicUuid.ToUpper(), 16)));
        return _shxCharacteristic != null;
    }

    public async void RegisterSerial()
    {
        _shxCharacteristic.CharacteristicValueChanged += Characteristic_CharacteristicValueChanged;
        MySerialPort.GetInstance().BtDeviceMtu = _shxDevice.Gatt.Mtu;
        await _shxCharacteristic.StartNotificationsAsync();
        MySerialPort.GetInstance().WriteBle = _shxCharacteristic.WriteValueWithoutResponseAsync;
    }

    public void RegisterHid()
    {
        throw new NotImplementedException();
    }

    private void Characteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        foreach (var b in e.Value) MySerialPort.GetInstance().RxData.Enqueue(b);
    }

    public void Dispose()
    {
        _shxDevice = null;
        _shxCharacteristic = null;
        _shxService = null;
    }

    public void SetStatusUpdater(Updater up)
    {
        throw new NotImplementedException();
    }
}
#endif