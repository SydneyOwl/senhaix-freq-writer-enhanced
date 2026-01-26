using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.BLE.Platforms.RPC;

public class Wsrpcble : IBluetooth
{
    private readonly bool _manual;
    private readonly Settings _settings = Settings.Load();

    private readonly WsrpcUtil _wsrpc;
    private Process _rpcClient;

    public Wsrpcble(bool useManual)
    {
        _manual = useManual;
        _wsrpc = WsrpcUtil.GetInstance();
        _wsrpc.StartWsrpc();
    }

    // See BLEPlugin.go
    public bool GetBleAvailabilityAsync()
    {
        if (!_manual)
        {
            var filePath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = Path.Join(AppContext.BaseDirectory, _settings.WindowsBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = Path.Join(AppContext.BaseDirectory, _settings.LinuxBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = Path.Join(AppContext.BaseDirectory, _settings.OsXBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"未找到文件：{filePath}");
                    filePath = $"{_settings.DataDir}/{_settings.OsXBlePluginName}";
                    // 在DATADIR里寻找
                    if (!File.Exists(filePath))
                    {
                        DebugWindow.GetInstance().UpdateDebugContent($"未找到文件：{filePath}");
                        return false;
                    }
                }
            }

            if (_rpcClient == null || _rpcClient.HasExited)
            {
                _rpcClient = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = _settings.RpcClientProcessArgs,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };
                _rpcClient.OutputDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"RPC Server: {args.Data}");
                };
                _rpcClient.ErrorDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"RPC Server: {args.Data}");
                };
                try
                {
                    if (!_rpcClient.Start()) return false;
                    _rpcClient.BeginOutputReadLine();
                    _rpcClient.BeginErrorReadLine();
                    //Send keepalive packets
                    // Task.Run(()=>SendKeepAlive(source.Token));
                }
                catch (Exception b)
                {
                    DebugWindow.GetInstance().UpdateDebugContent(b.Message);
                    return false;
                }

                DebugWindow.GetInstance().UpdateDebugContent("RPC Start!");
                // 等待一秒
                Thread.Sleep(1000);
            }
        }

        return _wsrpc.GetBleAvailability();
    }

    public List<GenerticBleDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSsidFilter)
    {
        var result = _wsrpc.ScanForShx();
        var pattern = @"(\\[^bfrnt\\/'\""])";
        result = Regex.Replace(result, pattern, "\\$1");
        var bleDeviceInfo = JsonConvert.DeserializeObject<List<GenerticBleDeviceInfo>>(result);
        List<GenerticBleDeviceInfo> fin = new();
        foreach (var generticBleDeviceInfo in bleDeviceInfo)
        {
            if (!disableSsidFilter &&
                generticBleDeviceInfo.DeviceName != BleConst.BtnameShx8800) continue;
            fin.Add(generticBleDeviceInfo);
        }

        return fin;
    }

    public void SetDevice(string seq)
    {
        _wsrpc.SetDevice(seq);
    }

    public bool ConnectShxDeviceAsync()
    {
        return _wsrpc.ConnectShxDevice();
    }

    public bool ConnectShxRwCharacteristicAsync()
    {
        return _wsrpc.ConnectShxRwCharacteristic();
    }

    public bool ConnectShxRwServiceAsync()
    {
        return _wsrpc.ConnectShxRwService();
    }

    public void RegisterHid()
    {
        HidTools.GetInstance().WriteBle = value => { _wsrpc.WriteData(value); };
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = value => { _wsrpc.WriteData(value); };
    }

    public void Dispose()
    {
        try
        {
            _wsrpc.DisposeBluetooth();
            _wsrpc.Shutdown();
            _rpcClient?.CancelErrorRead();
            _rpcClient?.CancelOutputRead();
        }
        catch
        {
            // ignored
        }

        try
        {
            _rpcClient?.Kill();
            // rpcServer.WaitForExit();
            _rpcClient = null;
            DebugWindow.GetInstance().UpdateDebugContent("Killed server!");
        }
        catch
        {
            // ignore
        }

        MySerialPort.GetInstance().WriteBle = null;
    }

    private void SendKeepAlive(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _wsrpc.KeepAlive();
            Thread.Sleep(9500);
        }
    }
}