using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using HidSharp;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Utils.HID;

public class HIDTools
{
    public HidDevice Gt12Device;

    public int OutputReportLength;

    public int InputReportLength;

    public HidStream hidStream;

    private static HIDTools instance;

    public bool isDeviceConnected = false;

    public DeviceList devList;
    
    public delegate void updateMainUIThread(bool connected);

    public updateMainUIThread updateLabel;


    //TODO: enhance
    public byte[] rxBuffer = new byte[64];
    public bool flagReceiveData;

    public bool requestReconnect;
    // For unix
    private CancellationTokenSource pollTokenSource;

    private Mutex _mutex = new();
    public static bool isSHXHIDExist()
    {
        var instance = new HIDTools();
        instance.devList = DeviceList.Local;
        return instance.devList.GetHidDeviceOrNull(GT12_HID.VID, GT12_HID.PID) != null;
    }

    public static HIDTools getInstance()
    {
        if (instance == null)
        {
            instance = new HIDTools();
            instance.pollTokenSource = new CancellationTokenSource();
            instance.devList = DeviceList.Local;
            
            // On unix system we poll!
#if WINDOWS
            instance.devList.Changed += (sender, args) =>
            {
                // Console.WriteLine("changed...");
                if (instance.devList.GetHidDeviceOrNull(GT12_HID.VID, GT12_HID.PID) == null)
                {
                    instance.isDeviceConnected = false;
                    instance.updateLabel(false);
                }
                else
                {
                    instance.findAndConnect();
                }
            };
#else
            Task.Run(()=>instance.pollDevStatus(instance.pollTokenSource.Token));
#endif
        }
        return instance;
    }

    private void pollDevStatus(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (devList.GetHidDeviceOrNull(GT12_HID.VID, GT12_HID.PID) == null )
            { 
                // Console.WriteLine("Chlose");
                isDeviceConnected = false;
                updateLabel(false);
                // requestReconnect = false;
                // hidStream?.Dispose();
                // hidStream = null;
            }
            else
            {
                findAndConnect();
            } 
            Thread.Sleep(100);
        }
    }
    public HID_STATUS findAndConnect()
    {
        _mutex.WaitOne();
        if (isDeviceConnected)
        {
            _mutex.ReleaseMutex();
            return HID_STATUS.SUCCESS;
        }
        // Console.WriteLine("alaysz");
        Gt12Device = devList.GetHidDeviceOrNull(GT12_HID.VID, GT12_HID.PID);
        if (Gt12Device == null)
        {
            _mutex.ReleaseMutex();
            return HID_STATUS.DEVICE_NOT_FOUND;
        }
        OutputReportLength = Gt12Device.GetMaxInputReportLength();
        InputReportLength = Gt12Device.GetMaxInputReportLength();
        if (Gt12Device.TryOpen(out hidStream))
        {
            hidStream.ReadTimeout = Timeout.Infinite;
            // Loop reading...
            BeginAsyncRead();
            instance.isDeviceConnected = true;
            instance.updateLabel(true);
            
            _mutex.ReleaseMutex();
            return HID_STATUS.SUCCESS;
        }
        else
        {
            // Console.WriteLine("Not Connected");
            _mutex.ReleaseMutex();
            return HID_STATUS.NO_DEVICE_CONNECTED;
        }
    }

    private void ReadCompleted(IAsyncResult iResult)
    {
        var array = (byte[])iResult.AsyncState;
        try
        {
            hidStream.EndRead(iResult);
            // var array2 = new byte[array.Length];
            // for (var i = 0; i < array.Length; i++) array2[i] = array[i];

            var e = new Report(array[0], array);
            var array1 = new byte[64];
            Array.Copy(e.reportBuff, 0, array1, 0, 64);
            rxBuffer = array1;
            // Console.WriteLine(BitConverter.ToString(rxBuffer));
            flagReceiveData = true;
            BeginAsyncRead();
            // else
            // {
            //     // Console.WriteLine("stop read...");
            //     CloseDevice();
            // }
        }
        catch(Exception e)
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
        var asyncResult = hidStream.BeginRead(array, 0, InputReportLength, ReadCompleted, array);
    }

    public HID_STATUS Write(Report r)
    {
        // Console.WriteLine("writing...");
        try
        {
            var array = new byte[OutputReportLength];
            var num = 0;
            num = r.reportBuff.Length >= OutputReportLength - 1
                ? OutputReportLength - 1
                : r.reportBuff.Length;
            for (var i = 0; i < num; i++) array[i] = r.reportBuff[i];
        
            hidStream.Write(array, 0, OutputReportLength);
            return HID_STATUS.SUCCESS;
        }
        catch (Exception ex)
        {
            // Console.WriteLine("stop write. due to."+ex.Message);
            // CloseDevice();
            return HID_STATUS.NO_DEVICE_CONNECTED;
        }
            
        return HID_STATUS.WRITE_FAILD;
    }

    public bool Send(byte[] byData)
    {
        var array = new byte[byData.Length];
        Array.Copy(byData, 0, array, 0, byData.Length);
        if (Write(new Report(1, array)) != HID_STATUS.SUCCESS) return false;
        return true;
    }

    public void CloseDevice()
    {
        // Console.WriteLine("closed...");
        Gt12Device = null;
        isDeviceConnected = false;
        hidStream.Close();
    }
}