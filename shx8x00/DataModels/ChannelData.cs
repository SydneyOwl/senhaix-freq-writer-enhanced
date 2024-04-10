using System;

namespace shx8x00.DataModels;

[Serializable]
public class ChannelData
{
    private string bandWidth;
    private string busyLock;
    private string chanName;
    private string chanNum;
    private string encrypt;
    private string pttid;
    private string QTDec;
    private string QTEnc;
    private string rxFreq;
    private string scanAdd;
    private string sigCode;
    private string txAllow;
    private string txFreq;
    private string txPwr;

    public override string ToString()
    {
        return $"{nameof(bandWidth)}: {bandWidth}, {nameof(busyLock)}: {busyLock}, {nameof(chanName)}: {chanName}, {nameof(chanNum)}: {chanNum}, {nameof(encrypt)}: {encrypt}, {nameof(pttid)}: {pttid}, {nameof(QTDec)}: {QTDec}, {nameof(QTEnc)}: {QTEnc}, {nameof(rxFreq)}: {rxFreq}, {nameof(scanAdd)}: {scanAdd}, {nameof(sigCode)}: {sigCode}, {nameof(txAllow)}: {txAllow}, {nameof(txFreq)}: {txFreq}, {nameof(txPwr)}: {txPwr}, {nameof(ChanNum)}: {ChanNum}, {nameof(TxAllow)}: {TxAllow}, {nameof(RxFreq)}: {RxFreq}, {nameof(QtDec)}: {QtDec}, {nameof(TxFreq)}: {TxFreq}, {nameof(QtEnc)}: {QtEnc}, {nameof(TxPwr)}: {TxPwr}, {nameof(BandWidth)}: {BandWidth}, {nameof(Pttid)}: {Pttid}, {nameof(BusyLock)}: {BusyLock}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(SigCode)}: {SigCode}, {nameof(ChanName)}: {ChanName}, {nameof(Encrypt)}: {Encrypt}";
    }

    public bool allEmpty()
    {
        return string.IsNullOrEmpty(bandWidth) && string.IsNullOrEmpty(busyLock) && string.IsNullOrEmpty(chanName) &&
               string.IsNullOrEmpty(encrypt) && string.IsNullOrEmpty(pttid) && string.IsNullOrEmpty(QtDec) &&
               string.IsNullOrEmpty(QtEnc) && string.IsNullOrEmpty(scanAdd) && string.IsNullOrEmpty(sigCode) &&
               string.IsNullOrEmpty(txAllow)
               && string.IsNullOrEmpty(txFreq) && string.IsNullOrEmpty(txPwr);
    }

    public string ChanNum
    {
        get => chanNum;
        set => chanNum = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string TxAllow
    {
        get => txAllow;
        set => txAllow = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string RxFreq
    {
        get => rxFreq;
        set => rxFreq = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string QtDec
    {
        get => QTDec;
        set => QTDec = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string TxFreq
    {
        get => txFreq;
        set => txFreq = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string QtEnc
    {
        get => QTEnc;
        set => QTEnc = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string TxPwr
    {
        get => txPwr;
        set => txPwr = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string BandWidth
    {
        get => bandWidth;
        set => bandWidth = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Pttid
    {
        get => pttid;
        set => pttid = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string BusyLock
    {
        get => busyLock;
        set => busyLock = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string ScanAdd
    {
        get => scanAdd;
        set => scanAdd = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string SigCode
    {
        get => sigCode;
        set => sigCode = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string ChanName
    {
        get => chanName;
        set => chanName = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Encrypt
    {
        get => encrypt;
        set => encrypt = value ?? throw new ArgumentNullException(nameof(value));
    }
}