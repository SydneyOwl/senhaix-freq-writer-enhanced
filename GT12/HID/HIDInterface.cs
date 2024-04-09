using System;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace HID;

public class HIDInterface : IDisposable
{
    public delegate void DelegateDataReceived(object sender, byte[] data);

    public delegate void DelegateStatusConnected(object sender, bool isConnect);

    public enum MessagesType
    {
        Message,
        Error
    }

    private static HIDInterface m_oInstance;

    public bool bConnected;

    private bool ContinueConnectFlag = true;

    public DelegateDataReceived DataReceived;

    private HidDevice lowHidDevice = default;

    public Hid oSp = new();

    private readonly BackgroundWorker ReadWriteThread = new();

    public DelegateStatusConnected StatusConnected;

    public HIDInterface()
    {
        m_oInstance = this;
        oSp.DataReceived = HidDataReceived;
        oSp.DeviceRemoved = HidDeviceRemoved;
    }

    public void Dispose()
    {
        try
        {
            DisConnect();
            var hid = oSp;
            hid.DataReceived =
                (Hid.DelegateDataReceived)Delegate.Remove(hid.DataReceived,
                    new Hid.DelegateDataReceived(HidDataReceived));
            var hid2 = oSp;
            hid2.DeviceRemoved = (Hid.DelegateStatusConnected)Delegate.Remove(hid2.DeviceRemoved,
                new Hid.DelegateStatusConnected(HidDeviceRemoved));
            ReadWriteThread.DoWork -= ReadWriteThread_DoWork;
            ReadWriteThread.CancelAsync();
            ReadWriteThread.Dispose();
        }
        catch
        {
        }
    }

    protected virtual void RaiseEventConnectedState(bool isConnect)
    {
        if (StatusConnected != null) StatusConnected(this, isConnect);
    }

    protected virtual void RaiseEventDataReceived(byte[] buf)
    {
        if (DataReceived != null) DataReceived(this, buf);
    }

    public void AutoConnect(HidDevice hidDevice)
    {
        lowHidDevice = hidDevice;
        ContinueConnectFlag = true;
        ReadWriteThread.DoWork += ReadWriteThread_DoWork;
        ReadWriteThread.WorkerSupportsCancellation = true;
        ReadWriteThread.RunWorkerAsync();
    }

    public void StopAutoConnect()
    {
        try
        {
            ContinueConnectFlag = false;
            Dispose();
        }
        catch
        {
        }
    }

    ~HIDInterface()
    {
        Dispose();
    }

    public bool Connect(HidDevice hidDevice)
    {
        var reusltString = default(ReusltString);
        if (oSp.OpenDevice(hidDevice.VID, hidDevice.PID, hidDevice.SERIAL) == Hid.HID_RETURN.SUCCESS)
        {
            bConnected = true;
            reusltString.Result = true;
            reusltString.message = "Connect Success!";
            RaiseEventConnectedState(reusltString.Result);
            return true;
        }

        bConnected = false;
        reusltString.Result = false;
        reusltString.message = "Device Connect Error";
        RaiseEventConnectedState(reusltString.Result);
        return false;
    }

    public bool Send(byte[] byData)
    {
        var array = new byte[byData.Length];
        Array.Copy(byData, 0, array, 0, byData.Length);
        if (oSp.Write(new report(1, array)) != 0) return false;

        return true;
    }

    public bool Send(string strData)
    {
        var bytes = Encoding.Unicode.GetBytes(strData);
        return Send(bytes);
    }

    public void DisConnect()
    {
        bConnected = false;
        Thread.Sleep(200);
        if (oSp != null) oSp.CloseDevice();
    }

    public void HidDeviceRemoved(object sender, EventArgs e)
    {
        bConnected = false;
        var reusltString = default(ReusltString);
        reusltString.Result = false;
        reusltString.message = "Device Remove";
        RaiseEventConnectedState(reusltString.Result);
        if (oSp != null) oSp.CloseDevice();
    }

    public void HidDataReceived(object sender, report e)
    {
        try
        {
            var array = new byte[64];
            Array.Copy(e.reportBuff, 0, array, 0, 64);
            RaiseEventDataReceived(array);
        }
        catch
        {
            var reusltString = default(ReusltString);
            reusltString.Result = false;
            reusltString.message = "Receive Error";
            RaiseEventConnectedState(reusltString.Result);
        }
    }

    private void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
    {
        while (ContinueConnectFlag)
            try
            {
                if (!bConnected) Connect(lowHidDevice);

                Thread.Sleep(500);
            }
            catch
            {
            }
    }

    public struct ReusltString
    {
        public bool Result;

        public string message;
    }

    public struct HidDevice
    {
        public ushort VID;

        public ushort PID;

        public string SERIAL;
    }

    public struct TagInfo
    {
        public string AntennaPort;

        public string EPC;
    }
}