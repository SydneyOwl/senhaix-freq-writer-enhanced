using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace shx8x00.DataModels;

[Serializable]
public partial class ChannelData : ObservableObject
{
    [ObservableProperty]
    private string bandWidth = "";
    [ObservableProperty]
    private string busyLock ;
    [ObservableProperty]
    private string chanName = "";
    [ObservableProperty]
    private string chanNum = "";
    [ObservableProperty]
    private string encrypt = "";
    [ObservableProperty]
    private string pttid = "";
    [ObservableProperty]
    private string qtDec = "";
    [ObservableProperty]
    private string qtEnc = "";
    [ObservableProperty]
    private string rxFreq = "";
    [ObservableProperty]
    private string scanAdd = "";
    [ObservableProperty]
    private string sigCode = "";
    [ObservableProperty]
    private string txAllow = "";
    [ObservableProperty]
    private string txFreq = "";
    [ObservableProperty]
    private string txPwr = "";

    public override string ToString()
    {
        return $"{nameof(bandWidth)}: {bandWidth}, {nameof(busyLock)}: {busyLock}, {nameof(chanName)}: {chanName}, {nameof(chanNum)}: {chanNum}, {nameof(encrypt)}: {encrypt}, {nameof(pttid)}: {pttid}, {nameof(qtDec)}: {qtDec}, {nameof(qtEnc)}: {qtEnc}, {nameof(rxFreq)}: {rxFreq}, {nameof(scanAdd)}: {scanAdd}, {nameof(sigCode)}: {sigCode}, {nameof(txAllow)}: {txAllow}, {nameof(txFreq)}: {txFreq}, {nameof(txPwr)}: {txPwr}, {nameof(BandWidth)}: {BandWidth}, {nameof(BusyLock)}: {BusyLock}, {nameof(ChanName)}: {ChanName}, {nameof(ChanNum)}: {ChanNum}, {nameof(Encrypt)}: {Encrypt}, {nameof(Pttid)}: {Pttid}, {nameof(QtDec)}: {QtDec}, {nameof(QtEnc)}: {QtEnc}, {nameof(RxFreq)}: {RxFreq}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(SigCode)}: {SigCode}, {nameof(TxAllow)}: {TxAllow}, {nameof(TxFreq)}: {TxFreq}, {nameof(TxPwr)}: {TxPwr}";
    }

    public bool allEmpty()
    {
        return string.IsNullOrEmpty(bandWidth) && string.IsNullOrEmpty(busyLock) && string.IsNullOrEmpty(chanName) &&
               string.IsNullOrEmpty(encrypt) && string.IsNullOrEmpty(pttid) && string.IsNullOrEmpty(qtDec) &&
               string.IsNullOrEmpty(qtEnc) && string.IsNullOrEmpty(scanAdd) && string.IsNullOrEmpty(sigCode) &&
               string.IsNullOrEmpty(txAllow)
               && string.IsNullOrEmpty(txFreq) && string.IsNullOrEmpty(txPwr);
    }
}