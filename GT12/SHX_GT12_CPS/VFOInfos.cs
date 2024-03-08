using System;

namespace SHX_GT12_CPS;

[Serializable]
public class VFOInfos
{
    private int pttid;

    private string strVFOARxCtsDcs = "OFF";

    private string strVFOATxCtsDcs = "OFF";

    private string strVFOBRxCtsDcs = "OFF";

    private string strVFOBTxCtsDcs = "OFF";

    private int vfoABandwide;

    private int vfoABusyLock;

    private int vfoADir;
    private string vfoAFreq = "440.62500";

    private string vfoAOffset = "00.0000";

    private int vfoAScram;

    private int vfoASignalGroup;

    private int vfoASignalSystem;

    private int vfoASQMode;

    private int vfoAStep;

    private int vfoATxPower;

    private int vfoBBandwide;

    private int vfoBBusyLock;

    private int vfoBDir;

    private string vfoBFreq = "145.62500";

    private string vfoBOffset = "00.0000";

    private int vfoBScram;

    private int vfoBSignalGroup;

    private int vfoBSignalSystem;

    private int vfoBSQMode;

    private int vfoBStep;

    private int vfoBTxPower;

    public string VfoAFreq
    {
        get => vfoAFreq;
        set => vfoAFreq = value;
    }

    public string StrVFOARxCtsDcs
    {
        get => strVFOARxCtsDcs;
        set => strVFOARxCtsDcs = value;
    }

    public string StrVFOATxCtsDcs
    {
        get => strVFOATxCtsDcs;
        set => strVFOATxCtsDcs = value;
    }

    public int VfoADir
    {
        get => vfoADir;
        set => vfoADir = value;
    }

    public int VfoASignalGroup
    {
        get => vfoASignalGroup;
        set => vfoASignalGroup = value;
    }

    public int VfoATxPower
    {
        get => vfoATxPower;
        set => vfoATxPower = value;
    }

    public int VfoABandwide
    {
        get => vfoABandwide;
        set => vfoABandwide = value;
    }

    public int VfoASQMode
    {
        get => vfoASQMode;
        set => vfoASQMode = value;
    }

    public int VfoAStep
    {
        get => vfoAStep;
        set => vfoAStep = value;
    }

    public string VfoAOffset
    {
        get => vfoAOffset;
        set => vfoAOffset = value;
    }

    public string VfoBFreq
    {
        get => vfoBFreq;
        set => vfoBFreq = value;
    }

    public string StrVFOBRxCtsDcs
    {
        get => strVFOBRxCtsDcs;
        set => strVFOBRxCtsDcs = value;
    }

    public string StrVFOBTxCtsDcs
    {
        get => strVFOBTxCtsDcs;
        set => strVFOBTxCtsDcs = value;
    }

    public int VfoBDir
    {
        get => vfoBDir;
        set => vfoBDir = value;
    }

    public int VfoBSignalGroup
    {
        get => vfoBSignalGroup;
        set => vfoBSignalGroup = value;
    }

    public int VfoBTxPower
    {
        get => vfoBTxPower;
        set => vfoBTxPower = value;
    }

    public int VfoBBandwide
    {
        get => vfoBBandwide;
        set => vfoBBandwide = value;
    }

    public int VfoBSQMode
    {
        get => vfoBSQMode;
        set => vfoBSQMode = value;
    }

    public int VfoBStep
    {
        get => vfoBStep;
        set => vfoBStep = value;
    }

    public string VfoBOffset
    {
        get => vfoBOffset;
        set => vfoBOffset = value;
    }

    public int Pttid
    {
        get => pttid;
        set => pttid = value;
    }

    public int VfoASignalSystem
    {
        get => vfoASignalSystem;
        set => vfoASignalSystem = value;
    }

    public int VfoBSignalSystem
    {
        get => vfoBSignalSystem;
        set => vfoBSignalSystem = value;
    }

    public int VfoABusyLock
    {
        get => vfoABusyLock;
        set => vfoABusyLock = value;
    }

    public int VfoBBusyLock
    {
        get => vfoBBusyLock;
        set => vfoBBusyLock = value;
    }

    public int VfoAScram
    {
        get => vfoAScram;
        set => vfoAScram = value;
    }

    public int VfoBScram
    {
        get => vfoBScram;
        set => vfoBScram = value;
    }
}