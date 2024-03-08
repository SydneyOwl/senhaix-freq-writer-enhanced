using System;

namespace SHX_GT12_CPS;

[Serializable]
public class Channel
{
    private int bandwide;
    private int id;

    private string name = "";

    private int pttid;

    private string rxFreq = "";

    private int scanAdd;

    private int signalGroup;

    private int signalSystem;

    private int sqMode;

    private string strRxCtsDcs = "OFF";

    private string strTxCtsDcs = "OFF";

    private string txFreq = "";

    private int txPower;

    public Channel()
    {
    }

    public Channel(int id, string rxFreq, string rxCts, string txFreq, string txCts, int power, int bandwide,
        int scanAdd, int sqMode, int pttid, int signal, string name)
    {
        this.id = id;
        this.rxFreq = rxFreq;
        strRxCtsDcs = rxCts;
        this.txFreq = txFreq;
        strTxCtsDcs = txCts;
        txPower = power;
        this.bandwide = bandwide;
        this.scanAdd = scanAdd;
        this.sqMode = sqMode;
        this.pttid = pttid;
        signalGroup = signal;
        this.name = name;
    }

    public int Id
    {
        get => id;
        set => id = value;
    }

    public string RxFreq
    {
        get => rxFreq;
        set => rxFreq = value;
    }

    public string StrRxCtsDcs
    {
        get => strRxCtsDcs;
        set => strRxCtsDcs = value;
    }

    public string TxFreq
    {
        get => txFreq;
        set => txFreq = value;
    }

    public string StrTxCtsDcs
    {
        get => strTxCtsDcs;
        set => strTxCtsDcs = value;
    }

    public int TxPower
    {
        get => txPower;
        set => txPower = value;
    }

    public int Bandwide
    {
        get => bandwide;
        set => bandwide = value;
    }

    public int ScanAdd
    {
        get => scanAdd;
        set => scanAdd = value;
    }

    public int SqMode
    {
        get => sqMode;
        set => sqMode = value;
    }

    public int Pttid
    {
        get => pttid;
        set => pttid = value;
    }

    public int SignalGroup
    {
        get => signalGroup;
        set => signalGroup = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public int SignalSystem
    {
        get => signalSystem;
        set => signalSystem = value;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}