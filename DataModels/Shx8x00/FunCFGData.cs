using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class FunCFGData : ObservableObject
{
    [ObservableProperty] private bool cB_AlarmSound = true;

    [ObservableProperty] private bool cB_AutoLock;

    [ObservableProperty] private bool cB_FMRadioEnable = true;

    [ObservableProperty] private bool cB_LockKeyBoard;

    [ObservableProperty] private bool cB_SoundOfBi = true;

    [ObservableProperty] private bool cB_StopSendOnBusy;

    [ObservableProperty] private bool cB_TDR;

    [ObservableProperty] private int cbB_1750Hz = 2;

    [ObservableProperty] private int cbB_A_Band;

    [ObservableProperty] private int cbB_A_CHBand;

    [ObservableProperty] private int cbB_A_Fhss;

    [ObservableProperty] private int cbB_A_FreqStep = 5;

    [ObservableProperty] private int cbB_A_Power;

    [ObservableProperty] private int cbB_A_RemainDir;

    [ObservableProperty] private string cbB_A_RxQT = "OFF";

    [ObservableProperty] private int cbB_A_SignalingEnCoder;

    [ObservableProperty] private string cbB_A_TxQT = "OFF";

    [ObservableProperty] private int cbB_AlarmMode = 1;

    [ObservableProperty] private int cbB_AutoBackLight = 5;

    [ObservableProperty] private int cbB_B_Band;

    [ObservableProperty] private int cbB_B_CHBand;

    [ObservableProperty] private int cbB_B_Fhss;

    [ObservableProperty] private int cbB_B_FreqStep = 5;

    [ObservableProperty] private int cbB_B_Power;

    [ObservableProperty] private int cbB_B_RemainDir;

    [ObservableProperty] private string cbB_B_RxQT = "OFF";

    [ObservableProperty] private int cbB_B_SignalingEnCoder;

    [ObservableProperty] private string cbB_B_TxQT = "OFF";

    [ObservableProperty] private int cbB_BackgroundColor;

    [ObservableProperty] private int cbB_CH_A_DisplayMode = 1;

    [ObservableProperty] private int cbB_CH_B_DisplayMode = 1;

    [ObservableProperty] private int cbB_DisplayTypeOfPowerUp;

    [ObservableProperty] private int cbB_DTMF = 3;

    [ObservableProperty] private int cbB_KeySide;

    [ObservableProperty] private int cbB_KeySideL = 1;

    [ObservableProperty] private int cbB_Language = 1;

    [ObservableProperty] private int cbB_MicGain = 2;

    [ObservableProperty] private int cbB_PassRepetNoiseClear = 5;

    [ObservableProperty] private int cbB_PassRepetNoiseDetect;

    [ObservableProperty] private int cbB_PowerOnMsg;

    [ObservableProperty] private int cbB_PTTID;

    [ObservableProperty] private int cbB_SaveMode = 1;

    [ObservableProperty] private int cbB_Scan = 1;

    [ObservableProperty] private int cbB_SendIDDelay = 5;

    [ObservableProperty] private int cbB_SoundOfTxEnd;

    [ObservableProperty] private int cbB_SQL = 3;

    [ObservableProperty] private int cbB_TailNoiseClear = 1;

    [ObservableProperty] private int cbB_TimerMenuQuit = 1;

    [ObservableProperty] private int cbB_TOT;

    [ObservableProperty] private int cbB_TxUnderTDRStart;

    [ObservableProperty] private int cbB_VoicSwitch = 1;

    [ObservableProperty] private int cbB_VOX;

    [ObservableProperty] private int cbB_VoxDelay = 5;

    [ObservableProperty] private int cbB_WorkModeA;

    [ObservableProperty] private int cbB_WorkModeB;

    [ObservableProperty] private string tB_A_CurFreq = "146.02500";

    [ObservableProperty] private string tB_A_RangeOfFreq = "136-173.99750";

    [ObservableProperty] private string tB_A_RemainFreq = "00.0000";

    [ObservableProperty] private string tB_B_CurFreq = "440.02500";

    [ObservableProperty] private string tB_B_RangeOfFreq = "400-519.99750";

    [ObservableProperty] private string tB_B_RemainFreq = "00.0000";

    [ObservableProperty] private string tB_CountsOfCH = "128";
}