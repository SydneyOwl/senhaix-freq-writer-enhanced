using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.BLE.Platforms.RPC;

public class RPCSHXBLE : IBluetooth
{
    private CancellationTokenSource source = new();

    private Process rpcServer;

    private bool manual;
    
    public RPCSHXBLE(bool useManual)
    {
        manual = useManual;
    }

    // See BLEPlugin.py
    public bool GetBleAvailabilityAsync()
    {
        if (!manual)
        {
            var filePath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = Path.Join(AppContext.BaseDirectory,SETTINGS.WINDOWS_BLE_PLUGIN_NAME);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = Path.Join(AppContext.BaseDirectory,SETTINGS.LINUX_BLE_PLUGIN_NAME);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = Path.Join(AppContext.BaseDirectory,SETTINGS.OSX_BLE_PLUGIN_NAME);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    filePath = $"{SETTINGS.DATA_DIR}/{SETTINGS.OSX_BLE_PLUGIN_NAME}";
                    // 在DATADIR里寻找
                    if (!File.Exists(filePath))
                    {
                        DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                        return false;
                    }
                }
            }
            if (rpcServer == null || rpcServer.HasExited)
            {
                rpcServer = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments =  SETTINGS.RPC_CLIENT_PROCESS_ARGS,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    }
                };
                rpcServer.OutputDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().updateDebugContent($"RPC Server: {args.Data}");
                };
                rpcServer.ErrorDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().updateDebugContent($"RPC Server: {args.Data}");
                };
                try
                {
                    if (!rpcServer.Start())
                    {
                        return false;
                    }
                    rpcServer.BeginOutputReadLine();
                    rpcServer.BeginErrorReadLine();
                    //Send keepalive packets
                    // Task.Run(()=>SendKeepAlive(source.Token));
                }
                catch(Exception b)
                {
                    DebugWindow.GetInstance().updateDebugContent(b.Message);
                    return false;
                }
                DebugWindow.GetInstance().updateDebugContent("RPC Start!");
            }
        }
        return RPCUtil.GetBleAvailability();
    }

    public List<GenerticBLEDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSSIDFilter)
    {
        var result = RPCUtil.ScanForShx();
        var pattern = @"(\\[^bfrnt\\/'\""])";
        result = Regex.Replace(result, pattern, "\\$1");
        List<GenerticBLEDeviceInfo> bleDeviceInfo = JsonConvert.DeserializeObject<List<GenerticBLEDeviceInfo>>(result);
        List<GenerticBLEDeviceInfo> fin = new();
        foreach (var generticBleDeviceInfo in bleDeviceInfo)
        {
            if (!disableSSIDFilter &&
                generticBleDeviceInfo.DeviceName != Constants.BLE.BleConst.BtnameShx8800) continue;
            fin.Add(generticBleDeviceInfo);
        }

        return fin;
    }

    public void SetDevice(string seq)
    {
        RPCUtil.SetDevice(seq);
    }

    public bool ConnectShxDeviceAsync()
    {
        return RPCUtil.ConnectShxDevice();
    }

    public bool ConnectShxRwCharacteristicAsync()
    {
        return RPCUtil.ConnectShxRwCharacteristic();
    }

    public bool ConnectShxRwServiceAsync()
    {
        return RPCUtil.ConnectShxRwService();
    }

    public void RegisterHid()
    {
        HidTools.GetInstance().WriteBle =  (value) =>
        {
            RPCUtil.WriteData(value);
        };;
        Task.Run(() => UpdateRecvQueueHid(source.Token));
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = (value) =>
        {
            RPCUtil.WriteData(value);
        };
        Task.Run(() => UpdateRecvQueue(source.Token));
    }

    public void Dispose()
    {
        try
        {
            RPCUtil.DisposeBluetooth();
            rpcServer?.CancelErrorRead();
            rpcServer?.CancelOutputRead();
        }
        catch
        {
            // ignored
        }
        try
        {
            source.Cancel();
            rpcServer?.Kill();
            // rpcServer.WaitForExit();
            rpcServer = null;
            DebugWindow.GetInstance().updateDebugContent("Killed server!");
        }
        catch
        {
            // ignore
        }
        MySerialPort.GetInstance().WriteBle = null;
    }

    public void SetStatusUpdater(Updater up)
    {
        throw new NotImplementedException();
    }

    private void UpdateRecvQueue(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Thread.Sleep(100);
            var result = RPCUtil.ReadCachedData();
            if (result == null) continue;
            foreach (var b in result)
            {
                var tmp = b;
                MySerialPort.GetInstance().RxData.Enqueue(tmp);
            }
        }
    }
    private void UpdateRecvQueueHid(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var result = RPCUtil.ReadCachedData();
            if (result == null) continue;
            HidTools.GetInstance().RxBuffer = result;
            HidTools.GetInstance().FlagReceiveData = true;
            Thread.Sleep(100);
        }
    }

    private void SendKeepAlive(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            RPCUtil.KeepAlive();
            Thread.Sleep(9500);
        }
    }
}