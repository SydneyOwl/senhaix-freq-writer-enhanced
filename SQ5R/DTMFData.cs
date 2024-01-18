using System;

namespace SQ5R;

[Serializable]
public class DTMFData
{
    private int groupCall = 14;
    private string groupOfDTMF_1 = "20202";

    private string groupOfDTMF_2 = "";

    private string groupOfDTMF_3 = "";

    private string groupOfDTMF_4 = "";

    private string groupOfDTMF_5 = "";

    private string groupOfDTMF_6 = "";

    private string groupOfDTMF_7 = "";

    private string groupOfDTMF_8 = "";

    private string groupOfDTMF_9 = "";

    private string groupOfDTMF_A = "";

    private string groupOfDTMF_B = "";

    private string groupOfDTMF_C = "";

    private string groupOfDTMF_D = "";

    private string groupOfDTMF_E = "";

    private string groupOfDTMF_F = "30303";

    private int lastTimeSend = 1;

    private int lastTimeStop = 1;

    private bool sendOnPTTPressed;

    private bool sendOnPTTReleased = true;

    private string theIDOfLocalHost = "80808";

    public string GroupOfDTMF_1
    {
        get => groupOfDTMF_1;
        set => groupOfDTMF_1 = value;
    }

    public string GroupOfDTMF_2
    {
        get => groupOfDTMF_2;
        set => groupOfDTMF_2 = value;
    }

    public string GroupOfDTMF_3
    {
        get => groupOfDTMF_3;
        set => groupOfDTMF_3 = value;
    }

    public string GroupOfDTMF_4
    {
        get => groupOfDTMF_4;
        set => groupOfDTMF_4 = value;
    }

    public string GroupOfDTMF_5
    {
        get => groupOfDTMF_5;
        set => groupOfDTMF_5 = value;
    }

    public string GroupOfDTMF_6
    {
        get => groupOfDTMF_6;
        set => groupOfDTMF_6 = value;
    }

    public string GroupOfDTMF_7
    {
        get => groupOfDTMF_7;
        set => groupOfDTMF_7 = value;
    }

    public string GroupOfDTMF_8
    {
        get => groupOfDTMF_8;
        set => groupOfDTMF_8 = value;
    }

    public string GroupOfDTMF_9
    {
        get => groupOfDTMF_9;
        set => groupOfDTMF_9 = value;
    }

    public string GroupOfDTMF_A
    {
        get => groupOfDTMF_A;
        set => groupOfDTMF_A = value;
    }

    public string GroupOfDTMF_B
    {
        get => groupOfDTMF_B;
        set => groupOfDTMF_B = value;
    }

    public string GroupOfDTMF_C
    {
        get => groupOfDTMF_C;
        set => groupOfDTMF_C = value;
    }

    public string GroupOfDTMF_D
    {
        get => groupOfDTMF_D;
        set => groupOfDTMF_D = value;
    }

    public string GroupOfDTMF_E
    {
        get => groupOfDTMF_E;
        set => groupOfDTMF_E = value;
    }

    public string GroupOfDTMF_F
    {
        get => groupOfDTMF_F;
        set => groupOfDTMF_F = value;
    }

    public int LastTimeSend
    {
        get => lastTimeSend;
        set => lastTimeSend = value;
    }

    public int LastTimeStop
    {
        get => lastTimeStop;
        set => lastTimeStop = value;
    }

    public int GroupCall
    {
        get => groupCall;
        set => groupCall = value;
    }

    public string TheIDOfLocalHost
    {
        get => theIDOfLocalHost;
        set => theIDOfLocalHost = value;
    }

    public bool SendOnPTTPressed
    {
        get => sendOnPTTPressed;
        set => sendOnPTTPressed = value;
    }

    public bool SendOnPTTReleased
    {
        get => sendOnPTTReleased;
        set => sendOnPTTReleased = value;
    }
}