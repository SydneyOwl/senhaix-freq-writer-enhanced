using System;

namespace SQ5R;

[Serializable]
public class FormFunCFGData
{
    private bool cB_AlarmSound = true;

    private bool cB_AutoLock;

    private bool cB_FMRadioEnable = true;

    private bool cB_LockKeyBoard;

    private bool cB_SoundOfBi = true;

    private bool cB_StopSendOnBusy;

    private bool cB_TDR;

    private int cbB_1750Hz = 2;

    private int cbB_A_Band;

    private int cbB_A_CHBand;

    private int cbB_A_Fhss;

    private int cbB_A_FreqStep = 5;

    private int cbB_A_Power;

    private int cbB_A_RemainDir;

    private string cbB_A_RxQT = "OFF";

    private int cbB_A_SignalingEnCoder;

    private string cbB_A_TxQT = "OFF";

    private int cbB_AlarmMode = 1;
    private int cbB_AutoBackLight = 5;

    private int cbB_B_Band = 1;

    private int cbB_B_CHBand;

    private int cbB_B_Fhss;

    private int cbB_B_FreqStep = 5;

    private int cbB_B_Power;

    private int cbB_B_RemainDir;

    private string cbB_B_RxQT = "OFF";

    private int cbB_B_SignalingEnCoder;

    private string cbB_B_TxQT = "OFF";

    private int cbB_BackgroundColor;

    private int cbB_CH_A_DisplayMode = 1;

    private int cbB_CH_B_DisplayMode = 1;

    private int cbB_DisplayTypeOfPowerUp;

    private int cbB_DTMF = 3;

    private int cbB_KeySide;

    private int cbB_KeySideL = 1;

    private int cbB_Language = 1;

    private int cbB_MicGain = 2;

    private int cbB_PassRepetNoiseClear = 5;

    private int cbB_PassRepetNoiseDetect;

    private int cbB_PowerOnMsg;

    private int cbB_PTTID;

    private int cbB_SaveMode = 1;

    private int cbB_Scan = 1;

    private int cbB_SendIDDelay = 5;

    private int cbB_SoundOfTxEnd;

    private int cbB_SQL = 3;

    private int cbB_TailNoiseClear = 1;

    private int cbB_TimerMenuQuit = 1;

    private int cbB_TOT;

    private int cbB_TxUnderTDRStart;

    private int cbB_VoicSwitch = 1;

    private int cbB_VOX;

    private int cbB_VoxDelay = 5;

    private int cbB_WorkModeA;

    private int cbB_WorkModeB;

    private string tB_A_CurFreq = "146.02500";

    private string tB_A_RangeOfFreq = "136-173.99750";

    private string tB_A_RemainFreq = "00.0000";

    private string tB_B_CurFreq = "440.02500";

    private string tB_B_RangeOfFreq = "400-519.99750";

    private string tB_B_RemainFreq = "00.0000";

    private string tB_CountsOfCH = "128";

    public int CbB_AutoBackLight
    {
        get => cbB_AutoBackLight;
        set => cbB_AutoBackLight = value;
    }

    public int CbB_VOX
    {
        get => cbB_VOX;
        set => cbB_VOX = value;
    }

    public int CbB_SQL
    {
        get => cbB_SQL;
        set => cbB_SQL = value;
    }

    public int CbB_TOT
    {
        get => cbB_TOT;
        set => cbB_TOT = value;
    }

    public string TB_CountsOfCH
    {
        get => tB_CountsOfCH;
        set => tB_CountsOfCH = value;
    }

    public int CbB_CH_B_DisplayMode
    {
        get => cbB_CH_B_DisplayMode;
        set => cbB_CH_B_DisplayMode = value;
    }

    public int CbB_CH_A_DisplayMode
    {
        get => cbB_CH_A_DisplayMode;
        set => cbB_CH_A_DisplayMode = value;
    }

    public int CbB_SendIDDelay
    {
        get => cbB_SendIDDelay;
        set => cbB_SendIDDelay = value;
    }

    public int CbB_PTTID
    {
        get => cbB_PTTID;
        set => cbB_PTTID = value;
    }

    public int CbB_Scan
    {
        get => cbB_Scan;
        set => cbB_Scan = value;
    }

    public int CbB_SaveMode
    {
        get => cbB_SaveMode;
        set => cbB_SaveMode = value;
    }

    public int CbB_DTMF
    {
        get => cbB_DTMF;
        set => cbB_DTMF = value;
    }

    public bool CB_SoundOfBi
    {
        get => cB_SoundOfBi;
        set => cB_SoundOfBi = value;
    }

    public bool CB_StopSendOnBusy
    {
        get => cB_StopSendOnBusy;
        set => cB_StopSendOnBusy = value;
    }

    public bool CB_AutoLock
    {
        get => cB_AutoLock;
        set => cB_AutoLock = value;
    }

    public bool CB_LockKeyBoard
    {
        get => cB_LockKeyBoard;
        set => cB_LockKeyBoard = value;
    }

    public string TB_A_RangeOfFreq
    {
        get => tB_A_RangeOfFreq;
        set => tB_A_RangeOfFreq = value;
    }

    public string TB_A_CurFreq
    {
        get => tB_A_CurFreq;
        set => tB_A_CurFreq = value;
    }

    public int CbB_A_SignalingEnCoder
    {
        get => cbB_A_SignalingEnCoder;
        set => cbB_A_SignalingEnCoder = value;
    }

    public int CbB_A_RemainDir
    {
        get => cbB_A_RemainDir;
        set => cbB_A_RemainDir = value;
    }

    public int CbB_A_FreqStep
    {
        get => cbB_A_FreqStep;
        set => cbB_A_FreqStep = value;
    }

    public int CbB_A_CHBand
    {
        get => cbB_A_CHBand;
        set => cbB_A_CHBand = value;
    }

    public string CbB_A_TxQT
    {
        get => cbB_A_TxQT;
        set => cbB_A_TxQT = value;
    }

    public string CbB_A_RxQT
    {
        get => cbB_A_RxQT;
        set => cbB_A_RxQT = value;
    }

    public int CbB_A_Power
    {
        get => cbB_A_Power;
        set => cbB_A_Power = value;
    }

    public int CbB_A_Band
    {
        get => cbB_A_Band;
        set => cbB_A_Band = value;
    }

    public string TB_B_RangeOfFreq
    {
        get => tB_B_RangeOfFreq;
        set => tB_B_RangeOfFreq = value;
    }

    public string TB_B_CurFreq
    {
        get => tB_B_CurFreq;
        set => tB_B_CurFreq = value;
    }

    public int CbB_B_SignalingEnCoder
    {
        get => cbB_B_SignalingEnCoder;
        set => cbB_B_SignalingEnCoder = value;
    }

    public int CbB_B_RemainDir
    {
        get => cbB_B_RemainDir;
        set => cbB_B_RemainDir = value;
    }

    public int CbB_B_FreqStep
    {
        get => cbB_B_FreqStep;
        set => cbB_B_FreqStep = value;
    }

    public int CbB_B_CHBand
    {
        get => cbB_B_CHBand;
        set => cbB_B_CHBand = value;
    }

    public string CbB_B_TxQT
    {
        get => cbB_B_TxQT;
        set => cbB_B_TxQT = value;
    }

    public string CbB_B_RxQT
    {
        get => cbB_B_RxQT;
        set => cbB_B_RxQT = value;
    }

    public int CbB_B_Power
    {
        get => cbB_B_Power;
        set => cbB_B_Power = value;
    }

    public int CbB_B_Band
    {
        get => cbB_B_Band;
        set => cbB_B_Band = value;
    }

    public int CbB_DisplayTypeOfPowerUp
    {
        get => cbB_DisplayTypeOfPowerUp;
        set => cbB_DisplayTypeOfPowerUp = value;
    }

    public int CbB_PassRepetNoiseDetect
    {
        get => cbB_PassRepetNoiseDetect;
        set => cbB_PassRepetNoiseDetect = value;
    }

    public int CbB_PassRepetNoiseClear
    {
        get => cbB_PassRepetNoiseClear;
        set => cbB_PassRepetNoiseClear = value;
    }

    public int CbB_TailNoiseClear
    {
        get => cbB_TailNoiseClear;
        set => cbB_TailNoiseClear = value;
    }

    public int CbB_TxUnderTDRStart
    {
        get => cbB_TxUnderTDRStart;
        set => cbB_TxUnderTDRStart = value;
    }

    public int CbB_SoundOfTxEnd
    {
        get => cbB_SoundOfTxEnd;
        set => cbB_SoundOfTxEnd = value;
    }

    public int CbB_AlarmMode
    {
        get => cbB_AlarmMode;
        set => cbB_AlarmMode = value;
    }

    public bool CB_AlarmSound
    {
        get => cB_AlarmSound;
        set => cB_AlarmSound = value;
    }

    public bool CB_FMRadioEnable
    {
        get => cB_FMRadioEnable;
        set => cB_FMRadioEnable = value;
    }

    public bool CB_TDR
    {
        get => cB_TDR;
        set => cB_TDR = value;
    }

    public int CbB_VoicSwitch
    {
        get => cbB_VoicSwitch;
        set => cbB_VoicSwitch = value;
    }

    public int CbB_Language
    {
        get => cbB_Language;
        set => cbB_Language = value;
    }

    public int CbB_KeySide
    {
        get => cbB_KeySide;
        set => cbB_KeySide = value;
    }

    public int CbB_KeySideL
    {
        get => cbB_KeySideL;
        set => cbB_KeySideL = value;
    }

    public int CbB_BackgroundColor
    {
        get => cbB_BackgroundColor;
        set => cbB_BackgroundColor = value;
    }

    public int CbB_PowerOnMsg
    {
        get => cbB_PowerOnMsg;
        set => cbB_PowerOnMsg = value;
    }

    public int CbB_VoxDelay
    {
        get => cbB_VoxDelay;
        set => cbB_VoxDelay = value;
    }

    public int CbB_TimerMenuQuit
    {
        get => cbB_TimerMenuQuit;
        set => cbB_TimerMenuQuit = value;
    }

    public int CbB_MicGain
    {
        get => cbB_MicGain;
        set => cbB_MicGain = value;
    }

    public int CbB_A_Fhss
    {
        get => cbB_A_Fhss;
        set => cbB_A_Fhss = value;
    }

    public int CbB_B_Fhss
    {
        get => cbB_B_Fhss;
        set => cbB_B_Fhss = value;
    }

    public string TB_B_RemainFreq
    {
        get => tB_B_RemainFreq;
        set => tB_B_RemainFreq = value;
    }

    public string TB_A_RemainFreq
    {
        get => tB_A_RemainFreq;
        set => tB_A_RemainFreq = value;
    }

    public int CbB_WorkModeA
    {
        get => cbB_WorkModeA;
        set => cbB_WorkModeA = value;
    }

    public int CbB_WorkModeB
    {
        get => cbB_WorkModeB;
        set => cbB_WorkModeB = value;
    }

    public int CbB_1750Hz
    {
        get => cbB_1750Hz;
        set => cbB_1750Hz = value;
    }
}