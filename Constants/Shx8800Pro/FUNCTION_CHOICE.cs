using System.Collections.ObjectModel;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Constants.Shx8800Pro;

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
        // "关", "普通省电", "超级省电", "深度省电"
        Language.GetString("off"),
        Language.GetString("normal_pwr_save"),
        Language.GetString("super_pwr_save"),
        Language.GetString("deep_pwr_save")
    };

    public static ObservableCollection<string> Vox = new()
    {
        // "高灵敏度", "中灵敏度", "低灵敏度"
        Language.GetString("high_sensitivity"), Language.GetString("mid_sensitivity"),
        Language.GetString("low_sensitivity")
    };

    public static ObservableCollection<string> VoiceSw = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> VoxDelay = new()
    {
        "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s", "1.1s", "1.2s", "1.3s", "1.4s",
        "1.5s", "1.6s", "1.7s", "1.8s", "1.9s", "2.0s"
    };

    public static ObservableCollection<string> DoubleWait = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Tone = new()
    {
        "1000Hz", "1450Hz", "1750Hz", "2100Hz"
    };

    public static ObservableCollection<string> SideTone = new()
    {
        // "关", "按键侧音", "身份码侧音", "两者"
        Language.GetString("off"),
        Language.GetString("key_sidetone"),
        Language.GetString("idcode_sidetone"),
        Language.GetString("both")
    };

    public static ObservableCollection<string> TailClear = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> PowerOnDisplay = new()
    {
        "LOGO", Language.GetString("bat_volt")
    };

    public static ObservableCollection<string> Beep = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Roger = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> BtMicGain = new()
    {
        "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> MicGain = new()
    {
        // "低增益", "中增益", "高增益"
        Language.GetString("low_gain"), Language.GetString("mid_gain"), Language.GetString("high_gain")
    };

    public static ObservableCollection<string> ScanMode = new()
    {
        // "时间扫描", "载波扫描", "搜索扫描"
        Language.GetString("time_scan"), Language.GetString("carrier_scan"), Language.GetString("search_scan")
    };

    public static ObservableCollection<string> Sos = new()
    {
        // "现场报警", "发射报警音", "发射报警码"
        Language.GetString("live_sos"), Language.GetString("send_sos_voice"), Language.GetString("send_sos_code")
    };

    public static ObservableCollection<string> SosVoice = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> KeyLock = new()
    {
        // "关", "开"
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Fm = new()
    {
        // "允许", "禁止"
        Language.GetString("allow"), Language.GetString("forbid")
    };

    public static ObservableCollection<string> AutoLock = new()
    {
        Language.GetString("off"), "5 s", "10 s", "15 s"
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

    public static ObservableCollection<string> SendCodeDelay = new()
    {
        "0 ms", "100 ms", "200 ms", "400 ms", "600 ms", "800 ms"
    };

    public static ObservableCollection<string> DisplayType = new()
    {
        // "信道名称", "频率", "信道号"
        Language.GetString("chan_name"), Language.GetString("freq"), Language.GetString("chan_num")
    };

    public static ObservableCollection<string> WorkMode = new()
    {
        // "频率模式", "信道模式"
        Language.GetString("freq_mode"), Language.GetString("chan_mode")
    };

    public static ObservableCollection<string> KeyFunc = new()
    {
        // "收音机", "监听", "扫描", "扫频", "天气预报"
        Language.GetString("fm"),
        Language.GetString("listen"),
        Language.GetString("scan"),
        Language.GetString("scan_freq"),
        Language.GetString("weather")
    };

    public static ObservableCollection<string> RptTailClear = new()
    {
        "0", "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
    };

    public static ObservableCollection<string> RptDetectTail = new()
    {
        "0", "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
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
        "6", "7", "8"
    };
}