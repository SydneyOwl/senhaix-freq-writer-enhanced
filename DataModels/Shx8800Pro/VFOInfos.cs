using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class VfoInfos : ObservableObject
{
    [ObservableProperty] private int _pttid;//
    [ObservableProperty] private string _strVfoaRxCtsDcs = "OFF";//
    [ObservableProperty] private string _strVfoaTxCtsDcs = "OFF";//
    [ObservableProperty] private string _strVfobRxCtsDcs = "OFF";//
    [ObservableProperty] private string _strVfobTxCtsDcs = "OFF";//
    [ObservableProperty] private int _vfoABandwide;//
    [ObservableProperty] private int _vfoABusyLock;//
    [ObservableProperty] private int _vfoADir;//
    [ObservableProperty] private string _vfoAFreq = "440.62500";//
    [ObservableProperty] private string _vfoAOffset = "00.0000";//
    [ObservableProperty] private int _vfoAScram;//
    [ObservableProperty] private int _vfoASignalGroup;//
    [ObservableProperty] private int _vfoASignalSystem;//
    [ObservableProperty] private int _vfoAsqMode;//
    [ObservableProperty] private int _vfoAStep;//
    [ObservableProperty] private int _vfoATxPower;//
    [ObservableProperty] private int _vfoBBandwide;//
    [ObservableProperty] private int _vfoBBusyLock;//
    [ObservableProperty] private int _vfoBDir;//
    [ObservableProperty] private string _vfoBFreq = "145.62500";//
    [ObservableProperty] private string _vfoBOffset = "00.0000";//
    [ObservableProperty] private int _vfoBScram;//
    [ObservableProperty] private int _vfoBSignalGroup;//
    [ObservableProperty] private int _vfoBSignalSystem;//
    [ObservableProperty] private int _vfoBsqMode;//
    [ObservableProperty] private int _vfoBStep;//
    [ObservableProperty] private int _vfoBTxPower;//
}