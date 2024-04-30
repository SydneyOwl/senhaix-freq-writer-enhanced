using System;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class Report : EventArgs
{
        public readonly byte[] reportBuff;
        public readonly byte reportID;

        public Report(byte id, byte[] arrayBuff)
        {
            reportID = id;
            reportBuff = arrayBuff;
        }
}