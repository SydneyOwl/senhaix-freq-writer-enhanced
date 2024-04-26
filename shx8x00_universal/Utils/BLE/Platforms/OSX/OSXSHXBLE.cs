using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InTheHand.Bluetooth;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using shx8x00.Constants;
using shx8x00.Utils.BLE.Interfaces;
using shx8x00.Utils.Serial;

namespace shx8x00.Utils.BLE.Platforms.OSX;

public class OSXSHXBLE : IBluetooth
{
    private IAdapter adapter;
    private IBluetoothLE ble;
    private List<IDevice> devices = new List<IDevice>();
    private IDevice shxDevice;
    private IService shxService;
    private ICharacteristic shxCharacteristic;

    public OSXSHXBLE()
    {
        adapter = CrossBluetoothLE.Current.Adapter;
        ble = CrossBluetoothLE.Current;
    }
   
    public async Task<bool> GetBLEAvailabilityAsync()
    {
        return ble.IsOn;
    }

    public async Task<bool> ScanForSHXAsync()
    {
        devices.Clear();
        adapter.DeviceDiscovered += (s, a) =>
        {
            devices.Add(a.Device);
        };
        adapter.ScanTimeout = 5000;
        await adapter.StartScanningForDevicesAsync();
        for (var i = 0; i < devices.Count; i++)
        {
            if (devices[i].Name.Equals(BLE_CONST.BTNAME_SHX8800))
            {
                shxDevice = devices[i];
                return true;
            }
        }

        return false;
    }

    public async Task ConnectSHXDeviceAsync()
    {
        await adapter.ConnectToDeviceAsync(shxDevice);
    }

    public async Task<bool> ConnectSHXRWServiceAsync()
    {
        shxService = await shxDevice.GetServiceAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BLE_CONST.RW_SERVICE_UUID.ToUpper(), 16)));
        return shxDevice != null;
    }

    public async Task<bool> ConnectSHXRWCharacteristicAsync()
    {
        shxCharacteristic =
            await shxService.GetCharacteristicAsync(
                BluetoothUuid.FromShortId(Convert.ToUInt16(BLE_CONST.RW_CHARACTERISTIC_UUID.ToUpper(), 16)));
        return shxCharacteristic != null;
    }

    public async void RegisterSerial()
    {
        shxCharacteristic.ValueUpdated += Characteristic_CharacteristicValueChanged;
        await shxCharacteristic.StartUpdatesAsync();
        MySerialPort.getInstance().WriteBLE = writeAsync;
    }

    private Task writeAsync(byte[] value)
    {
        return shxCharacteristic.WriteAsync(value);
    }
    private void Characteristic_CharacteristicValueChanged(object sender, CharacteristicUpdatedEventArgs e)
    {
        foreach (var b in e.Characteristic.Value) MySerialPort.getInstance().RxData.Enqueue(b);
    }
}