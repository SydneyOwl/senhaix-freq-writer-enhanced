using System;

namespace HID;

public struct SP_DEVICE_INTERFACE_DATA
{
    public int cbSize;

    public Guid interfaceClassGuid;

    public int flags;

    public int reserved;
}