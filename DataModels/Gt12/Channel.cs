using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Channel : ObservableObject
{
#pragma warning disable CS0657
    [ObservableProperty] [property: EpplusTableColumn(Header = "信道")] private int _id;
    [ObservableProperty] [property: EpplusTableColumn(Header = "接收频率")] private string _rxFreq = "";
    [ObservableProperty] [property: EpplusTableColumn(Header = "亚音解码")] private string _strRxCtsDcs = "OFF";
    [ObservableProperty] [property: EpplusTableColumn(Header = "发射频率")] private string _txFreq = "";
    [ObservableProperty] [property: EpplusTableColumn(Header = "亚音编码")] private string _strTxCtsDcs = "OFF";
    [ObservableProperty] [property: EpplusTableColumn(Header = "功率")] private int _txPower;
    [ObservableProperty] [property: EpplusTableColumn(Header = "带宽")] private int _bandwide;
    [ObservableProperty] [property: EpplusTableColumn(Header = "扫描添加")] private int _scanAdd;
    [ObservableProperty] [property: EpplusTableColumn(Header = "信令")] private int _signalSystem;
    [ObservableProperty] [property: EpplusTableColumn(Header = "静音模式")] private int _sqMode;
    [ObservableProperty] [property: EpplusTableColumn(Header = "PTT-ID")] private int _pttid;
    [ObservableProperty] [property: EpplusTableColumn(Header = "信令码")] private int _signalGroup;
    [ObservableProperty] [property: EpplusTableColumn(Header = "信道名称")] private string _name = "";
    [XmlIgnore, ObservableProperty] [property: EpplusTableColumn(Hidden = true)] private bool _isVisable;

#pragma warning restore CS0657
    public Channel()
    {
    }

    public Channel(int id, string rxFreq, string rxCts, string txFreq, string txCts, int power, int bandwide,
        int scanAdd, int sqMode, int pttid, int signal, string name)
    {
        _id = id;
        _rxFreq = rxFreq;
        _strRxCtsDcs = rxCts;
        _txFreq = txFreq;
        _strTxCtsDcs = txCts;
        _txPower = power;
        _bandwide = bandwide;
        _scanAdd = scanAdd;
        _sqMode = sqMode;
        _pttid = pttid;
        _signalGroup = signal;
        _name = name;
    }

    public Channel DeepCopy()
    {
        Channel rel;
        using (var ms = new MemoryStream())
        {
            var xml = new XmlSerializer(typeof(Channel));
            xml.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            rel = (Channel)xml.Deserialize(ms);
            ms.Close();
        }

        return rel;
    }
}