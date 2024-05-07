using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MsBox.Avalonia;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public class ClassTheRadioData
{
    [XmlIgnore] public static ClassTheRadioData Instance;

    [XmlIgnore] public ObservableCollection<ChannelData> ChanData = new();

    //TODO 无法直接反序列化到chanData, 只能这样一下
    public List<ChannelData> Channeldata = new();

    public DtmfData DtmfData = new();

    public FunCfgData FunCfgData = new();

    public OtherImfData OtherImfData = new();

    public ClassTheRadioData()
    {
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            ChanData.Add(data);
        }
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new XmlSerializer(typeof(ClassTheRadioData));
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            Instance.Channeldata = Instance.ChanData.ToList();
            serializer.Serialize(streamWriter, Instance);
        }
    }


    public static void CreatObjFromFile(Stream s)
    {
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var xmls = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(ClassTheRadioData));
                var stringReader = new StringReader(xmls);
                tmp = (ClassTheRadioData)xmlSerializer.Deserialize(stringReader);
                Instance.FunCfgData = tmp.FunCfgData;
                Instance.DtmfData = tmp.DtmfData;
                Instance.OtherImfData = tmp.OtherImfData;
                Instance.ChanData.Clear();
                foreach (var cd in tmp.Channeldata)
                {
                    if (!cd.AllEmpty()) cd.IsVisable = true;
                    Instance.ChanData.Add(cd);
                }
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
            }
        }
    }

    public static ClassTheRadioData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new ClassTheRadioData();
        return Instance;
    }

    public void ForceNewChannel()
    {
        Instance.ChanData.Clear();
        for (var i = 0; i < 128; i++)
        {
            var chan = new ChannelData();
            chan.ChanNum = i.ToString();
            chan.IsVisable = false;
            Instance.ChanData.Add(chan);
        }
    }
}