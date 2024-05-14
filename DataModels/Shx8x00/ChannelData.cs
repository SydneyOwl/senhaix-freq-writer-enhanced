using System;
using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class ChannelData : ObservableObject
{
    [ObservableProperty] private string _bandWidth = "";

    [ObservableProperty] private string _busyLock = "";

    [ObservableProperty] private string _chanName = "";

    [ObservableProperty] private string _chanNum = "";

    [ObservableProperty] private string _encrypt = "";

    [XmlIgnore]
    // [JsonIgnore]
    [ObservableProperty]
    private bool _isVisable;

    [ObservableProperty] private string _pttid = "";

    [ObservableProperty] private string _qtDec = "";

    [ObservableProperty] private string _qtEnc = "";

    [ObservableProperty] private string _rxFreq = "";

    [ObservableProperty] private string _scanAdd = "";

    [ObservableProperty] private string _sigCode = "";

    [ObservableProperty] private string _txAllow = "";

    [ObservableProperty] private string _txFreq = "";

    [ObservableProperty] private string _txPwr = "";

    public ChannelData DeepCopy()
    {
        ChannelData rel;
        using (var ms = new MemoryStream())
        {
            var xml = new XmlSerializer(typeof(ChannelData));
            xml.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            rel = (ChannelData)xml.Deserialize(ms)!;
            ms.Close();
        }

        return rel;
    }


    public void ChangeByNum(int index, string target)
    {
        switch (index)
        {
            case 0:
                ChanNum = target;
                break;
            case 1:
                TxAllow = target;
                break;
            case 2:
                RxFreq = target;
                if (!string.IsNullOrEmpty(RxFreq)) IsVisable = true;
                break;
            case 3:
                QtDec = target;
                break;
            case 4:
                TxFreq = target;
                break;
            case 5:
                QtEnc = target;
                break;
            case 6:
                TxPwr = target;
                break;
            case 7:
                BandWidth = target;
                break;
            case 8:
                Pttid = target;
                break;
            case 9:
                BusyLock = target;
                break;
            case 10:
                ScanAdd = target;
                break;
            case 11:
                SigCode = target;
                break;
            case 12:
                ChanName = target;
                break;
            case 13:
                Encrypt = target;
                break;
        }
    }

    public string[] TransList()
    {
        return new[]
        {
            ChanNum, TxAllow, RxFreq, QtDec, TxFreq, QtEnc, TxPwr, BandWidth, Pttid, BusyLock, ScanAdd,
            SigCode,
            ChanName,
            Encrypt
        };
    }

    public override string ToString()
    {
        return
            $"{nameof(BandWidth)}: {BandWidth}, {nameof(BusyLock)}: {BusyLock}, {nameof(ChanName)}: {ChanName}, {nameof(ChanNum)}: {ChanNum}, {nameof(Encrypt)}: {Encrypt}, {nameof(Pttid)}: {Pttid}, {nameof(QtDec)}: {QtDec}, {nameof(QtEnc)}: {QtEnc}, {nameof(RxFreq)}: {RxFreq}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(SigCode)}: {SigCode}, {nameof(TxAllow)}: {TxAllow}, {nameof(TxFreq)}: {TxFreq}, {nameof(TxPwr)}: {TxPwr}";
    }

    public bool AllEmpty()
    {
        return string.IsNullOrEmpty(BandWidth) && string.IsNullOrEmpty(BusyLock) && string.IsNullOrEmpty(ChanName) &&
               string.IsNullOrEmpty(Encrypt) && string.IsNullOrEmpty(Pttid) && string.IsNullOrEmpty(QtDec) &&
               string.IsNullOrEmpty(QtEnc) && string.IsNullOrEmpty(ScanAdd) && string.IsNullOrEmpty(SigCode) &&
               string.IsNullOrEmpty(TxAllow)
               && string.IsNullOrEmpty(TxFreq) && string.IsNullOrEmpty(TxPwr);
    }

    public bool Filled()
    {
        return !string.IsNullOrEmpty(BandWidth) && !string.IsNullOrEmpty(BusyLock) &&
               !string.IsNullOrEmpty(ChanName) &&
               !string.IsNullOrEmpty(Encrypt) && !string.IsNullOrEmpty(Pttid) && !string.IsNullOrEmpty(QtDec) &&
               !string.IsNullOrEmpty(QtEnc) && !string.IsNullOrEmpty(ScanAdd) && !string.IsNullOrEmpty(SigCode) &&
               !string.IsNullOrEmpty(TxAllow)
               && !string.IsNullOrEmpty(TxFreq) && !string.IsNullOrEmpty(TxPwr);
    }
}