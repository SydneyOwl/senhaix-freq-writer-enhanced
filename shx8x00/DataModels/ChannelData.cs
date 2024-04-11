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

    public void changeByNum(int index, string target)
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
    public string[] transList()
    {
        return new[]
        {
            chanNum, txAllow, rxFreq, qtDec, txFreq, qtEnc, txPwr,bandWidth, pttid, busyLock, scanAdd, sigCode, chanName, encrypt
        };
    }
    public override string ToString()
    {
        return $"{nameof(bandWidth)}: {bandWidth}, {nameof(busyLock)}: {busyLock}, {nameof(chanName)}: {chanName}, {nameof(chanNum)}: {chanNum}, {nameof(encrypt)}: {encrypt}, {nameof(pttid)}: {pttid}, {nameof(qtDec)}: {qtDec}, {nameof(qtEnc)}: {qtEnc}, {nameof(rxFreq)}: {rxFreq}, {nameof(scanAdd)}: {scanAdd}, {nameof(sigCode)}: {sigCode}, {nameof(txAllow)}: {txAllow}, {nameof(txFreq)}: {txFreq}, {nameof(txPwr)}: {txPwr}";
    }

    public bool allEmpty()
    {
        return string.IsNullOrEmpty(bandWidth) && string.IsNullOrEmpty(busyLock) && string.IsNullOrEmpty(chanName) &&
               string.IsNullOrEmpty(encrypt) && string.IsNullOrEmpty(pttid) && string.IsNullOrEmpty(qtDec) &&
               string.IsNullOrEmpty(qtEnc) && string.IsNullOrEmpty(scanAdd) && string.IsNullOrEmpty(sigCode) &&
               string.IsNullOrEmpty(txAllow)
               && string.IsNullOrEmpty(txFreq) && string.IsNullOrEmpty(txPwr);
    }
    public bool filled()
    {
        return !string.IsNullOrEmpty(bandWidth) && !string.IsNullOrEmpty(busyLock) && !string.IsNullOrEmpty(chanName) &&
               !string.IsNullOrEmpty(encrypt) && !string.IsNullOrEmpty(pttid) && !string.IsNullOrEmpty(qtDec) &&
               !string.IsNullOrEmpty(qtEnc) && !string.IsNullOrEmpty(scanAdd) && !string.IsNullOrEmpty(sigCode) &&
               !string.IsNullOrEmpty(txAllow)
               && !string.IsNullOrEmpty(txFreq) && !string.IsNullOrEmpty(txPwr);
    }
}