using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Function : ObservableObject
{
    [ObservableProperty] private int _alarmMode;
    [ObservableProperty] private int _autoLock = 2;
    [ObservableProperty] private int _autoPowerOff;
    [ObservableProperty] private int _backlight = 5;
    [ObservableProperty] private int _beep = 3;
    [ObservableProperty] private int _bluetoothAudioGain = 2;
    [ObservableProperty] private int _bright = 1;
    [ObservableProperty] private string _callSign = "";
    [ObservableProperty] private int _cbBMisscall;
    [ObservableProperty] private int _chADisType;
    [ObservableProperty] private int _chAWorkmode;
    [ObservableProperty] private int _chBDisType;
    [ObservableProperty] private int _chBWorkmode;
    [ObservableProperty] private int _curBank;
    [ObservableProperty] private int _dualStandby;
    [ObservableProperty] private int _fmEnable;
    [ObservableProperty] private int _key1Long = 1;
    [ObservableProperty] private int _key1Short;
    [ObservableProperty] private int _key2Long = 2;
    [ObservableProperty] private int _key2Short = 3;
    [ObservableProperty] private int _keyLock;
    [ObservableProperty] private int _menuQuitTime = 1;
    [ObservableProperty] private int _micGain = 1;
    [ObservableProperty] private int _powerOnDisType;
    [ObservableProperty] private int _powerUpDisTime = 3;
    [ObservableProperty] private int _pttDly = 6;
    [ObservableProperty] private int _roger;
    [ObservableProperty] private int _rptTailClear = 5;
    [ObservableProperty] private int _rptTailDet = 5;
    [ObservableProperty] private int _saveMode = 1;
    [ObservableProperty] private int _scanMode = 1;
    [ObservableProperty] private int _sideTone;
    [ObservableProperty] private int _sql = 3;
    [ObservableProperty] private int _tailClear = 1;
    [ObservableProperty] private int _tone = 2;
    [ObservableProperty] private int _tot = 2;
    [ObservableProperty] private int _vox = 1;
    [ObservableProperty] private int _voxDlyTime = 5;
    [ObservableProperty] private int _voxSw;
}