using System;
using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class ChannelData : ObservableObject
{
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
#pragma warning disable CS0657
    [ObservableProperty] [property: EpplusTableColumn(Header = "信道号")] [property: JsonProperty]
    private string _chanNum = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "发射允许")] [property: JsonProperty]
    private string _txAllow = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "接收频率")] [property: JsonProperty]
    private string _rxFreq = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "QT/DQT解码")] [property: JsonProperty]
    private string _qtDec = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "发射频率")] [property: JsonProperty]
    private string _txFreq = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "QT/DQT编码")] [property: JsonProperty]
    private string _qtEnc = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "功率")] [property: JsonProperty]
    private string _txPwr = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "带宽")] [property: JsonProperty]
    private string _bandWidth = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "PTT-ID")] [property: JsonProperty]
    private string _pttid = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "繁忙锁定")] [property: JsonProperty]
    private string _busyLock = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "扫描添加")] [property: JsonProperty]
    private string _scanAdd = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "信令码")] [property: JsonProperty]
    private string _sigCode = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "信道名称")] [property: JsonProperty]
    private string _chanName = "";

    [ObservableProperty] [property: EpplusTableColumn(Header = "加密")] [property: JsonProperty]
    private string _encrypt = "";


    [ObservableProperty] [property: EpplusIgnore] [property: JsonIgnore]
    private bool _isVisable;

#pragma warning restore CS0657
}