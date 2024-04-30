using System;
using System.Threading;
using HidSharp;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Utils.HID;

public class HIDTools
{
    public HidDevice Gt12Device;

    public int OutputReportLength;

    public int InputReportLength;

    public HidStream hidStream;

    public static HIDTools instance;


    //TODO: enhance
    public byte[] rxBuffer = new byte[64];
    public bool flagReceiveData;

    public static HIDTools getInstance()
    {
        if (instance == null) instance = new HIDTools();

        return instance;
    }

    public HID_STATUS findAndConnect()
    {
        var list = DeviceList.Local;
        Gt12Device = list.GetHidDeviceOrNull(GT12_HID.VID, GT12_HID.PID);
        if (Gt12Device == null) return HID_STATUS.DEVICE_NOT_FOUND;
        OutputReportLength = Gt12Device.GetMaxInputReportLength();
        InputReportLength = Gt12Device.GetMaxInputReportLength();
        if (Gt12Device.TryOpen(out hidStream))
        {
            hidStream.ReadTimeout = Timeout.Infinite;
            // Loop reading...
            BeginAsyncRead();
            return HID_STATUS.SUCCESS;
        }
        else
        {
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
            flagReceiveData = true;
            if (hidStream.CanRead) BeginAsyncRead();
        }
        catch
        {
            CloseDevice();
        }
    }

    private void BeginAsyncRead()
    {
        var array = new byte[InputReportLength];
        var asyncResult = hidStream.BeginRead(array, 0, InputReportLength, ReadCompleted, array);
    }

    public HID_STATUS Write(Report r)
    {
        if (hidStream.CanWrite)
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
                CloseDevice();
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
        Gt12Device = null;
        hidStream.Close();
    }
}