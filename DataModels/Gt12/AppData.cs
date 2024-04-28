using System.Xml.Serialization;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class AppData
{
    public string[] bankName = new string[30]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八", "区域九", "区域十",
        "区域十一", "区域十二", "区域十三", "区域十四", "区域十五", "区域十六", "区域十七", "区域十八", "区域十九", "区域二十",
        "区域二十一", "区域二十二", "区域二十三", "区域二十四", "区域二十五", "区域二十六", "区域二十七", "区域二十八", "区域二十九", "区域三十"
    };
    public Channel[][] channelList = new Channel[30][];
    public DTMF dtmfs = new();
    public FMChannel fms = new();
    public Function funCfgs = new();
    public MDC1200 mdcs = new();
    public VFOInfos vfos = new();

    [XmlIgnore] public static AppData instance;
    
    public static AppData getInstance()
    {
        if (instance != null) return instance;

        instance = new AppData();
        return instance;
    }
    
    public AppData()
    {
        for (var i = 0; i < 30; i++)
        {
            channelList[i] = new Channel[32];
            for (var j = 0; j < 32; j++)
            {
                var rmp = new Channel();
                rmp.Id = j + 1;
                channelList[i][j] = rmp;
            }
        }
    }
}