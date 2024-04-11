using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MsBox.Avalonia;

namespace shx8x00.DataModels;

[Serializable]
public class ClassTheRadioData
{
    public ObservableCollection<ChannelData> chanData = new();

    public DTMFData dtmfData = new();

    public FunCFGData funCfgData = new();

    public OtherImfData otherImfData = new();
    
    [XmlIgnore]
    public static ClassTheRadioData instance;

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
        XmlSerializer serializer = new XmlSerializer(typeof(ClassTheRadioData));
        using (StreamWriter streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            serializer.Serialize(streamWriter, instance);
        }
    }


    public static void CreatObjFromFile(Stream s)
    {
        using (StreamReader streamReader = new StreamReader(s, Encoding.UTF8))
        {
            string xmls = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ClassTheRadioData));
                StringReader stringReader = new StringReader(xmls);
                tmp = (ClassTheRadioData)xmlSerializer.Deserialize(stringReader);
                Console.WriteLine(tmp.chanData[0].ToString());
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
        if (instance != null)
        {
            return instance;
        }

        instance = new ClassTheRadioData();
        return instance;
    }
    
    public static void forceNew()
    {
        instance = new ClassTheRadioData();
    }
}