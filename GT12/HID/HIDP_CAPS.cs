using System.Runtime.InteropServices;

namespace HID;

public struct HIDP_CAPS
{
    public ushort Usage;

    public ushort UsagePage;

    public ushort InputReportByteLength;

    public ushort OutputReportByteLength;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
    public ushort[] Reserved;

    public ushort NumberLinkCollectionNodes;

    public ushort NumberInputButtonCaps;

    public ushort NumberInputValueCaps;

    public ushort NumberInputDataIndices;

    public ushort NumberOutputButtonCaps;

    public ushort NumberOutputValueCaps;

    public ushort NumberOutputDataIndices;

    public ushort NumberFeatureButtonCaps;

    public ushort NumberFeatureValueCaps;

    public ushort NumberFeatureDataIndices;
}