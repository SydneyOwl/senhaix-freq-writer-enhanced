using System;
using System.Runtime.InteropServices;

namespace HID;

public struct DEV_BROADCAST_DEVICEINTERFACE
{
    public int dbcc_size;

    public int dbcc_devicetype;

    public int dbcc_reserved;

    public Guid dbcc_classguid;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
    public string dbcc_name;
}