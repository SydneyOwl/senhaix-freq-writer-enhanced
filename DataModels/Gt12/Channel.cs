using System;
using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Channel:ObservableObject
{
    [ObservableProperty]
    private int bandwide;
    [ObservableProperty]
    // Useless member: id
    private int id;
    [ObservableProperty]
    private string name = "";
    [ObservableProperty]
    private int pttid;
    [ObservableProperty]
    private string rxFreq = "";
    [ObservableProperty]
    private int scanAdd;
    [ObservableProperty]
    private int signalGroup;
    [ObservableProperty]
    private int signalSystem;
    [ObservableProperty]
    private int sqMode;
    [ObservableProperty]
    private string strRxCtsDcs = "OFF";
    [ObservableProperty]
    private string strTxCtsDcs = "OFF";
    [ObservableProperty]
    private string txFreq = "";
    [ObservableProperty]
    private int txPower;

    [XmlIgnore]
    [ObservableProperty]
    private bool isVisable;

    public Channel()
    {
        
    }
    
    public Channel(int id, string rxFreq, string rxCts, string txFreq, string txCts, int power, int bandwide,
        int scanAdd, int sqMode, int pttid, int signal, string name)
    {
        this.id = id;
        this.rxFreq = rxFreq;
        strRxCtsDcs = rxCts;
        this.txFreq = txFreq;
        strTxCtsDcs = txCts;
        txPower = power;
        this.bandwide = bandwide;
        this.scanAdd = scanAdd;
        this.sqMode = sqMode;
        this.pttid = pttid;
        signalGroup = signal;
        this.name = name;
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