using System;

namespace SHX_GT12_CPS;

[Serializable]
public class Function
{
    private int alarmMode;

    private int autoLock = 2;

    private int autoPowerOff;

    private int backlight = 5;

    private int beep = 3;

    private int bluetoothAudioGain = 2;

    private int bright = 1;

    private string callSign = "";

    private int cbBMisscall;

    private int chADisType;

    private int chAWorkmode;

    private int chBDisType;

    private int chBWorkmode;

    private int curBank;

    private int dualStandby;

    private int fmEnable;

    private int key1Long = 1;

    private int key1Short;

    private int key2Long = 2;

    private int key2Short = 3;

    private int keyLock;

    private int menuQuitTime = 1;

    private int micGain = 1;

    private int powerOnDisType;

    private int powerUpDisTime = 3;

    private int pttDly = 6;

    private int roger;

    private int rptTailClear = 5;

    private int rptTailDet = 5;

    private int saveMode = 1;

    private int scanMode = 1;

    private int sideTone;
    private int sql = 3;

    private int tailClear = 1;

    private int tone = 2;

    private int tot = 2;

    private int vox = 1;

    private int voxDlyTime = 5;

    private int voxSw;

    public string CallSign
    {
        get => callSign;
        set => callSign = value;
    }

    public int BluetoothAudioGain
    {
        get => bluetoothAudioGain;
        set => bluetoothAudioGain = value;
    }

    public int Sql
    {
        get => sql;
        set => sql = value;
    }

    public int SaveMode
    {
        get => saveMode;
        set => saveMode = value;
    }

    public int Vox
    {
        get => vox;
        set => vox = value;
    }

    public int VoxDlyTime
    {
        get => voxDlyTime;
        set => voxDlyTime = value;
    }

    public int DualStandby
    {
        get => dualStandby;
        set => dualStandby = value;
    }

    public int Tot
    {
        get => tot;
        set => tot = value;
    }

    public int Beep
    {
        get => beep;
        set => beep = value;
    }

    public int SideTone
    {
        get => sideTone;
        set => sideTone = value;
    }

    public int ScanMode
    {
        get => scanMode;
        set => scanMode = value;
    }

    public int PttDly
    {
        get => pttDly;
        set => pttDly = value;
    }

    public int ChADisType
    {
        get => chADisType;
        set => chADisType = value;
    }

    public int ChBDisType
    {
        get => chBDisType;
        set => chBDisType = value;
    }

    public int AutoLock
    {
        get => autoLock;
        set => autoLock = value;
    }

    public int MicGain
    {
        get => micGain;
        set => micGain = value;
    }

    public int AlarmMode
    {
        get => alarmMode;
        set => alarmMode = value;
    }

    public int TailClear
    {
        get => tailClear;
        set => tailClear = value;
    }

    public int RptTailClear
    {
        get => rptTailClear;
        set => rptTailClear = value;
    }

    public int RptTailDet
    {
        get => rptTailDet;
        set => rptTailDet = value;
    }

    public int Roger
    {
        get => roger;
        set => roger = value;
    }

    public int FmEnable
    {
        get => fmEnable;
        set => fmEnable = value;
    }

    public int ChAWorkmode
    {
        get => chAWorkmode;
        set => chAWorkmode = value;
    }

    public int ChBWorkmode
    {
        get => chBWorkmode;
        set => chBWorkmode = value;
    }

    public int KeyLock
    {
        get => keyLock;
        set => keyLock = value;
    }

    public int AutoPowerOff
    {
        get => autoPowerOff;
        set => autoPowerOff = value;
    }

    public int PowerOnDisType
    {
        get => powerOnDisType;
        set => powerOnDisType = value;
    }

    public int Tone
    {
        get => tone;
        set => tone = value;
    }

    public int Backlight
    {
        get => backlight;
        set => backlight = value;
    }

    public int MenuQuitTime
    {
        get => menuQuitTime;
        set => menuQuitTime = value;
    }

    public int Key1Short
    {
        get => key1Short;
        set => key1Short = value;
    }

    public int Key1Long
    {
        get => key1Long;
        set => key1Long = value;
    }

    public int Key2Short
    {
        get => key2Short;
        set => key2Short = value;
    }

    public int Key2Long
    {
        get => key2Long;
        set => key2Long = value;
    }

    public int Bright
    {
        get => bright;
        set => bright = value;
    }

    public int CbBMisscall
    {
        get => cbBMisscall;
        set => cbBMisscall = value;
    }

    public int CurBank
    {
        get => curBank;
        set => curBank = value;
    }

    public int VoxSw
    {
        get => voxSw;
        set => voxSw = value;
    }

    public int PowerUpDisTime
    {
        get => powerUpDisTime;
        set => powerUpDisTime = value;
    }
}