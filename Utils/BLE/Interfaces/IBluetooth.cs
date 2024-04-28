using System.Threading.Tasks;

namespace SenhaixFreqWriter.Utils.BLE.Interfaces;

public interface IBluetooth
{
    public Task<bool> GetBLEAvailabilityAsync();
    public Task<bool> ScanForSHXAsync();
    public Task ConnectSHXDeviceAsync();
    public Task<bool> ConnectSHXRWServiceAsync();
    public Task<bool> ConnectSHXRWCharacteristicAsync();
    public void RegisterSerial();
    public void Dispose();
}