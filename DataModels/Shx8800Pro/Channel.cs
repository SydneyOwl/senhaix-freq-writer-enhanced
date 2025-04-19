using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class Channel : ObservableObject
{
#pragma warning disable CS0657
    [ObservableProperty] [property:EpplusTableColumn(Header = "信道")] private int _id; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "接收频率")] private string _rxFreq = ""; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "亚音解码")] private string _strRxCtsDcs = "OFF"; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "发射频率")] private string _txFreq = ""; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "亚音编码")] private string _strTxCtsDcs = "OFF"; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "功率")] private int _txPower; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "带宽")] private int _bandwide; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "扫描添加")] private int _scanAdd; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "繁忙锁定")] private int _busyLock;
    [ObservableProperty] [property:EpplusTableColumn(Header = "PTT-ID")] private int _pttid; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "信令码")] private int _signalGroup; //
    [ObservableProperty] [property:EpplusTableColumn(Header = "信道名称")] private string _name = ""; //
    [ObservableProperty] [property:EpplusTableColumn(Hidden = true)] private bool _isVisable;
#pragma warning restore CS0657

    public Channel()
    {
    }

    public Channel(int id, string rxFreq, string rxCts, string txFreq, string txCts, int power, int bandwide,
        int scanAdd, int busyLock, int pttid, int signal, string name)
    {
        _id = id;
        _rxFreq = rxFreq;
        _strRxCtsDcs = rxCts;
        _txFreq = txFreq;
        _strTxCtsDcs = txCts;
        _txPower = power;
        _bandwide = bandwide;
        _scanAdd = scanAdd;
        _pttid = pttid;
        _signalGroup = signal;
        _busyLock = busyLock;
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

    public override string ToString()
    {
        return
            $"{nameof(_id)}: {_id}, {nameof(_rxFreq)}: {_rxFreq}, {nameof(_strRxCtsDcs)}: {_strRxCtsDcs}, {nameof(_txFreq)}: {_txFreq}, {nameof(_strTxCtsDcs)}: {_strTxCtsDcs}, {nameof(_txPower)}: {_txPower}, {nameof(_bandwide)}: {_bandwide}, {nameof(_scanAdd)}: {_scanAdd}, {nameof(_busyLock)}: {_busyLock}, {nameof(_pttid)}: {_pttid}, {nameof(_signalGroup)}: {_signalGroup}, {nameof(_name)}: {_name}, {nameof(_isVisable)}: {_isVisable}, {nameof(Id)}: {Id}, {nameof(RxFreq)}: {RxFreq}, {nameof(StrRxCtsDcs)}: {StrRxCtsDcs}, {nameof(TxFreq)}: {TxFreq}, {nameof(StrTxCtsDcs)}: {StrTxCtsDcs}, {nameof(TxPower)}: {TxPower}, {nameof(Bandwide)}: {Bandwide}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(BusyLock)}: {BusyLock}, {nameof(Pttid)}: {Pttid}, {nameof(SignalGroup)}: {SignalGroup}, {nameof(Name)}: {Name}, {nameof(IsVisable)}: {IsVisable}";
    }
}