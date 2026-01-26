using OfficeOpenXml.Attributes;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class ExcelChannel
{
    [EpplusIgnore] public bool IsVisable;
    [EpplusTableColumn(Header = "信道")] public string Id { get; set; } = "";
    [EpplusTableColumn(Header = "接收频率")] public string RxFreq { get; set; } = "";
    [EpplusTableColumn(Header = "亚音解码")] public string StrRxCtsDcs { get; set; } = "OFF";
    [EpplusTableColumn(Header = "发射频率")] public string TxFreq { get; set; } = "";
    [EpplusTableColumn(Header = "亚音编码")] public string StrTxCtsDcs { get; set; } = "OFF";
    [EpplusTableColumn(Header = "功率")] public string TxPower { get; set; } = "";
    [EpplusTableColumn(Header = "带宽")] public string Bandwide { get; set; } = "";
    [EpplusTableColumn(Header = "扫描添加")] public string ScanAdd { get; set; } = "";
    [EpplusTableColumn(Header = "信令")] public string SignalSystem { get; set; } = "";
    [EpplusTableColumn(Header = "静音模式")] public string SqMode { get; set; } = "";
    [EpplusTableColumn(Header = "PTT-ID")] public string Pttid { get; set; } = "";
    [EpplusTableColumn(Header = "信令码")] public string SignalGroup { get; set; } = "";
    [EpplusTableColumn(Header = "信道名称")] public string Name { get; set; } = "";
}