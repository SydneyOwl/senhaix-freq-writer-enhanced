using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter.Utils.Other;

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

public class RPCUtil
{
    private static readonly HttpClient client = new();

    public static string SendRPCRequest(string method, string arg)
    {
        var data = JsonConvert.SerializeObject(new RPCRequest
        {
            method = method,
            arg = arg
        });
        var content =
            new StringContent(data, Encoding.UTF8, "application/json");
        var response = client.PostAsync(SETTINGS.RPC_URL, content).Result;
        response.EnsureSuccessStatusCode(); // 确保响应状态码为200-399之间
        var responseBody = response.Content.ReadAsStringAsync().Result;
        var resp = JsonConvert.DeserializeObject<RPCResponse>(responseBody);
        if (!string.IsNullOrEmpty(resp.error)) throw new Exception(resp.error);
        return resp.response;
    }

    public static bool GetBleAvailability()
    {
        var resp = SendRPCRequest("GetBleAvailability", "");
        return resp == "True";
    }

    public static string ScanForShx()
    {
        var resp = SendRPCRequest("ScanForShx", "");
        return resp;
    }

    public static void SetDevice(string seq)
    {
        SendRPCRequest("SetDevice", seq);
    }

    public static bool ConnectShxDevice()
    {
        return SendRPCRequest("ConnectShxDevice", "") == "True";
    }

    public static bool ConnectShxRwService()
    {
        return SendRPCRequest("ConnectShxRwService", "") == "True";
    }

    public static bool ConnectShxRwCharacteristic()
    {
        return SendRPCRequest("ConnectShxRwCharacteristic", "") == "True";
    }

    public static byte[] ReadCachedData()
    {
        var encoded = SendRPCRequest("ReadCachedData", "");
        if (string.IsNullOrEmpty(encoded)) return null;
        return Convert.FromBase64String(encoded);
    }

    public static bool WriteData(byte[] data)
    {
        var enco = Convert.ToBase64String(data);
        SendRPCRequest("WriteData", enco);
        return true;
    }

    public static void DisposeBluetooth()
    {
        SendRPCRequest("DisposeBluetooth", "");
    }

    // deprecated
    public static void KeepAlive()
    {
        SendRPCRequest("KeepAlive", "");
    }

    public static void TerminatePlugin()
    {
        SendRPCRequest("TerminatePlugin", "");
    }
}