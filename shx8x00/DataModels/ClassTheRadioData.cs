using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        var binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(s, this);
    }


    public static ClassTheRadioData CreatObjFromFile(Stream s)
    {
        var binaryFormatter = new BinaryFormatter();
        return binaryFormatter.Deserialize(s) as ClassTheRadioData;
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