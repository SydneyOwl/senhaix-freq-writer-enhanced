﻿using System;
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

public class WSRPCBLE : IBluetooth
{
    private readonly bool manual;
    private Process rpcClient;

    private readonly WSRPCUtil wsrpc;
    private SETTINGS Settings = SETTINGS.Load();

    public WSRPCBLE(bool useManual)
    {
        manual = useManual;
        wsrpc = WSRPCUtil.GetInstance();
        wsrpc.StartWSRPC();
    }

    // See BLEPlugin.go
    public bool GetBleAvailabilityAsync()
    {
        if (!manual)
        {
            var filePath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = Path.Join(AppContext.BaseDirectory, Settings.WindowsBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = Path.Join(AppContext.BaseDirectory, Settings.LinuxBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = Path.Join(AppContext.BaseDirectory, Settings.OsXBlePluginName);
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    filePath = $"{Settings.DataDir}/{Settings.OsXBlePluginName}";
                    // 在DATADIR里寻找
                    if (!File.Exists(filePath))
                    {
                        DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                        return false;
                    }
                }
            }

            if (rpcClient == null || rpcClient.HasExited)
            {
                rpcClient = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = Settings.RpcClientProcessArgs,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };
                rpcClient.OutputDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().updateDebugContent($"RPC Server: {args.Data}");
                };
                rpcClient.ErrorDataReceived += (sender, args) =>
                {
                    DebugWindow.GetInstance().updateDebugContent($"RPC Server: {args.Data}");
                };
                try
                {
                    if (!rpcClient.Start()) return false;
                    rpcClient.BeginOutputReadLine();
                    rpcClient.BeginErrorReadLine();
                    //Send keepalive packets
                    // Task.Run(()=>SendKeepAlive(source.Token));
                }
                catch (Exception b)
                {
                    DebugWindow.GetInstance().updateDebugContent(b.Message);
                    return false;
                }

                DebugWindow.GetInstance().updateDebugContent("RPC Start!");
                // 等待一秒
                Thread.Sleep(1000);
            }
        }

        return wsrpc.GetBleAvailability();
    }

    public List<GenerticBLEDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSSIDFilter)
    {
        var result = wsrpc.ScanForShx();
        var pattern = @"(\\[^bfrnt\\/'\""])";
        result = Regex.Replace(result, pattern, "\\$1");
        var bleDeviceInfo = JsonConvert.DeserializeObject<List<GenerticBLEDeviceInfo>>(result);
        List<GenerticBLEDeviceInfo> fin = new();
        foreach (var generticBleDeviceInfo in bleDeviceInfo)
        {
            if (!disableSSIDFilter &&
                generticBleDeviceInfo.DeviceName != BleConst.BtnameShx8800) continue;
            fin.Add(generticBleDeviceInfo);
        }

        return fin;
    }

    public void SetDevice(string seq)
    {
        wsrpc.SetDevice(seq);
    }

    public bool ConnectShxDeviceAsync()
    {
        return wsrpc.ConnectShxDevice();
    }

    public bool ConnectShxRwCharacteristicAsync()
    {
        return wsrpc.ConnectShxRwCharacteristic();
    }

    public bool ConnectShxRwServiceAsync()
    {
        return wsrpc.ConnectShxRwService();
    }

    public void RegisterHid()
    {
        HidTools.GetInstance().WriteBle = value => { wsrpc.WriteData(value); };
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = value => { wsrpc.WriteData(value); };
    }

    public void Dispose()
    {
        try
        {
            wsrpc.DisposeBluetooth();
            wsrpc.Shutdown();
            rpcClient?.CancelErrorRead();
            rpcClient?.CancelOutputRead();
        }
        catch
        {
            // ignored
        }

        try
        {
            rpcClient?.Kill();
            // rpcServer.WaitForExit();
            rpcClient = null;
            DebugWindow.GetInstance().updateDebugContent("Killed server!");
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
            wsrpc.KeepAlive();
            Thread.Sleep(9500);
        }
    }
}