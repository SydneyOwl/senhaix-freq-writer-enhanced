using System;
using System.Collections.Generic;
using System.Threading;
using Fleck;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Other;

public class RpcRequest
{
    public string Arg = "";
    public string Method = "";
}

public class RpcResponse
{
    public string Error = "";
    public string Response = "";
}

public class WsrpcUtil
{
    private static WsrpcUtil _instance;
    private IWebSocketConnection _currentClient;
    private readonly Queue<string> _normalDataQueue = new();
    private WebSocketServer _wsServer;
    private Settings _settings = Settings.Load();


    public WsrpcUtil()
    {
        FleckLog.LogAction = (level, message, ex) =>
        {
            if (level > LogLevel.Info) DebugWindow.GetInstance().UpdateDebugContent(message);
        };
        // StartWSRPC();
    }

    public static WsrpcUtil GetInstance()
    {
        if (_instance == null) _instance = new WsrpcUtil();

        return _instance;
    }

    private string SendRpcRequest(string method, string arg)
    {
        var data = JsonConvert.SerializeObject(new RpcRequest
        {
            Method = method,
            Arg = arg
        });
        var exc = _currentClient.Send(data).Exception;
        if (exc != null) throw exc;
        while (_normalDataQueue.Count == 0) Thread.Sleep(10);
        var responseBody = _normalDataQueue.Dequeue();
        var resp = JsonConvert.DeserializeObject<RpcResponse>(responseBody);
        if (!string.IsNullOrEmpty(resp.Error)) throw new Exception(resp.Error);
        return resp.Response;
    }

    private void SendRpcRequest(byte[] arg)
    {
        var exc = _currentClient.Send(arg).Exception;
        if (exc != null) throw exc;
        while (_normalDataQueue.Count == 0) Thread.Sleep(10);
        var responseBody = _normalDataQueue.Dequeue();
        var resp = JsonConvert.DeserializeObject<RpcResponse>(responseBody);
        if (!string.IsNullOrEmpty(resp.Error)) throw new Exception(resp.Error);
    }

    public void StartWsrpc()
    {
        // wsServer?.Dispose();
        // currentClient = null;
        if (_wsServer != null)
        {
            DebugWindow.GetInstance().UpdateDebugContent("ws已启动过！");
            return;
        }

        try
        {
            _wsServer = new WebSocketServer(_settings.WsRpcUrl);
            _wsServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    if (_currentClient != null)
                    {
                        DebugWindow.GetInstance().UpdateDebugContent("拒绝连接");
                        socket.Close();
                        return;
                    }

                    _currentClient = socket;
                    _currentClient.OnError = err =>
                    {
                        DebugWindow.GetInstance().UpdateDebugContent($"出错:{err.Message}");
                        _currentClient.Close();
                        _currentClient = null;
                    };
                    _currentClient.OnClose = () =>
                    {
                        DebugWindow.GetInstance().UpdateDebugContent($"客户端已断开:{socket.ConnectionInfo.Id}");
                        _currentClient.Close();
                        _currentClient = null;
                    };
                    _currentClient.OnMessage = msg =>
                    {
                        // DebugWindow.GetInstance().updateDebugContent($"OnMessage Recv:{msg}");
                        _normalDataQueue.Enqueue(msg);
                    };
                    _currentClient.OnBinary = data =>
                    {
                        // DebugWindow.GetInstance().updateDebugContent($"OnBinary Recv:{BitConverter.ToString(data)}");
                        if (data == null) return;
                        foreach (var b in data)
                        {
                            var tmp = b;
                            MySerialPort.GetInstance().RxData.Enqueue(tmp);
                        }
                    };
                    DebugWindow.GetInstance().UpdateDebugContent($"客户端已连接:{socket.ConnectionInfo.Id}");
                };
            });

            DebugWindow.GetInstance().UpdateDebugContent("ws已启动！");
        }
        catch (Exception b)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"启动WS服务器失败：{b.Message}");
        }
    }

    public bool GetBleAvailability()
    {
        var resp = SendRpcRequest("GetBleAvailability", "");
        return resp == "True";
    }

    public string ScanForShx()
    {
        var resp = SendRpcRequest("ScanForShx", "");
        return resp;
    }

    public void SetDevice(string seq)
    {
        SendRpcRequest("SetDevice", seq);
    }

    public bool ConnectShxDevice()
    {
        return SendRpcRequest("ConnectShxDevice", "") == "True";
    }

    public bool ConnectShxRwService()
    {
        return SendRpcRequest("ConnectShxRwService", "") == "True";
    }

    public bool ConnectShxRwCharacteristic()
    {
        return SendRpcRequest("ConnectShxRwCharacteristic", "") == "True";
    }

    public bool WriteData(byte[] data)
    {
        SendRpcRequest(data);
        return true;
    }

    public void DisposeBluetooth()
    {
        SendRpcRequest("DisposeBluetooth", "");
    }

    // deprecated
    public void KeepAlive()
    {
        SendRpcRequest("KeepAlive", "");
    }

    public void TerminatePlugin()
    {
        SendRpcRequest("TerminatePlugin", "");
    }

    public void Shutdown()
    {
        _currentClient?.Close();
        // wsServer?.Dispose();
        // wsServer = null;
        _currentClient = null;
    }
}