using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MsBox.Avalonia;

namespace shx8x00.DataModels;

[Serializable]
public class ClassTheRadioData
{
    [XmlIgnore] public static ClassTheRadioData instance;

    [XmlIgnore] public ObservableCollection<ChannelData> chanData = new();

    //TODO 无法直接反序列化到chanData, 只能这样一下
    public List<ChannelData> channeldata = new();

    public DTMFData dtmfData = new();

    public FunCFGData funCfgData = new();

    public OtherImfData otherImfData = new();

    public ClassTheRadioData()
    {
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            chanData.Add(data);
        }
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new XmlSerializer(typeof(ClassTheRadioData));
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            instance.channeldata = instance.chanData.ToList();
            serializer.Serialize(streamWriter, instance);
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
                tmp.chanData = new ObservableCollection<ChannelData>(tmp.channeldata);
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
                return;
            }

            instance = tmp;
        }
    }

    public static ClassTheRadioData getInstance()
    {
        if (instance != null) return instance;

        instance = new ClassTheRadioData();
        return instance;
    }

    public static void forceNew()
    {
        instance = new ClassTheRadioData();
    }
}