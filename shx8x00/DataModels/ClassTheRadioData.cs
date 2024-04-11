using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;

namespace shx8x00.DataModels;

[Serializable]
public class ClassTheRadioData
{
    public ObservableCollection<ChannelData> channelData = new();

    public DTMFData dtmfData = new();

    public FunCFGData funCfgData = new();

    public OtherImfData otherImfData = new();

    public static ClassTheRadioData instance;

    public ClassTheRadioData()
    {
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            channelData.Add(data);
        }
    }
    public void SaveToFile(Stream s)
    {
        string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
        using (StreamWriter streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            Console.Write(instance.channelData[0].ToString());
            streamWriter.Write(json);
        }
    }


    public static void CreatObjFromFile(Stream s)
    {
        using (StreamReader streamReader = new StreamReader(s, Encoding.UTF8))
        {
            string json = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            try
            {
                tmp = JsonConvert.DeserializeObject<ClassTheRadioData>(json);
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
                return;
            }
           if (tmp == null)
           {
               MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
           }
           else
           {
               instance = tmp;
               Console.Write(instance.channelData[0].ToString());
           }
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