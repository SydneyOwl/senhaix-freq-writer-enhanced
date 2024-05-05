using System;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Bluetooth;
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.Serial;

namespace SenhaixFreqWriter.Utils.BLE.Platforms.Generic;

public class GenerticSHXBLE : IBluetooth
{
    private GattCharacteristic shxCharacteristic;
    private BluetoothDevice shxDevice;

    private GattService shxService;

    public Task<bool> GetBLEAvailabilityAsync()
    {
        return Bluetooth.GetAvailabilityAsync();
    }

    public async Task<bool> ScanForSHXAsync()
    {
        var filter = new BluetoothLEScanFilter
        {
            Name = BLE_CONST.BTNAME_SHX8800
        };
        try
        {
            // 过滤名称
            shxDevice = await Bluetooth.RequestDeviceAsync(new RequestDeviceOptions { Filters = { filter } });
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
                if (discoveredDevice.Name.Equals(BLE_CONST.BTNAME_SHX8800))
                {
                    shxDevice = discoveredDevice;
                    break;
                }
        }

        return shxDevice != null;
    }

    public Task ConnectSHXDeviceAsync()
    {
        return shxDevice.Gatt.ConnectAsync();
    }

    public async Task<bool> ConnectSHXRWServiceAsync()
    {
        shxService = await shxDevice.Gatt.GetPrimaryServiceAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BLE_CONST.RW_SERVICE_UUID.ToUpper(), 16)));
        return shxService != null;
    }

    public async Task<bool> ConnectSHXRWCharacteristicAsync()
    {
        shxCharacteristic = await shxService.GetCharacteristicAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BLE_CONST.RW_CHARACTERISTIC_UUID.ToUpper(), 16)));
        return shxCharacteristic != null;
    }

    public async void RegisterSerial()
    {
        shxCharacteristic.CharacteristicValueChanged += Characteristic_CharacteristicValueChanged;
        MySerialPort.getInstance().BTDeviceMtu = shxDevice.Gatt.Mtu;
        await shxCharacteristic.StartNotificationsAsync();
        MySerialPort.getInstance().WriteBLE = shxCharacteristic.WriteValueWithoutResponseAsync;
    }

    public void RegisterHID()
    {
        throw new NotImplementedException();
    }

    private void Characteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        foreach (var b in e.Value) MySerialPort.getInstance().RxData.Enqueue(b);
    }

    public void Dispose()
    {
        shxDevice = null;
        shxCharacteristic = null;
        shxService = null;
    }

    public void setStatusUpdater(updater up)
    {
        throw new NotImplementedException();
    }
}