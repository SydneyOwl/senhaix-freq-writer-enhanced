using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HidSharp;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.HID;

public class HidTools
{
    public HidDevice Gt12Device;

    public int OutputReportLength;

    public int InputReportLength;

    public HidStream HidStream;

    private static HidTools _instance;

    public bool IsDeviceConnected = false;

    public DeviceList DevList;

    public delegate void UpdateMainUiThread(bool connected);

    public UpdateMainUiThread UpdateLabel;

    //TODO: enhance
    public byte[] RxBuffer = new byte[64];

    public bool FlagReceiveData;

    public delegate void WriteValueAsync(byte[] value);

    public WriteValueAsync WriteBle;

    public int BtDeviceMtu = 23;

    private CancellationTokenSource _pollTokenSource = new();

    private Mutex _mutexSearch = new();

    private Mutex _mutexSend = new();

    private void UpdateChanDebugInfo(string a)
    {
        if (!SETTINGS.DISABLE_DEBUG_CHAN_DATA_OUTPUT) DebugWindow.GetInstance().updateDebugContent(a);
    }

    public static bool IsShxGt12HidExist()
    {
        return DeviceList.Local.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid) != null;
    }

    public static List<HidDevice> GetAllHidDevices()
    {
        return DeviceList.Local.GetHidDevices().ToList();
    }

    public static HidTools GetInstance()
    {
        if (_instance == null)
        {
            _instance = new HidTools();
            _instance.DevList = DeviceList.Local;

            // On unix system we poll!
#if WINDOWS
            _instance.DevList.Changed += (sender, args) =>
            {
                // DebugWindow.GetInstance().updateDebugContent("changed...");
                if (_instance.DevList.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid) == null)
                {
                    _instance.IsDeviceConnected = false;
                    _instance.UpdateLabel(false);
                }
                else
                {
                    _instance.FindAndConnect();
                }
            };
#else
            Task.Run(() => _instance.PollDevStatus(_instance._pollTokenSource.Token));
#endif
        }

        return _instance;
    }

    private void PollDevStatus(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (DevList.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid) == null)
            {
                // DebugWindow.GetInstance().updateDebugContent("Chlose");
                IsDeviceConnected = false;
                UpdateLabel(false);
                // requestReconnect = false;
                // hidStream?.Dispose();
                // hidStream = null;
            }
            else
            {
                FindAndConnect();
            }

            Thread.Sleep(100);
        }
    }

    public HidStatus FindAndConnect()
    {
        _mutexSearch.WaitOne();
        if (IsDeviceConnected)
        {
            _mutexSearch.ReleaseMutex();
            return HidStatus.Success;
        }

        // DebugWindow.GetInstance().updateDebugContent("alaysz");
        Gt12Device = DevList.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid);
        if (Gt12Device == null)
        {
            _mutexSearch.ReleaseMutex();
            return HidStatus.DeviceNotFound;
        }

        OutputReportLength = Gt12Device.GetMaxOutputReportLength();
        InputReportLength = Gt12Device.GetMaxInputReportLength();
        if (Gt12Device.TryOpen(out HidStream))
        {
            HidStream.WriteTimeout = 5000;
            HidStream.ReadTimeout = Timeout.Infinite;
            // Loop reading...
            BeginAsyncRead();
            _instance.IsDeviceConnected = true;
            _instance.UpdateLabel(true);

            _mutexSearch.ReleaseMutex();
            return HidStatus.Success;
        }
        else
        {
            // DebugWindow.GetInstance().updateDebugContent("Not Connected");
            _mutexSearch.ReleaseMutex();
            return HidStatus.NoDeviceConnected;
        }
    }

    private void ReadCompleted(IAsyncResult iResult)
    {
        var array = (byte[])iResult.AsyncState;
        try
        {
            HidStream.EndRead(iResult);
            // var array2 = new byte[array.Length];
            // for (var i = 0; i < array.Length; i++) array2[i] = array[i];

            var e = new Report(array[0], array);
            var array1 = new byte[64];
            Array.Copy(e.ReportBuff, 0, array1, 0, 64);
            UpdateChanDebugInfo($"收到数据（长度{array1.Length}）：{BitConverter.ToString(array1)}");
            RxBuffer = array1;
            // DebugWindow.GetInstance().updateDebugContent(BitConverter.ToString(rxBuffer));
            FlagReceiveData = true;
            // Console.Write("Read!");
            BeginAsyncRead();
            // else
            // {
            //     // DebugWindow.GetInstance().updateDebugContent("stop read...");
            //     CloseDevice();
            // }
        }
        catch (Exception e)
        {
            // Dispatcher.UIThread.Post(()=>
            // {
            //     MessageBoxManager.GetMessageBoxStandard("注意", "出错，请重新插拔设备！").ShowAsync();
            // });
            // DebugWindow.GetInstance().updateDebugContent("stop read. due to."+e.Message);
            // CloseDevice();
        }
    }

    private void BeginAsyncRead()
    {
        // DebugWindow.GetInstance().updateDebugContent("Reading...");
        var array = new byte[InputReportLength];
        var asyncResult = HidStream.BeginRead(array, 0, InputReportLength, ReadCompleted, array);
    }

    public HidStatus Write(Report r)
    {
        // DebugWindow.GetInstance().updateDebugContent("writing...");
        _mutexSend.WaitOne();
        try
        {
            if (WriteBle != null)
            {
                var array = new byte[BtDeviceMtu - 1];
                var num = 0;
                // num = r.reportBuff.Length >= BTDeviceMtu - 1
                //     ? BTDeviceMtu - 1
                //     : r.reportBuff.Length;
                // for (var i = 0; i < num; i++) array[i] = r.reportBuff[i];
                // WriteBLE(array);
                var tobeWrite = BytesTrimEnd(r.ReportBuff);
                var singleSize = BtDeviceMtu - 1;
                var sendTimes = tobeWrite.Length / singleSize;
                var tmp = 0;
                for (var i = 0; i < sendTimes + 1; i++)
                {
                    if (i == sendTimes)
                    {
                        WriteBle(tobeWrite.Skip(tmp)
                            .Take(tobeWrite.Length - sendTimes * singleSize).ToArray());
                        break;
                    }

                    WriteBle(tobeWrite.Skip(tmp).Take(singleSize).ToArray());
                    tmp += singleSize;
                }
            }
            else
            {
                var array = new byte[OutputReportLength];
                // var num = 0;
                // num = r.ReportBuff.Length >= OutputReportLength - 1
                //     ? OutputReportLength - 1
                //     : r.ReportBuff.Length;
                // DebugWindow.GetInstance().updateDebugContent(OutputReportLength);
                for (var i = 0; i < r.ReportBuff.Length; i++) array[i] = r.ReportBuff[i];
                HidStream.Write(array, 0, OutputReportLength);
            }

            // Console.Write("Write!");
            return HidStatus.Success;
        }
        catch (Exception ex)
        {
            // DebugWindow.GetInstance().updateDebugContent("stop write. due to." + ex.Message);
            // CloseDevice();
            return HidStatus.NoDeviceConnected;
        }
        finally
        {
            _mutexSend.ReleaseMutex();
        }

        return HidStatus.WriteFaild;
    }

    public byte[] BytesTrimEnd(byte[] bytes)
    {
        var list = bytes.ToList();
        for (var i = bytes.Length - 1; i >= 0; i--)
            if (bytes[i] == 0x00)
                list.RemoveAt(i);
            else
                break;
        return list.ToArray();
    }

    public bool Send(byte[] byData)
    {
        var array = new byte[byData.Length];
        Array.Copy(byData, 0, array, 0, byData.Length);
        UpdateChanDebugInfo($"发送数据（长度{array.Length}，蓝牙状态{WriteBle != null}）：{BitConverter.ToString(array)}");
        if (Write(new Report(1, array)) != HidStatus.Success) return false;
        return true;
    }

    public void CloseDevice()
    {
        // DebugWindow.GetInstance().updateDebugContent("closed...");
        Gt12Device = null;
        IsDeviceConnected = false;
        HidStream.Close();
    }
}