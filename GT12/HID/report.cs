using System;

namespace HID;

public class report : EventArgs
{
    public readonly byte[] reportBuff;
    public readonly byte reportID;

    public report(byte id, byte[] arrayBuff)
    {
        reportID = id;
        reportBuff = arrayBuff;
    }
}