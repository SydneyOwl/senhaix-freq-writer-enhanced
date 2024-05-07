using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class FunCfgData : ObservableObject
{
    [ObservableProperty] private bool _cBAlarmSound = true;

    [ObservableProperty] private bool _cBAutoLock;

    [ObservableProperty] private bool _cBFmRadioEnable = true;

    [ObservableProperty] private bool _cBLockKeyBoard;

    [ObservableProperty] private bool _cBSoundOfBi = true;

    [ObservableProperty] private bool _cBStopSendOnBusy;

    [ObservableProperty] private bool _cBTdr;

    [ObservableProperty] private int _cbB1750Hz = 2;

    [ObservableProperty] private int _cbBABand;

    [ObservableProperty] private int _cbBAChBand;

    [ObservableProperty] private int _cbBAFhss;

    [ObservableProperty] private int _cbBAFreqStep = 5;

    [ObservableProperty] private int _cbBAPower;

    [ObservableProperty] private int _cbBARemainDir;

    [ObservableProperty] private string _cbBARxQt = "OFF";

    [ObservableProperty] private int _cbBASignalingEnCoder;

    [ObservableProperty] private string _cbBATxQt = "OFF";

    [ObservableProperty] private int _cbBAlarmMode = 1;

    [ObservableProperty] private int _cbBAutoBackLight = 5;

    [ObservableProperty] private int _cbBBBand;

    [ObservableProperty] private int _cbBBChBand;

    [ObservableProperty] private int _cbBBFhss;

    [ObservableProperty] private int _cbBBFreqStep = 5;

    [ObservableProperty] private int _cbBBPower;

    [ObservableProperty] private int _cbBBRemainDir;

    [ObservableProperty] private string _cbBBRxQt = "OFF";

    [ObservableProperty] private int _cbBBSignalingEnCoder;

    [ObservableProperty] private string _cbBBTxQt = "OFF";

    [ObservableProperty] private int _cbBBackgroundColor;

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

    [ObservableProperty] private string _tBACurFreq = "146.02500";

    [ObservableProperty] private string _tBARangeOfFreq = "136-173.99750";

    [ObservableProperty] private string _tBARemainFreq = "00.0000";

    [ObservableProperty] private string _tBBCurFreq = "440.02500";

    [ObservableProperty] private string _tBBRangeOfFreq = "400-519.99750";

    [ObservableProperty] private string _tBBRemainFreq = "00.0000";

    [ObservableProperty] private string _tBCountsOfCh = "128";
}