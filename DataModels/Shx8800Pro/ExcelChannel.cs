using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public class ExcelChannel
{
  [EpplusTableColumn(Header = "信道")]public string Id { get; set; } = ""; //
  [EpplusTableColumn(Header = "接收频率")] public string RxFreq  { get; set; } = ""; //
  [EpplusTableColumn(Header = "亚音解码")] public string StrRxCtsDcs { get; set; } = "OFF"; //
  [EpplusTableColumn(Header = "发射频率")] public string TxFreq { get; set; } = ""; //
  [EpplusTableColumn(Header = "亚音编码")]  public string StrTxCtsDcs { get; set; } = "OFF"; //
  [EpplusTableColumn(Header = "功率")] public string TxPower { get; set; } = ""; //
  [EpplusTableColumn(Header = "带宽")] public string Bandwide { get; set; } = ""; //
  [EpplusTableColumn(Header = "扫描添加")] public string ScanAdd { get; set; } = ""; //
  [EpplusTableColumn(Header = "繁忙锁定")] public string BusyLock { get; set; } = "";
  [EpplusTableColumn(Header = "PTT-ID")] public string Pttid { get; set; } = ""; //
  [EpplusTableColumn(Header = "信令码")] public string SignalGroup { get; set; } = ""; //
  [EpplusTableColumn(Header = "信道名称")] public string Name { get; set; } = ""; //
  [EpplusIgnore]public bool IsVisable;

  public override string ToString()
  {
    return
      $"{nameof(Id)}: {Id}, {nameof(RxFreq)}: {RxFreq}, {nameof(StrRxCtsDcs)}: {StrRxCtsDcs}, {nameof(TxFreq)}: {TxFreq}, {nameof(StrTxCtsDcs)}: {StrTxCtsDcs}, {nameof(TxPower)}: {TxPower}, {nameof(Bandwide)}: {Bandwide}, {nameof(ScanAdd)}: {ScanAdd}, {nameof(BusyLock)}: {BusyLock}, {nameof(Pttid)}: {Pttid}, {nameof(SignalGroup)}: {SignalGroup}, {nameof(Name)}: {Name}, {nameof(IsVisable)}: {IsVisable}";
  }
}