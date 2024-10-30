using System.Collections.ObjectModel;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Constants.Gt12;

public class FunctionChoice
{
    public static ObservableCollection<string> Sql = new()
    {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
    };

    public static ObservableCollection<string> TxTimeout = new()
    {
        Language.GetString("off"), "30s", "60s", "90s", "120s", "150s", "180s", "210s", "240s"
    };

    public static ObservableCollection<string> SaveMode = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Vox = new()
    {
        Language.GetString("high_sensitivity"), Language.GetString("mid_sensitivity"),
        Language.GetString("low_sensitivity")
        // "高灵敏度", "中灵敏度", "低灵敏度"
    };

    public static ObservableCollection<string> VoxDelay = new()
    {
        "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s", "1.1s", "1.2s", "1.3s", "1.4s",
        "1.5s", "1.6s", "1.7s", "1.8s", "1.9s", "2.0s"
    };

    public static ObservableCollection<string> DoubleWait = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Tone = new()
    {
        "1000Hz", "1450Hz", "1750Hz", "2100Hz"
    };

    public static ObservableCollection<string> SideTone = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> TailClear = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };
    
    public static ObservableCollection<string> RelayMode = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };
    
    public static ObservableCollection<string> RelaySpeaker = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> PowerOnDisplay = new()
    {
        "LOGO", Language.GetString("bat_volt"), Language.GetString("default_msg")
        // "LOGO", "电池电压", "预设信息"
    };

    public static ObservableCollection<string> Beep = new()
    {
        Language.GetString("off"), Language.GetString("beep"), Language.GetString("voice"), Language.GetString("all")
        // "关", "BEEP 音", "语音", "全部"
    };

    public static ObservableCollection<string> Roger = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> MicGain = new()
    {
        Language.GetString("low_gain"), Language.GetString("mid_gain"), Language.GetString("high_gain")
        // "低增益", "中增益", "高增益"
    };

    public static ObservableCollection<string> ScanMode = new()
    {
        Language.GetString("time_scan"), Language.GetString("carrier_scan"), Language.GetString("search_scan")
        // "时间扫描", "载波扫描", "搜索扫描"
    };

    public static ObservableCollection<string> Sos = new()
    {
        Language.GetString("live_sos"), Language.GetString("send_sos_voice"), Language.GetString("send_sos_code")
        // "现场报警", "发射报警音", "发射报警码"
    };

    public static ObservableCollection<string> KeyLock = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Fm = new()
    {
        Language.GetString("allow"), Language.GetString("forbid")
        // "允许", "禁止"
    };

    public static ObservableCollection<string> AutoLock = new()
    {
        Language.GetString("off"), "10 s", "30 s", "1 min", "5 min", "10 min", "30 min"
    };

    public static ObservableCollection<string> AutoQuit = new()
    {
        "5 s", "10 s", "15 s", "20 s", "25 s", "30 s", "35 s", "40 s", "45 s", "50 s",
        "60 s"
    };

    public static ObservableCollection<string> BackgroundLightTime = new()
    {
        Language.GetString("normal_open"), "5 s", "10 s", "15 s", "20 s", "30 s", "1 min", "2 min", "3 min"
    };

    public static ObservableCollection<string> Bright = new()
    {
        "60 %", "100 %"
    };

    public static ObservableCollection<string> SendCodeDelay = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms",
        "1100 ms", "1200 ms", "1300 ms", "1400 ms", "1500 ms", "1600 ms"
    };

    public static ObservableCollection<string> DisplayType = new()
    {
        Language.GetString("chan_name"), Language.GetString("freq"), Language.GetString("chan_num")
        // "信道名称", "频率", "信道号"
    };

    public static ObservableCollection<string> WorkMode = new()
    {
        Language.GetString("freq_mode"), Language.GetString("chan_mode")
        // "频率模式", "信道模式"
    };

    public static ObservableCollection<string> KeyFunc = new()
    {
        Language.GetString("fm"),
        Language.GetString("switch_pwr"),
        Language.GetString("listen"),
        Language.GetString("scan_freq"),
        Language.GetString("sos"),
        Language.GetString("weather"),
        Language.GetString("scan"),
        Language.GetString("vox"),
        Language.GetString("remote_tone_scan")
        // "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音"
    };

    public static ObservableCollection<string> RptTailClear = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
    };

    public static ObservableCollection<string> RptDetectTail = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
    };

    public static ObservableCollection<string> VoxSwitch = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> PowerUpDisplayTime = new()
    {
        "0.2 s", "0.4 s", "0.6 s", "0.8 s", "1.0 s", "1.2 s", "1.4 s", "1.6 s", "1.8 s", "2.0 s",
        "2.2 s", "2.4 s", "2.6 s", "2.8 s", "3.0 s"
    };

    public static ObservableCollection<string> BtGain = new()
    {
        "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> CurBank = new()
    {
        "1", "2", "3", "4", "5",
        "6", "7", "8", "9", "10",
        "11", "12", "13", "14", "15",
        "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25",
        "26", "27", "28", "29", "30"
    };
}