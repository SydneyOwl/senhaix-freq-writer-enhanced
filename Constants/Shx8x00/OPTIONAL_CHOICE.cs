using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Shx8x00;

public class OPTIONAL_CHOICE
{
    public static ObservableCollection<string> txTimeout = new()
    {
        "关", "30", "60", "90", "120", "150", "180", "210", "240"
    };

    public static ObservableCollection<string> sqlLevel = new()
    {
        "0", "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> VOX = new()
    {
        "OFF", "1", "2", "3"
    };

    public static ObservableCollection<string> VOXDelay = new()
    {
        "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s",
        "1.1s", "1.2s", "1.3s", "1.4s", "1.5s", "1.6s", "1.7s",
        "1.8s", "1.9s", "2.0s"
    };

    public static ObservableCollection<string> speech = new()
    {
        "关", "开"
    };

    public static ObservableCollection<string> autoBackLight = new()
    {
        "OFF", "5s", "10s", "15s", "20s", "30s", "1min", "2min", "3min"
    };

    public static ObservableCollection<string> AMode = new()
    {
        "频率模式", "信道模式"
    };

    public static ObservableCollection<string> BMode = new()
    {
        "频率模式", "信道模式"
    };

    public static ObservableCollection<string> scanMode = new()
    {
        "时间", "载波", "搜索"
    };

    public static ObservableCollection<string> savePower = new()
    {
        "OFF", "ON"
    };

    public static ObservableCollection<string> txPwr = new()
    {
        "高功率", "低功率"
    };

    public static ObservableCollection<string> txCTCSS = CHAN_CHOICE.qtdqt;
    public static ObservableCollection<string> rxCTCSS = CHAN_CHOICE.qtdqt;

    public static ObservableCollection<string> bandWidth = new()
    {
        "宽带", "窄带"
    };

    public static ObservableCollection<string> step = new()
    {
        "2.50 kHz", "5.00 kHz", "6.25 kHz", "10.00 kHz",
        "12.50 kHz", "20.00 kHz", "25.00 kHz", "50.00 kHz"
    };

    public static ObservableCollection<string> stepDirection = new()
    {
        "OFF", "+", "-"
    };

    public static ObservableCollection<string> signCode = CHAN_CHOICE.signCode;

    public static ObservableCollection<string> encrypt = new()
    {
        "关闭", "开启"
    };

    public static ObservableCollection<string> display = new()
    {
        "信道号+信道名称", "信道号+频率"
    };

    public static ObservableCollection<string> DTMF = new()
    {
        "OFF", "按键侧音", "发身份码侧音", "按键侧音+发身份码"
    };

    public static ObservableCollection<string> pttid = new()
    {
        "OFF", "按下发码", "松开发码", "两者均发"
    };

    public static ObservableCollection<string> idDelay = new()
    {
        "0", "1", "2", "3", "4", "5", "6",
        "7", "8", "9", "10", "11", "12",
        "13", "14", "15", "16", "17", "18",
        "19", "20", "21", "22", "23", "24",
        "25", "26", "27", "28", "29", "30"
    };

    public static ObservableCollection<string> side = new()
    {
        "收音机", "功率切换", "监听", "扫描", "扫频"
    };

    public static ObservableCollection<string> tailTone = new()
    {
        "关闭", "开启"
    };

    public static ObservableCollection<string> rpt = new()
    {
        "0", "100ms", "200ms", "300ms", "400ms", "500ms", "600ms", "700ms", "800ms", "900ms", "1000ms"
    };

    public static ObservableCollection<string> bootImage = new()
    {
        "预设图片1", "预设图片2", "预设字符"
    };

    public static ObservableCollection<string> menuExit = new()
    {
        "5s", "10s", "15s", "20s", "25s", "30s", "35s", "40s", "45s", "50s", "55s", "60s"
    };

    public static ObservableCollection<string> micGain = new()
    {
        "1", "2", "3", "4", "5"
    };

    public static ObservableCollection<string> rptFrq = new()
    {
        "1000Hz", "1450Hz", "1750Hz", "2100Hz"
    };

    public static ObservableCollection<string> alarmMode = new()
    {
        "现场", "发射报警音", "发射报警码"
    };

    public static ObservableCollection<string> alarmEnd = new()
    {
        "关闭", "开启"
    };
}