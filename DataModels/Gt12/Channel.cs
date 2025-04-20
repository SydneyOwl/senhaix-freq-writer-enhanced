using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Channel : ObservableObject
{
#pragma warning disable CS0657
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _rxFreq = "";
    [ObservableProperty] private string _strRxCtsDcs = "OFF";
    [ObservableProperty] private string _txFreq = "";
    [ObservableProperty] private string _strTxCtsDcs = "OFF";
    [ObservableProperty] private int _txPower;
    [ObservableProperty] private int _bandwide;
    [ObservableProperty] private int _scanAdd;
    [ObservableProperty] private int _signalSystem;
    [ObservableProperty] private int _sqMode;
    [ObservableProperty] private int _pttid;
    [ObservableProperty] private int _signalGroup;
    [ObservableProperty] private string _name = "";
    [XmlIgnore, ObservableProperty] private bool _isVisable;

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

    public ExcelChannel ToExcelChannel()
    {
        return new ExcelChannel
        {
            Id = Id.ToString(),
            RxFreq = RxFreq,
            StrRxCtsDcs = StrRxCtsDcs,
            TxFreq = TxFreq,
            StrTxCtsDcs = StrTxCtsDcs,
            TxPower = Constants.Gt12.ChanChoice.Power[TxPower],
            Bandwide = Constants.Gt12.ChanChoice.Bandwidth[Bandwide],
            ScanAdd = Constants.Gt12.ChanChoice.Scanadd[ScanAdd],
            SignalSystem = Constants.Gt12.ChanChoice.SigSys[SignalSystem],
            SqMode = Constants.Gt12.ChanChoice.Sql[SqMode],
            Pttid = Constants.Gt12.ChanChoice.Pttid[Pttid],
            SignalGroup = Constants.Gt12.ChanChoice.SigGrp[SignalGroup],
            Name = Name
        };
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