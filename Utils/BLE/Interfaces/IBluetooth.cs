using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.Utils.BLE.Interfaces;

public partial class GenerticBleDeviceInfo : ObservableObject
{
    [ObservableProperty] private string _deviceId = "";
    [ObservableProperty] private string _deviceMacAddr;
    [ObservableProperty] private string _deviceName;
}

public interface IBluetooth
{
    public bool GetBleAvailabilityAsync();

    public List<GenerticBleDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSsidFilter);

    public void SetDevice(string seq);
    public bool ConnectShxDeviceAsync();
    public bool ConnectShxRwServiceAsync();
    public bool ConnectShxRwCharacteristicAsync();
    public void RegisterSerial();
    public void RegisterHid();
    public void Dispose();
}