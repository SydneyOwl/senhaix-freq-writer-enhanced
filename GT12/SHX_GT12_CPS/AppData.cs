using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SHX_GT12_CPS;

[Serializable]
public class AppData
{
    private string[] bankName = new string[30];

    private Channel[][] channelList = new Channel[30][];

    private DTMF dtmfs = new();

    private FMChannel fms = new();

    private Function funCfgs = new();

    private MDC1200 mdcs = new();

    private string[] strAreaCN = new string[30]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八", "区域九", "区域十",
        "区域十一", "区域十二", "区域十三", "区域十四", "区域十五", "区域十六", "区域十七", "区域十八", "区域十九", "区域二十",
        "区域二十一", "区域二十二", "区域二十三", "区域二十四", "区域二十五", "区域二十六", "区域二十七", "区域二十八", "区域二十九", "区域三十"
    };

    private string[] strAreaEN = new string[30]
    {
        "ZONE 1", "ZONE 2", "ZONE 3", "ZONE 4", "ZONEE 5", "ZONE 6", "ZONE 7", "ZONE 8", "ZONE 9", "ZONE 10",
        "ZONE 11", "ZONE 12", "ZONE 13", "ZONE 14", "ZONEE 15", "ZONE 16", "ZONE 17", "ZONE 18", "ZONE 19", "ZONE 20",
        "ZONE 21", "ZONE 22", "ZONE 23", "ZONE 24", "ZONEE 25", "ZONE 26", "ZONE 27", "ZONE 28", "ZONE 29", "ZONE 30"
    };

    private VFOInfos vfos = new();

    public AppData(string LANG)
    {
        if (LANG == "Chinese")
            bankName = strAreaCN;
        else
            bankName = strAreaEN;

        for (var i = 0; i < 30; i++)
        {
            ChannelList[i] = new Channel[32];
            for (var j = 0; j < 32; j++) ChannelList[i][j] = new Channel();
        }
    }

    public Channel[][] ChannelList
    {
        get => channelList;
        set => channelList = value;
    }

    public VFOInfos Vfos
    {
        get => vfos;
        set => vfos = value;
    }

    public Function FunCfgs
    {
        get => funCfgs;
        set => funCfgs = value;
    }

    public FMChannel Fms
    {
        get => fms;
        set => fms = value;
    }

    public DTMF Dtmfs
    {
        get => dtmfs;
        set => dtmfs = value;
    }

    public MDC1200 Mdcs
    {
        get => mdcs;
        set => mdcs = value;
    }

    public string[] BankName
    {
        get => bankName;
        set => bankName = value;
    }

    public void SaveToFile(Stream s)
    {
        var binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(s, this);
    }

    public static AppData CreatObjFromFile(Stream s)
    {
        var binaryFormatter = new BinaryFormatter();
        return binaryFormatter.Deserialize(s) as AppData;
    }
}