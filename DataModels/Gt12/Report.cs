using System;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class Report : EventArgs
{
    public readonly byte[] ReportBuff;
    public readonly byte ReportId;

    public Report(byte id, byte[] arrayBuff)
    {
        ReportId = id;
        ReportBuff = arrayBuff;
    }
}