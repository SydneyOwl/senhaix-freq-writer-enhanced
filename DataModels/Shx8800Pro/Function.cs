using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class Function : ObservableObject
{
    [ObservableProperty] private int _alarmMode; //
    [ObservableProperty] private int _autoLock = 2; //
    [ObservableProperty] private int _backlight = 5; //
    [ObservableProperty] private int _beep = 1; //
    [ObservableProperty] private int _bluetoothAudioGain = 2; //
    [ObservableProperty] private int _btMicGain = 2; //
    [ObservableProperty] private string _callSign = ""; //
    [ObservableProperty] private int _chADisType; //
    [ObservableProperty] private int _chAWorkmode; //
    [ObservableProperty] private int _chBDisType; //
    [ObservableProperty] private int _chBWorkmode; //
    [ObservableProperty] private int _curBankA; //
    [ObservableProperty] private int _curBankB; //
    [ObservableProperty] private int _dualStandby; //
    [ObservableProperty] private int _fmEnable; //
    [ObservableProperty] private int _key2Long = 1; //
    [ObservableProperty] private int _key2Short; //
    [ObservableProperty] private int _keyLock; //
    [ObservableProperty] private int _localSosTone = 1; //
    [ObservableProperty] private int _menuQuitTime = 1; //
    [ObservableProperty] private int _micGain = 1; //
    [ObservableProperty] private int _powerOnDisType; //
    [ObservableProperty] private int _pwrOnDlyTime; //
    [ObservableProperty] private int _pttDly = 4; //
    [ObservableProperty] private int _roger; //
    [ObservableProperty] private int _rptTailClear = 5; //
    [ObservableProperty] private int _rptTailDet = 5; //
    [ObservableProperty] private int _saveMode = 1; //
    [ObservableProperty] private int _scanMode = 1; //
    [ObservableProperty] private int _sideTone; //
    [ObservableProperty] private int _sql = 3; //
    [ObservableProperty] private int _tailClear = 1; //
    [ObservableProperty] private int _tone = 2; //
    [ObservableProperty] private int _tot = 2; //
    [ObservableProperty] private int _voiceSw = 1; //
    [ObservableProperty] private int _vox = 1; //
    [ObservableProperty] private int _voxDlyTime = 5; //
    [ObservableProperty] private int _voxSw; //
}