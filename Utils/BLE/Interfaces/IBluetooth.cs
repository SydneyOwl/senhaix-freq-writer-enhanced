using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.Utils.BLE.Interfaces;

public partial class GenerticBLEDeviceInfo : ObservableObject
{
    [ObservableProperty] private string _deviceID = "";
    [ObservableProperty] private string _deviceMacAddr;
    [ObservableProperty] private string _deviceName;
}

public interface IBluetooth
{
    public bool GetBleAvailabilityAsync();

    public List<GenerticBLEDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSSIDFilter);

    public void SetDevice(string seq);
    public bool ConnectShxDeviceAsync();
    public bool ConnectShxRwServiceAsync();
    public bool ConnectShxRwCharacteristicAsync();
    public void RegisterSerial();
    public void RegisterHid();
    public void Dispose();
}