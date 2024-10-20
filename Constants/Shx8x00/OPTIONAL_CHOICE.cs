using System.Collections.ObjectModel;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Constants.Shx8x00;

public class OptionalChoice
{
    public static ObservableCollection<string> TxTimeout = new()
    {
        Language.GetString("off"), "30", "60", "90", "120", "150", "180", "210", "240"
    };

    public static ObservableCollection<string> SqlLevel = new()
    {
        "0", "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> Vox = new()
    {
        "OFF", "1", "2", "3"
    };

    public static ObservableCollection<string> VoxDelay = new()
    {
        "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s",
        "1.1s", "1.2s", "1.3s", "1.4s", "1.5s", "1.6s", "1.7s",
        "1.8s", "1.9s", "2.0s"
    };

    public static ObservableCollection<string> Speech = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> AutoBackLight = new()
    {
        "OFF", "5s", "10s", "15s", "20s", "30s", "1min", "2min", "3min"
    };

    public static ObservableCollection<string> AMode = new()
    {
        Language.GetString("freq_mode"), Language.GetString("chan_mode")
    };

    public static ObservableCollection<string> BMode = new()
    {
        Language.GetString("freq_mode"), Language.GetString("chan_mode")
    };

    public static ObservableCollection<string> ScanMode = new()
    {
        Language.GetString("time_scan"),  Language.GetString("carrier_scan"), Language.GetString("search_scan")
        // "时间", "载波", "搜索"
    };

    public static ObservableCollection<string> SavePower = new()
    {
        "OFF", "ON"
    };

    public static ObservableCollection<string> TxPwr = new()
    {
        Language.GetString("pwr_high"),  Language.GetString("pwr_low")
        // "高功率", "低功率"
    };

    public static ObservableCollection<string> TxCtcss = ChanChoice.Qtdqt;
    public static ObservableCollection<string> RxCtcss = ChanChoice.Qtdqt;

    public static ObservableCollection<string> BandWidth = new()
    {
        Language.GetString("wide"),
        Language.GetString("narrow")
        // "宽带", "窄带"
    };

    public static ObservableCollection<string> Step = new()
    {
        "2.50 kHz", "5.00 kHz", "6.25 kHz", "10.00 kHz",
        "12.50 kHz", "20.00 kHz", "25.00 kHz", "50.00 kHz"
    };

    public static ObservableCollection<string> StepDirection = new()
    {
        "OFF", "+", "-"
    };

    public static ObservableCollection<string> SignCode = ChanChoice.SignCode;

    public static ObservableCollection<string> Encrypt = new()
    {
        Language.GetString("off"), Language.GetString("on")
        // "关闭", "开启"
    };

    public static ObservableCollection<string> Display = new()
    {
        Language.GetString("chan_num_and_chan_name"), Language.GetString("chan_num_and_freq")
        // "信道号+信道名称", "信道号+频率"
    };

    public static ObservableCollection<string> Dtmf = new()
    {
        "OFF", Language.GetString("key_sidetone"), Language.GetString("idcode_sidetone"), Language.GetString("key_and_idcode_sidetone")
        // "OFF", "按键侧音", "发身份码侧音", "按键侧音+发身份码"
    };

    public static ObservableCollection<string> Pttid = new()
    {
        "OFF", Language.GetString("tx_on_press"), Language.GetString("tx_on_release"), Language.GetString("tx_on_both")
        // "OFF", "按下发码", "松开发码", "两者均发"
    };

    public static ObservableCollection<string> IdDelay = new()
    {
        "0", "1", "2", "3", "4", "5", "6",
        "7", "8", "9", "10", "11", "12",
        "13", "14", "15", "16", "17", "18",
        "19", "20", "21", "22", "23", "24",
        "25", "26", "27", "28", "29", "30"
    };

    public static ObservableCollection<string> Side = new()
    {
        Language.GetString("fm"),
        Language.GetString("switch_pwr"),
        Language.GetString("listen"),
        Language.GetString("scan"),
        Language.GetString("scan_freq")
        // "收音机", "功率切换", "监听", "扫描", "扫频"
    };

    public static ObservableCollection<string> TailTone = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };

    public static ObservableCollection<string> Rpt = new()
    {
        "0", "100ms", "200ms", "300ms", "400ms", "500ms", "600ms", "700ms", "800ms", "900ms", "1000ms"
    };

    public static ObservableCollection<string> BootImage = new()
    {
        Language.GetString("default_img1"), Language.GetString("default_img2"), Language.GetString("default_word")
        // "预设图片1", "预设图片2", "预设字符"
    };

    public static ObservableCollection<string> MenuExit = new()
    {
        "5s", "10s", "15s", "20s", "25s", "30s", "35s", "40s", "45s", "50s", "55s", "60s"
    };

    public static ObservableCollection<string> MicGain = new()
    {
        "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> RptFrq = new()
    {
        "1000Hz", "1450Hz", "1750Hz", "2100Hz"
    };

    public static ObservableCollection<string> AlarmMode = new()
    {
        Language.GetString("live_sos"),  Language.GetString("send_sos_voice"), Language.GetString("send_sos_code")
        // "现场", "发射报警音", "发射报警码"
    };

    public static ObservableCollection<string> AlarmEnd = new()
    {
        Language.GetString("off"), Language.GetString("on")
    };
}