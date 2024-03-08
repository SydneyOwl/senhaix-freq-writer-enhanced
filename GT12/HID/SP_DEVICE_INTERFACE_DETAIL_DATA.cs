using System.Runtime.InteropServices;

namespace HID;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
{
    internal int cbSize;

    internal short devicePath;
}