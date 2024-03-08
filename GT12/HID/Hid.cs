using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace HID;

public class Hid
{
    public delegate void DelegateDataReceived(object sender, report e);

    public delegate void DelegateStatusConnected(object sender, EventArgs e);

    public enum HID_RETURN
    {
        SUCCESS,
        NO_DEVICE_CONECTED,
        DEVICE_NOT_FIND,
        DEVICE_OPENED,
        WRITE_FAILD,
        READ_FAILD
    }

    private IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    private const int MAX_USB_DEVICES = 64;

    private bool deviceOpened = false;

    private FileStream hidDevice = null;

    private IntPtr hHubDevice;

    private int outputReportLength;

    private int inputReportLength;

    public DelegateDataReceived DataReceived;

    public DelegateStatusConnected DeviceRemoved;

    public int OutputReportLength => outputReportLength;

    public int InputReportLength => inputReportLength;

    public HID_RETURN OpenDevice(ushort vID, ushort pID, string serial)
    {
        if (!deviceOpened)
        {
            List<string> deviceList = new List<string>();
            GetHidDeviceList(ref deviceList);
            if (deviceList.Count == 0)
            {
                return HID_RETURN.NO_DEVICE_CONECTED;
            }

            for (int i = 0; i < deviceList.Count; i++)
            {
                IntPtr intPtr = CreateFile(deviceList[i], 3221225472u, 0u, 0u, 3u, 1073741824u, 0u);
                if (intPtr != INVALID_HANDLE_VALUE)
                {
                    IntPtr intPtr2 = Marshal.AllocHGlobal(512);
                    HidD_GetAttributes(intPtr, out var attributes);
                    HidD_GetSerialNumberString(intPtr, intPtr2, 512);
                    string text = Marshal.PtrToStringAuto(intPtr2);
                    Marshal.FreeHGlobal(intPtr2);
                    if (attributes.VendorID == vID && attributes.ProductID == pID && text.Contains(serial))
                    {
                        HidD_GetPreparsedData(intPtr, out var PreparsedData);
                        HidP_GetCaps(PreparsedData, out var Capabilities);
                        HidD_FreePreparsedData(PreparsedData);
                        outputReportLength = Capabilities.OutputReportByteLength;
                        inputReportLength = Capabilities.InputReportByteLength;
                        hidDevice = new FileStream(new SafeFileHandle(intPtr, ownsHandle: false), FileAccess.ReadWrite,
                            inputReportLength, isAsync: true);
                        deviceOpened = true;
                        BeginAsyncRead();
                        hHubDevice = intPtr;
                        return HID_RETURN.SUCCESS;
                    }
                }
            }

            return HID_RETURN.DEVICE_NOT_FIND;
        }

        return HID_RETURN.DEVICE_OPENED;
    }

    public void CloseDevice()
    {
        if (deviceOpened)
        {
            deviceOpened = false;
            hidDevice.Close();
            CloseHandle(hHubDevice);
        }
    }

    private void BeginAsyncRead()
    {
        byte[] array = new byte[InputReportLength];
        IAsyncResult asyncResult = hidDevice.BeginRead(array, 0, InputReportLength, ReadCompleted, array);
    }

    private void ReadCompleted(IAsyncResult iResult)
    {
        byte[] array = (byte[])iResult.AsyncState;
        try
        {
            hidDevice.EndRead(iResult);
            byte[] array2 = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array2[i] = array[i];
            }

            report e = new report(array[0], array2);
            OnDataReceived(e);
            if (deviceOpened)
            {
                BeginAsyncRead();
            }
        }
        catch
        {
            EventArgs e2 = new EventArgs();
            OnDeviceRemoved(e2);
            CloseDevice();
        }
    }

    protected virtual void OnDataReceived(report e)
    {
        if (DataReceived != null)
        {
            DataReceived(this, e);
        }
    }

    protected virtual void OnDeviceRemoved(EventArgs e)
    {
        if (DeviceRemoved != null)
        {
            DeviceRemoved(this, e);
        }
    }

    public HID_RETURN Write(report r)
    {
        if (deviceOpened)
        {
            try
            {
                byte[] array = new byte[outputReportLength];
                int num = 0;
                num = ((r.reportBuff.Length >= outputReportLength - 1)
                    ? (outputReportLength - 1)
                    : r.reportBuff.Length);
                for (int i = 0; i < num; i++)
                {
                    array[i] = r.reportBuff[i];
                }

                hidDevice.Write(array, 0, OutputReportLength);
                return HID_RETURN.SUCCESS;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                EventArgs e = new EventArgs();
                OnDeviceRemoved(e);
                CloseDevice();
                return HID_RETURN.NO_DEVICE_CONECTED;
            }
        }

        return HID_RETURN.WRITE_FAILD;
    }

    public static void GetHidDeviceList(ref List<string> deviceList)
    {
        Guid HidGuid = Guid.Empty;
        uint num = 0u;
        deviceList.Clear();
        HidD_GetHidGuid(ref HidGuid);
        IntPtr intPtr = SetupDiGetClassDevs(ref HidGuid, 0u, IntPtr.Zero, (DIGCF)18);
        if (intPtr != IntPtr.Zero)
        {
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData = default(SP_DEVICE_INTERFACE_DATA);
            deviceInterfaceData.cbSize = Marshal.SizeOf((object)deviceInterfaceData);
            for (num = 0u; num < 64; num++)
            {
                if (SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref HidGuid, num, ref deviceInterfaceData))
                {
                    int requiredSize = 0;
                    SetupDiGetDeviceInterfaceDetail(intPtr, ref deviceInterfaceData, IntPtr.Zero, requiredSize,
                        ref requiredSize, null);
                    IntPtr intPtr2 = Marshal.AllocHGlobal(requiredSize);
                    SP_DEVICE_INTERFACE_DETAIL_DATA sP_DEVICE_INTERFACE_DETAIL_DATA =
                        default(SP_DEVICE_INTERFACE_DETAIL_DATA);
                    sP_DEVICE_INTERFACE_DETAIL_DATA.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
                    Marshal.StructureToPtr((object)sP_DEVICE_INTERFACE_DETAIL_DATA, intPtr2, fDeleteOld: false);
                    if (SetupDiGetDeviceInterfaceDetail(intPtr, ref deviceInterfaceData, intPtr2, requiredSize,
                            ref requiredSize, null))
                    {
                        deviceList.Add(Marshal.PtrToStringAuto((IntPtr)((int)intPtr2 + 4)));
                    }

                    Marshal.FreeHGlobal(intPtr2);
                }
            }
        }

        SetupDiDestroyDeviceInfoList(intPtr);
    }

    [DllImport("hid.dll")]
    private static extern void HidD_GetHidGuid(ref Guid HidGuid);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent,
        DIGCF Flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData,
        ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet,
        ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
        int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, out HIDD_ATTRIBUTES attributes);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetSerialNumberString(IntPtr hidDeviceObject, IntPtr buffer, int bufferLength);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, out IntPtr PreparsedData);

    [DllImport("hid.dll")]
    private static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);

    [DllImport("hid.dll")]
    private static extern uint HidP_GetCaps(IntPtr PreparsedData, out HIDP_CAPS Capabilities);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode,
        uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, uint templateFile);

    [DllImport("kernel32.dll")]
    private static extern int CloseHandle(IntPtr hObject);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool ReadFile(IntPtr file, byte[] buffer, uint numberOfBytesToRead,
        out uint numberOfBytesRead, IntPtr lpOverlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool WriteFile(IntPtr file, byte[] buffer, uint numberOfBytesToWrite,
        out uint numberOfBytesWritten, IntPtr lpOverlapped);

    [DllImport("User32.dll", SetLastError = true)]
    private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterDeviceNotification(IntPtr handle);
}