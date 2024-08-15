using System;
using System.IO;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using SenhaixFreqWriter.DataModels.Interfaces;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public class AppData : IBackupable
{
    [JsonIgnore] public static AppData Instance;

    public string[] BankName = new string[8]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八",
    };//

    public Channel[][] ChannelList = new Channel[8][];//
    public Dtmf Dtmfs = new();//
    public FmChannel Fms = new();//
    public Function FunCfgs = new();//
    public Mdc1200 Mdcs = new();//
    public VfoInfos Vfos = new();//

    public AppData()
    {
        for (var i = 0; i < 8; i++)
        {
            ChannelList[i] = new Channel[64];
            for (var j = 0; j < 64; j++)
            {
                var rmp = new Channel();
                rmp.Id = j + 1;
                ChannelList[i][j] = rmp;
            }
        }
    }

    public static AppData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new AppData();
        return Instance;
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