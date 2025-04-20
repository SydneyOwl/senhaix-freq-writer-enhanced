using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class Channel : ObservableObject
{
#pragma warning disable CS0657
    [ObservableProperty] private int _id; //
    [ObservableProperty] private string _rxFreq = ""; //
    [ObservableProperty] private string _strRxCtsDcs = "OFF"; //
    [ObservableProperty] private string _txFreq = ""; //
    [ObservableProperty] private string _strTxCtsDcs = "OFF"; //
    [ObservableProperty] private int _txPower; //
    [ObservableProperty] private int _bandwide; //
    [ObservableProperty] private int _scanAdd; //
    [ObservableProperty] private int _busyLock;
    [ObservableProperty] private int _pttid; //
    [ObservableProperty] private int _signalGroup; //
    [ObservableProperty] private string _name = ""; //
    [ObservableProperty] private bool _isVisable;
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

    public ExcelChannel ToExcelChannel()
    {
        return new ExcelChannel
        {
            Id = Id.ToString(),
            RxFreq = RxFreq.ToString(),
            StrRxCtsDcs = StrRxCtsDcs.ToString(),
            TxFreq = TxFreq,
            StrTxCtsDcs = StrTxCtsDcs,
            TxPower = Constants.Shx8800Pro.ChanChoice.Power[TxPower],
            Bandwide = Constants.Shx8800Pro.ChanChoice.Bandwidth[Bandwide],
            ScanAdd = Constants.Shx8800Pro.ChanChoice.Scanadd[ScanAdd],
            BusyLock = Constants.Shx8800Pro.ChanChoice.BusyLock[BusyLock],
            Pttid = Constants.Shx8800Pro.ChanChoice.Pttid[Pttid],
            SignalGroup = Constants.Shx8800Pro.ChanChoice.SigGrp[SignalGroup],
            Name = Name,
            IsVisable = false
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

    public override string ToString()
    {
        return
            $"{nameof(_id)}: {_id}, {nameof(_rxFreq)}: {_rxFreq}, {nameof(_strRxCtsDcs)}: {_strRxCtsDcs}, {nameof(_txFreq)}: {_txFreq}, {nameof(_strTxCtsDcs)}: {_strTxCtsDcs}, {nameof(_txPower)}: {_txPower}, {nameof(_bandwide)}: {_bandwide}, {nameof(_scanAdd)}: {_scanAdd}, {nameof(_busyLock)}: {_busyLock}, {nameof(_pttid)}: {_pttid}, {nameof(_signalGroup)}: {_signalGroup}, {nameof(_name)}: {_name}, {nameof(_isVisable)}: {_isVisable}, {nameof(Id)}: {Id}, {nameof(RxFreq)}: {RxFreq}, {nameof(StrRxCtsDcs)}: {StrRxCtsDcs}, {nameof(TxFreq)}: {TxFreq}, {nameof(StrTxCtsDcs)}: {StrTxCtsDcs}, {nameof(TxPower)}: {TxPower}, {nameof(Bandwide)}: {Bandwide}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(BusyLock)}: {BusyLock}, {nameof(Pttid)}: {Pttid}, {nameof(SignalGroup)}: {SignalGroup}, {nameof(Name)}: {Name}, {nameof(IsVisable)}: {IsVisable}";
    }
}