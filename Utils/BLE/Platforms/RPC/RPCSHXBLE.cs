using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.BLE.Platforms.RPC;

public class RPCSHXBLE : IBluetooth
{
    private CancellationTokenSource source = new();
    private IProxyInterface proxy = (IProxyInterface)XmlRpcProxyGen.Create(typeof(IProxyInterface));
    // See BLEPlugin.py
    public bool GetBleAvailabilityAsync()
    {
        return proxy.GetBleAvailability();
    }

    public List<GenerticBLEDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSSIDFilter)
    {
        var result = proxy.ScanForShx();
        string pattern = @"(\\[^bfrnt\\/'\""])";
        result = Regex.Replace(result, pattern, "\\$1");
        List<GenerticBLEDeviceInfo> bleDeviceInfo = JsonConvert.DeserializeObject<List<GenerticBLEDeviceInfo>>(result);
        List<GenerticBLEDeviceInfo> fin = new();
        foreach (var generticBleDeviceInfo in bleDeviceInfo)
        {
            if (!disableSSIDFilter && generticBleDeviceInfo.DeviceName != Constants.BLE.BleConst.BtnameShx8800)
            {
                continue;
            }
            fin.Add(generticBleDeviceInfo);
        }

        return fin;
    }

    public void SetDevice(string seq)
    {
        proxy.setDevice(seq);
    }

    public bool ConnectShxDeviceAsync()
    {
        return proxy.ConnectShxDevice();
    }

    public bool ConnectShxRwCharacteristicAsync()
    {
        return proxy.ConnectShxRwCharacteristic();
    }

    public bool ConnectShxRwServiceAsync()
    {
        return proxy.ConnectShxRwService();
    }

    public void RegisterHid()
    {
        throw new System.NotImplementedException();
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = (value) =>
        {
            proxy.WriteData(value);
            return Task.Run(()=>{});
        };
        Task.Run(()=>UpdateRecvQueue(source.Token));
    }

    public void Dispose()
    {
        try
        {
            proxy.DisposeBluetooth();
            source.Cancel();
        }
        catch
        {
            // ignore
        }
    }

    public void SetStatusUpdater(Updater up)
    {
        throw new System.NotImplementedException();
    }

    private void UpdateRecvQueue(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var result = proxy.ReadCachedData();
            if (result == null)
            {
                continue;
            }
            foreach (var b in result)
            {
                var tmp = b;
                MySerialPort.GetInstance().RxData.Enqueue(tmp);
            }
        }
    }

}

[XmlRpcUrl(SETTINGS.RPC_URL)]
public interface IProxyInterface : IXmlRpcProxy
{
    [XmlRpcMethod("GetBleAvailability")]
    bool GetBleAvailability();
    
    [XmlRpcMethod("ScanForShx")]
    string ScanForShx();
    
    [XmlRpcMethod("setDevice")]
    void setDevice(string seq);
    
    [XmlRpcMethod("ConnectShxDevice")]
    bool ConnectShxDevice();
    
    [XmlRpcMethod("ConnectShxRwService")]
    bool ConnectShxRwService();
    
    [XmlRpcMethod("ConnectShxRwCharacteristic")]
    bool ConnectShxRwCharacteristic();
    
    [XmlRpcMethod("ReadCachedData")]
    byte[] ReadCachedData();
    
    [XmlRpcMethod("WriteData")]
    bool WriteData(byte[] data);
    
    [XmlRpcMethod("DisposeBluetooth")]
    void DisposeBluetooth();
}