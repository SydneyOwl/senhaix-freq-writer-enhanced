using System.Threading.Tasks;

namespace SenhaixFreqWriter.Utils.BLE.Interfaces;

public delegate void Updater(bool status);

public interface IBluetooth
{
    public Task<bool> GetBleAvailabilityAsync();
    public Task<bool> ScanForShxAsync();
    public Task ConnectShxDeviceAsync();
    public Task<bool> ConnectShxRwServiceAsync();
    public Task<bool> ConnectShxRwCharacteristicAsync();
    public void RegisterSerial();
    public void RegisterHid();
    public void Dispose();
    public void SetStatusUpdater(Updater up);
}