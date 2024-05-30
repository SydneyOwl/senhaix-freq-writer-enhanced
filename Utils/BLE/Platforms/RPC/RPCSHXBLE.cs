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
                filePath = "./"+SETTINGS.WINDOWS_BLE_PLUGIN_NAME;
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = "./"+SETTINGS.LINUX_BLE_PLUGIN_NAME;
                if (!File.Exists(filePath))
                {
                    DebugWindow.GetInstance().updateDebugContent($"未找到文件：{filePath}");
                    return false;
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = "./"+SETTINGS.OSX_BLE_PLUGIN_NAME;
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
                        Arguments =  SETTINGS.RPC_SERVER_PROCESS_ARGS,
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
        return ProxyClass.GetBleAvailability();
    }

    public List<GenerticBLEDeviceInfo> ScanForShxAsync(bool disableWeakSignalRestriction,
        bool disableSSIDFilter)
    {
        var result = ProxyClass.ScanForShx();
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
        ProxyClass.SetDevice(seq);
    }

    public bool ConnectShxDeviceAsync()
    {
        return ProxyClass.ConnectShxDevice();
    }

    public bool ConnectShxRwCharacteristicAsync()
    {
        return ProxyClass.ConnectShxRwCharacteristic();
    }

    public bool ConnectShxRwServiceAsync()
    {
        return ProxyClass.ConnectShxRwService();
    }

    public void RegisterHid()
    {
        HidTools.GetInstance().WriteBle =  (value) =>
        {
            ProxyClass.WriteData(value);
        };;
        Task.Run(() => UpdateRecvQueueHid(source.Token));
    }

    public void RegisterSerial()
    {
        MySerialPort.GetInstance().WriteBle = (value) =>
        {
            ProxyClass.WriteData(value);
        };
        Task.Run(() => UpdateRecvQueue(source.Token));
    }

    public void Dispose()
    {
        try
        {
            ProxyClass.DisposeBluetooth();
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
            var result = ProxyClass.ReadCachedData();
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
            var result = ProxyClass.ReadCachedData();
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
            ProxyClass.KeepAlive();
            Thread.Sleep(9500);
        }
    }
}

// RPC PART
// 没有想用的库其实
// 参考了RPC 1.0规范

public class RPCRequest
{
    public string method = "";
    public string arg = "";
}

public class RPCResponse
{
    public string response = "";
    public string error = "";
}

public class ProxyClass
{
    public static string Post(string method, string arg)
    {
        using (var client = new HttpClient()) // 创建HttpClient实例
        {
            var data = JsonConvert.SerializeObject(new RPCRequest
            {
                method = method,
                arg = arg
            });
            var content =
                new StringContent(data, Encoding.UTF8, "application/json"); // 创建HttpContent对象，设置Json内容、编码和媒体类型
            var response = client.PostAsync(SETTINGS.RPC_URL, content).Result; // 发送异步Post请求
            response.EnsureSuccessStatusCode(); // 确保响应状态码为200-399之间
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var resp = JsonConvert.DeserializeObject<RPCResponse>(responseBody);
            if (!string.IsNullOrEmpty(resp.error)) throw new Exception(resp.error);
            return resp.response;
        }
    }

    public static bool GetBleAvailability()
    {
        var resp = Post("GetBleAvailability", "");
        return resp == "True";
    }

    public static string ScanForShx()
    {
        var resp = Post("ScanForShx", "");
        return resp;
    }

    public static void SetDevice(string seq)
    {
        Post("SetDevice", seq);
    }

    public static bool ConnectShxDevice()
    {
        return Post("ConnectShxDevice", "") == "True";
    }

    public static bool ConnectShxRwService()
    {
        return Post("ConnectShxRwService", "") == "True";
    }

    public static bool ConnectShxRwCharacteristic()
    {
        return Post("ConnectShxRwCharacteristic", "") == "True";
    }

    public static byte[] ReadCachedData()
    {
        var encoded = Post("ReadCachedData", "");
        if (string.IsNullOrEmpty(encoded)) return null;
        return Convert.FromBase64String(encoded);
    }

    public static bool WriteData(byte[] data)
    {
        var enco = Convert.ToBase64String(data);
        Post("WriteData", enco);
        return true;
    }

    public static void DisposeBluetooth()
    {
        Post("DisposeBluetooth", "");
    }

    // deprecated
    public static void KeepAlive()
    {
        Post("KeepAlive", "");
    }
    public static void TerminatePlugin()
    {
        Post("TerminatePlugin", "");
    }
}