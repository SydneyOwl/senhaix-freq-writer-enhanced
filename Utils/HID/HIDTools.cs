using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using HidSharp;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;

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

    public delegate Task WriteValueAsync(byte[] value);

    private Queue<byte> _rxData = new(1024);

    public WriteValueAsync WriteBle;

    public int BtDeviceMtu = 23;

    private CancellationTokenSource _pollTokenSource = new();

    private Mutex _mutex = new();

    public static bool IsShxhidExist()
    {
        var instance = new HidTools();
        instance.DevList = DeviceList.Local;
        return instance.DevList.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid) != null;
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
                // Console.WriteLine("changed...");
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
            Task.Run(()=>instance.pollDevStatus(instance.pollTokenSource.Token));
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
                // Console.WriteLine("Chlose");
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
        _mutex.WaitOne();
        if (IsDeviceConnected)
        {
            _mutex.ReleaseMutex();
            return HidStatus.Success;
        }

        // Console.WriteLine("alaysz");
        Gt12Device = DevList.GetHidDeviceOrNull(Gt12Hid.Vid, Gt12Hid.Pid);
        if (Gt12Device == null)
        {
            _mutex.ReleaseMutex();
            return HidStatus.DeviceNotFound;
        }

        OutputReportLength = Gt12Device.GetMaxOutputReportLength();
        InputReportLength = Gt12Device.GetMaxInputReportLength();
        if (Gt12Device.TryOpen(out HidStream))
        {
            HidStream.ReadTimeout = Timeout.Infinite;
            // Loop reading...
            BeginAsyncRead();
            _instance.IsDeviceConnected = true;
            _instance.UpdateLabel(true);

            _mutex.ReleaseMutex();
            return HidStatus.Success;
        }
        else
        {
            // Console.WriteLine("Not Connected");
            _mutex.ReleaseMutex();
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
            RxBuffer = array1;
            // Console.WriteLine(BitConverter.ToString(rxBuffer));
            FlagReceiveData = true;
            BeginAsyncRead();
            // else
            // {
            //     // Console.WriteLine("stop read...");
            //     CloseDevice();
            // }
        }
        catch (Exception e)
        {
            // Dispatcher.UIThread.Post(()=>
            // {
            //     MessageBoxManager.GetMessageBoxStandard("注意", "出错，请重新插拔设备！").ShowAsync();
            // });
            // Console.WriteLine("stop read. due to."+e.Message);
            // CloseDevice();
        }
    }

    private void BeginAsyncRead()
    {
        // Console.WriteLine("Reading...");
        var array = new byte[InputReportLength];
        var asyncResult = HidStream.BeginRead(array, 0, InputReportLength, ReadCompleted, array);
    }

    public HidStatus Write(Report r)
    {
        // Console.WriteLine("writing...");
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
                var num = 0;
                num = r.ReportBuff.Length >= OutputReportLength - 1
                    ? OutputReportLength - 1
                    : r.ReportBuff.Length;
                for (var i = 0; i < num; i++) array[i] = r.ReportBuff[i];
                Console.WriteLine(OutputReportLength);
                HidStream.Write(array, 0, OutputReportLength);
            }

            return HidStatus.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine("stop write. due to." + ex.Message);
            // CloseDevice();
            return HidStatus.NoDeviceConnected;
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
        if (Write(new Report(1, array)) != HidStatus.Success) return false;
        return true;
    }

    public void CloseDevice()
    {
        // Console.WriteLine("closed...");
        Gt12Device = null;
        IsDeviceConnected = false;
        HidStream.Close();
    }
}