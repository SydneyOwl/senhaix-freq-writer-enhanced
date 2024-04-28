using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class VFOInfos: ObservableObject
{
    [ObservableProperty]
    private int pttid;
    [ObservableProperty]
    private string strVFOARxCtsDcs = "OFF";
    [ObservableProperty]
    private string strVFOATxCtsDcs = "OFF";
    [ObservableProperty]
    private string strVFOBRxCtsDcs = "OFF";
    [ObservableProperty]
    private string strVFOBTxCtsDcs = "OFF";
    [ObservableProperty]
    private int vfoABandwide;
    [ObservableProperty]
    private int vfoABusyLock;
    [ObservableProperty]
    private int vfoADir;
    [ObservableProperty]
    private string vfoAFreq = "440.62500";
    [ObservableProperty]
    private string vfoAOffset = "00.0000";
    [ObservableProperty]
    private int vfoAScram;
    [ObservableProperty]
    private int vfoASignalGroup;
    [ObservableProperty]
    private int vfoASignalSystem;
    [ObservableProperty]
    private int vfoASQMode;
    [ObservableProperty]
    private int vfoAStep;
    [ObservableProperty]
    private int vfoATxPower;
    [ObservableProperty]
    private int vfoBBandwide;
    [ObservableProperty]
    private int vfoBBusyLock;
    [ObservableProperty]
    private int vfoBDir;
    [ObservableProperty]
    private string vfoBFreq = "145.62500";
    [ObservableProperty]
    private string vfoBOffset = "00.0000";
    [ObservableProperty]
    private int vfoBScram;
    [ObservableProperty]
    private int vfoBSignalGroup;
    [ObservableProperty]
    private int vfoBSignalSystem;
    [ObservableProperty]
    private int vfoBSQMode;
    [ObservableProperty]
    private int vfoBStep;
    [ObservableProperty]
    private int vfoBTxPower;
}