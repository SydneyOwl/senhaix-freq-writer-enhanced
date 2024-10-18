using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class FunCfgData : ObservableObject
{
    [ObservableProperty] private bool _cBAlarmSound = true;

    [ObservableProperty] private bool _cBAutoLock;

    [ObservableProperty] private int _cbB1750Hz = 2;

    [ObservableProperty] private int _cbBaBand;

    [ObservableProperty] private int _cbBaChBand;

    [ObservableProperty] private int _cbBaFhss;

    [ObservableProperty] private int _cbBaFreqStep = 5;

    [ObservableProperty] private int _cbBAlarmMode = 1;

    [ObservableProperty] private int _cbBaPower;

    [ObservableProperty] private int _cbBaRemainDir;

    [ObservableProperty] private string _cbBaRxQt = "OFF";

    [ObservableProperty] private int _cbBaSignalingEnCoder;

    [ObservableProperty] private string _cbBaTxQt = "OFF";

    [ObservableProperty] private int _cbBAutoBackLight = 5;

    [ObservableProperty] private int _cbBBackgroundColor;

    [ObservableProperty] private int _cbBbBand;

    [ObservableProperty] private int _cbBbChBand;

    [ObservableProperty] private int _cbBbFhss;

    [ObservableProperty] private int _cbBbFreqStep = 5;

    [ObservableProperty] private int _cbBbPower;

    [ObservableProperty] private int _cbBbRemainDir;

    [ObservableProperty] private string _cbBbRxQt = "OFF";

    [ObservableProperty] private int _cbBbSignalingEnCoder;

    [ObservableProperty] private string _cbBbTxQt = "OFF";

    [ObservableProperty] private int _cbBChADisplayMode = 1;

    [ObservableProperty] private int _cbBChBDisplayMode = 1;

    [ObservableProperty] private int _cbBDisplayTypeOfPowerUp;

    [ObservableProperty] private int _cbBDtmf = 3;

    [ObservableProperty] private int _cbBKeySide;

    [ObservableProperty] private int _cbBKeySideL = 1;

    [ObservableProperty] private int _cbBLanguage = 1;

    [ObservableProperty] private int _cbBMicGain = 2;

    [ObservableProperty] private int _cbBPassRptNoiseClear = 5;

    [ObservableProperty] private int _cbBPassRptNoiseDetect;

    [ObservableProperty] private int _cbBPowerOnMsg;

    [ObservableProperty] private int _cbBPttid;

    [ObservableProperty] private int _cbBSaveMode = 1;

    [ObservableProperty] private int _cbBScan = 1;

    [ObservableProperty] private int _cbBSendIdDelay = 5;

    [ObservableProperty] private int _cbBSoundOfTxEnd;

    [ObservableProperty] private int _cbBSql = 3;

    [ObservableProperty] private int _cbBTailNoiseClear = 1;

    [ObservableProperty] private int _cbBTimerMenuQuit = 1;

    [ObservableProperty] private int _cbBTot;

    [ObservableProperty] private int _cbBTxUnderTdrStart;

    [ObservableProperty] private int _cbBVoicSwitch = 1;

    [ObservableProperty] private int _cbBVox;

    [ObservableProperty] private int _cbBVoxDelay = 5;

    [ObservableProperty] private int _cbBWorkModeA;

    [ObservableProperty] private int _cbBWorkModeB;

    [ObservableProperty] private bool _cBFmRadioEnable = true;

    [ObservableProperty] private bool _cBLockKeyBoard;

    [ObservableProperty] private bool _cBSoundOfBi = true;

    [ObservableProperty] private bool _cBStopSendOnBusy;

    [ObservableProperty] private bool _cBTdr;

    [ObservableProperty] private string _tBaCurFreq = "146.02500";

    [ObservableProperty] private string _tBaRangeOfFreq = "136-173.99750";

    [ObservableProperty] private string _tBaRemainFreq = "00.0000";

    [ObservableProperty] private string _tBbCurFreq = "440.02500";

    [ObservableProperty] private string _tBbRangeOfFreq = "400-519.99750";

    [ObservableProperty] private string _tBbRemainFreq = "00.0000";

    [ObservableProperty] private string _tBCountsOfCh = "128";
}