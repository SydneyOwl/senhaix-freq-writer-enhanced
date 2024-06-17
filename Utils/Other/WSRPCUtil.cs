using System;
using System.Collections.Generic;
using System.Threading;
using Fleck;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Other;

public class WSRPCUtil
{
    private static WSRPCUtil instance;
    private IWebSocketConnection currentClient;
    private readonly Queue<string> NormalDataQueue = new();
    private WebSocketServer wsServer;

    public WSRPCUtil()
    {
        FleckLog.LogAction = (level, message, ex) =>
        {
            if (level > LogLevel.Info) DebugWindow.GetInstance().updateDebugContent(message);
        };
        // StartWSRPC();
    }

    public static WSRPCUtil GetInstance()
    {
        if (instance == null) instance = new WSRPCUtil();

        return instance;
    }

    private string SendRPCRequest(string method, string arg)
    {
        var data = JsonConvert.SerializeObject(new RPCRequest
        {
            method = method,
            arg = arg
        });
        var exc = currentClient.Send(data).Exception;
        if (exc != null) throw exc;
        while (NormalDataQueue.Count == 0) Thread.Sleep(10);
        var responseBody = NormalDataQueue.Dequeue();
        var resp = JsonConvert.DeserializeObject<RPCResponse>(responseBody);
        if (!string.IsNullOrEmpty(resp.error)) throw new Exception(resp.error);
        return resp.response;
    }

    private void SendRPCRequest(byte[] arg)
    {
        var exc = currentClient.Send(arg).Exception;
        if (exc != null) throw exc;
        while (NormalDataQueue.Count == 0) Thread.Sleep(10);
        var responseBody = NormalDataQueue.Dequeue();
        var resp = JsonConvert.DeserializeObject<RPCResponse>(responseBody);
        if (!string.IsNullOrEmpty(resp.error)) throw new Exception(resp.error);
    }

    public void StartWSRPC()
    {
        // wsServer?.Dispose();
        // currentClient = null;
        if (wsServer != null)
        {
            DebugWindow.GetInstance().updateDebugContent("ws已启动过！");
            return;
        }

        try
        {
            wsServer = new WebSocketServer(SETTINGS.WS_RPC_URL);
            wsServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    if (currentClient != null)
                    {
                        DebugWindow.GetInstance().updateDebugContent("拒绝连接");
                        socket.Close();
                        return;
                    }

                    currentClient = socket;
                    currentClient.OnError = err =>
                    {
                        DebugWindow.GetInstance().updateDebugContent($"出错:{err.Message}");
                        currentClient.Close();
                        currentClient = null;
                    };
                    currentClient.OnClose = () =>
                    {
                        DebugWindow.GetInstance().updateDebugContent($"客户端已断开:{socket.ConnectionInfo.Id}");
                        currentClient.Close();
                        currentClient = null;
                    };
                    currentClient.OnMessage = msg =>
                    {
                        // DebugWindow.GetInstance().updateDebugContent($"OnMessage Recv:{msg}");
                        NormalDataQueue.Enqueue(msg);
                    };
                    currentClient.OnBinary = data =>
                    {
                        // DebugWindow.GetInstance().updateDebugContent($"OnBinary Recv:{BitConverter.ToString(data)}");
                        if (data == null) return;
                        foreach (var b in data)
                        {
                            var tmp = b;
                            MySerialPort.GetInstance().RxData.Enqueue(tmp);
                        }
                    };
                    DebugWindow.GetInstance().updateDebugContent($"客户端已连接:{socket.ConnectionInfo.Id}");
                };
            });

            DebugWindow.GetInstance().updateDebugContent("ws已启动！");
        }
        catch (Exception b)
        {
            DebugWindow.GetInstance().updateDebugContent($"启动WS服务器失败：{b.Message}");
        }
    }

    public bool GetBleAvailability()
    {
        var resp = SendRPCRequest("GetBleAvailability", "");
        return resp == "True";
    }

    public string ScanForShx()
    {
        var resp = SendRPCRequest("ScanForShx", "");
        return resp;
    }

    public void SetDevice(string seq)
    {
        SendRPCRequest("SetDevice", seq);
    }

    public bool ConnectShxDevice()
    {
        return SendRPCRequest("ConnectShxDevice", "") == "True";
    }

    public bool ConnectShxRwService()
    {
        return SendRPCRequest("ConnectShxRwService", "") == "True";
    }

    public bool ConnectShxRwCharacteristic()
    {
        return SendRPCRequest("ConnectShxRwCharacteristic", "") == "True";
    }

    public bool WriteData(byte[] data)
    {
        SendRPCRequest(data);
        return true;
    }

    public void DisposeBluetooth()
    {
        SendRPCRequest("DisposeBluetooth", "");
    }

    // deprecated
    public void KeepAlive()
    {
        SendRPCRequest("KeepAlive", "");
    }

    public void TerminatePlugin()
    {
        SendRPCRequest("TerminatePlugin", "");
    }

    public void Shutdown()
    {
        currentClient?.Close();
        // wsServer?.Dispose();
        // wsServer = null;
        currentClient = null;
    }
}