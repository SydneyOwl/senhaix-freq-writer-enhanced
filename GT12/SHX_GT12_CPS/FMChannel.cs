using System;

namespace SHX_GT12_CPS;

[Serializable]
public class FMChannel
{
    private int[] channels = new int[15];

    private int curFreq = 904;

    public int[] Channels
    {
        get => channels;
        set => channels = value;
    }

    public int CurFreq
    {
        get => curFreq;
        set => curFreq = value;
    }
}