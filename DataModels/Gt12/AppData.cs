using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MsBox.Avalonia;
using Newtonsoft.Json;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class AppData
{
    public string[] BankName = new string[30]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八", "区域九", "区域十",
        "区域十一", "区域十二", "区域十三", "区域十四", "区域十五", "区域十六", "区域十七", "区域十八", "区域十九", "区域二十",
        "区域二十一", "区域二十二", "区域二十三", "区域二十四", "区域二十五", "区域二十六", "区域二十七", "区域二十八", "区域二十九", "区域三十"
    };

    public Channel[][] ChannelList = new Channel[30][];
    public Dtmf Dtmfs = new();
    public FmChannel Fms = new();
    public Function FunCfgs = new();
    public Mdc1200 Mdcs = new();
    public VfoInfos Vfos = new();

    [JsonIgnore] public static AppData Instance;

    public static AppData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new AppData();
        return Instance;
    }

    public AppData()
    {
        for (var i = 0; i < 30; i++)
        {
            ChannelList[i] = new Channel[32];
            for (var j = 0; j < 32; j++)
            {
                var rmp = new Channel();
                rmp.Id = j + 1;
                ChannelList[i][j] = rmp;
            }
        }
    }

    public static AppData ForceNewInstance()
    {
        Instance = new AppData();
        return Instance;
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new JsonSerializer();
        serializer.Formatting = Formatting.Indented;
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            serializer.Serialize(streamWriter, Instance);
        }
    }


    public static void CreatObjFromFile(Stream s)
    {
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var res = streamReader.ReadToEnd();
            AppData tmp;
            try
            {
                var jsonSerializer = new JsonSerializer();
                var stringReader = new JsonTextReader(new StringReader(res));
                tmp = jsonSerializer.Deserialize<AppData>(stringReader);
                Instance.Dtmfs = tmp.Dtmfs;
                Instance.FunCfgs = tmp.FunCfgs;
                Instance.Fms = tmp.Fms;
                Instance.Mdcs = tmp.Mdcs;
                Instance.Vfos = tmp.Vfos;
                Instance.BankName = tmp.BankName;
                Instance.ChannelList = tmp.ChannelList;
            }
            catch (Exception e)
            {
                // DebugWindow.GetInstance().updateDebugContent(e.Message);
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
            }
        }
    }
}