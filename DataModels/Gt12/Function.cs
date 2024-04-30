using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Function : ObservableObject
{
    [ObservableProperty] private int alarmMode;
    [ObservableProperty] private int autoLock = 2;
    [ObservableProperty] private int autoPowerOff;
    [ObservableProperty] private int backlight = 5;
    [ObservableProperty] private int beep = 3;
    [ObservableProperty] private int bluetoothAudioGain = 2;
    [ObservableProperty] private int bright = 1;
    [ObservableProperty] private string callSign = "";
    [ObservableProperty] private int cbBMisscall;
    [ObservableProperty] private int chADisType;
    [ObservableProperty] private int chAWorkmode;
    [ObservableProperty] private int chBDisType;
    [ObservableProperty] private int chBWorkmode;
    [ObservableProperty] private int curBank;
    [ObservableProperty] private int dualStandby;
    [ObservableProperty] private int fmEnable;
    [ObservableProperty] private int key1Long = 1;
    [ObservableProperty] private int key1Short;
    [ObservableProperty] private int key2Long = 2;
    [ObservableProperty] private int key2Short = 3;
    [ObservableProperty] private int keyLock;
    [ObservableProperty] private int menuQuitTime = 1;
    [ObservableProperty] private int micGain = 1;
    [ObservableProperty] private int powerOnDisType;
    [ObservableProperty] private int powerUpDisTime = 3;
    [ObservableProperty] private int pttDly = 6;
    [ObservableProperty] private int roger;
    [ObservableProperty] private int rptTailClear = 5;
    [ObservableProperty] private int rptTailDet = 5;
    [ObservableProperty] private int saveMode = 1;
    [ObservableProperty] private int scanMode = 1;
    [ObservableProperty] private int sideTone;
    [ObservableProperty] private int sql = 3;
    [ObservableProperty] private int tailClear = 1;
    [ObservableProperty] private int tone = 2;
    [ObservableProperty] private int tot = 2;
    [ObservableProperty] private int vox = 1;
    [ObservableProperty] private int voxDlyTime = 5;
    [ObservableProperty] private int voxSw;
}