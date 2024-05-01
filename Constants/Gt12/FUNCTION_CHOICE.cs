using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Gt12;

public class FUNCTION_CHOICE
{
    public static ObservableCollection<string> sql = new()
    {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
    };

    public static ObservableCollection<string> txTimeout = new()
    {
        "关", "30s", "60s", "90s", "120s", "150s", "180s", "210s", "240s"
    };

    public static ObservableCollection<string> saveMode = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> vox = new()
    {
        "高灵敏度", "中灵敏度", "低灵敏度"
    };

    public static ObservableCollection<string> voxDelay = new()
    {
        "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s", "1.1s", "1.2s", "1.3s", "1.4s",
        "1.5s", "1.6s", "1.7s", "1.8s", "1.9s", "2.0s"
    };

    public static ObservableCollection<string> doubleWait = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> tone = new()
    {
        "1000Hz", "1450Hz", "1750Hz", "2100Hz"
    };

    public static ObservableCollection<string> sideTone = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> tailClear = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> powerOnDisplay = new()
    {
        "LOGO", "电池电压", "预设信息"
    };

    public static ObservableCollection<string> beep = new()
    {
        "关", "BEEP 音", "语音", "全部"
    };

    public static ObservableCollection<string> roger = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> micGain = new()
    {
        "低增益", "中增益", "高增益"
    };

    public static ObservableCollection<string> scanMode = new()
    {
        "时间扫描", "载波扫描", "搜索扫描"
    };

    public static ObservableCollection<string> SOS = new()
    {
        "现场报警", "发射报警音", "发射报警码"
    };

    public static ObservableCollection<string> keyLock = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> FM = new()
    {
        "允许", "禁止"
    };

    public static ObservableCollection<string> autoLock = new()
    {
        "关", "10 s", "30 s", "1 min", "5 min", "10 min", "30 min"
    };

    public static ObservableCollection<string> autoQuit = new()
    {
        "5 s", "10 s", "15 s", "20 s", "25 s", "30 s", "35 s", "40 s", "45 s", "50 s",
        "60 s"
    };

    public static ObservableCollection<string> backgroundLightTime = new()
    {
        "常开", "5 s", "10 s", "15 s", "20 s", "30 s", "1 min", "2 min", "3 min"
    };

    public static ObservableCollection<string> bright = new()
    {
        "60 %", "100 %"
    };

    public static ObservableCollection<string> sendCodeDelay = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms",
        "1100 ms", "1200 ms", "1300 ms", "1400 ms", "1500 ms", "1600 ms"
    };

    public static ObservableCollection<string> displayType = new()
    {
        "信道名称", "频率", "信道号"
    };

    public static ObservableCollection<string> workMode = new()
    {
        "频率模式", "信道模式"
    };

    public static ObservableCollection<string> keyFunc = new()
    {
        "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音"
    };

    public static ObservableCollection<string> rptTailClear = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
    };

    public static ObservableCollection<string> rptDetectTail = new()
    {
        "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms"
    };

    public static ObservableCollection<string> voxSwitch = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> powerUpDisplayTime = new()
    {
        "0.2 s", "0.4 s", "0.6 s", "0.8 s", "1.0 s", "1.2 s", "1.4 s", "1.6 s", "1.8 s", "2.0 s",
        "2.2 s", "2.4 s", "2.6 s", "2.8 s", "3.0 s"
    };

    public static ObservableCollection<string> BTGain = new()
    {
        "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> curBank = new()
    {
        "1", "2", "3", "4", "5",
        "6", "7", "8", "9", "10",
        "11", "12", "13", "14", "15",
        "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25",
        "26", "27", "28", "29", "30"
    };
}