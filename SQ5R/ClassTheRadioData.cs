using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SQ5R;

[Serializable]
public class ClassTheRadioData
{
    private string[][] channelData = new string[128][];

    public DTMFData dtmfData = new();

    public FormFunCFGData funCfgData = new();

    public OtherImfData otherImfData = new();

    public string[][] ChannelData
    {
        get => channelData;
        set => channelData = value;
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
}