using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using OfficeOpenXml;
using SenhaixFreqWriter.Constants.Shx8800Pro;
using SenhaixFreqWriter.DataModels.Interfaces;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public class AppData : IBackupable
{
    [JsonIgnore] public static AppData Instance;

    public string[] BankName = new string[8]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八"
    }; //

    public Channel[][] ChannelList = new Channel[8][]; //
    public Dtmf Dtmfs = new(); //
    public FmChannel Fms = new(); //
    public Function FunCfgs = new(); //
    public Mdc1200 Mdcs = new(); //
    public VfoInfos Vfos = new(); //

    public AppData()
    {
        for (var i = 0; i < 8; i++)
        {
            ChannelList[i] = new Channel[64];
            for (var j = 0; j < 64; j++)
            {
                var rmp = new Channel();
                rmp.Id = j + 1;
                ChannelList[i][j] = rmp;
            }
        }
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new JsonSerializer();
        serializer.Formatting = Formatting.Indented;
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            serializer.Serialize(streamWriter, Instance);
        }
    }

    public void SaveAsExcel(string filename)
    {
        // try
        // {
        if (File.Exists(filename)) File.Delete(filename);
        using var excelPack = new ExcelPackage(filename);
        for (var i = 0; i < ChannelList.Length; i++)
        {
            var transChannelList = ChannelList[i].Select(x => x.ToExcelChannel());
            // foreach (var excelChannel in transChannelList)
            // {
            //     Console.WriteLine(excelChannel.ToString());
            // }
            var ws = excelPack.Workbook.Worksheets.Add(BankName[i]);
            // Load the sample data into the worksheet
            ws.Cells["A1"].LoadFromCollection(transChannelList, options =>
            {
                options.PrintHeaders = true;
                // options.TableStyle = TableStyles.Dark1;
            });
            ws.Cells.AutoFitColumns();
            addValidationTo(ws, "F:F", ChanChoice.Power);
            addValidationTo(ws, "G:G", ChanChoice.Bandwidth);
            addValidationTo(ws, "H:H", ChanChoice.Scanadd);
            addValidationTo(ws, "I:I", ChanChoice.BusyLock);
            addValidationTo(ws, "J:J", ChanChoice.Pttid);
            addValidationTo(ws, "K:K", ChanChoice.SigGrp);
        }

        excelPack.Save();
        // }
        // catch(Exception ex)
        // {
        //     DebugWindow.GetInstance().UpdateDebugContent($"Failed to read from excel: {ex.Message}");
        // }
    }

    private void addValidationTo(ExcelWorksheet ws, string range, IEnumerable<string> target)
    {
        var validation = ws.DataValidations.AddListValidation(range);
        foreach (var se in target) validation.Formula.Values.Add(se);
        // validation.ShowErrorMessage = true;
        // validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
        // validation.ErrorTitle = "无效值";
        // validation.Error = "请在下拉框中选择！";
    }

    public void LoadFromExcel(string filename)
    {
        // try
        // {
        if (!File.Exists(filename)) return;
        using var excelPack = new ExcelPackage(filename);
        // _ = excelPack.Workbook.Worksheets[0];
        // _ = excelPack.Workbook.Worksheets[1];
        // _ = excelPack.Workbook.Worksheets[2];
        for (var i = 0; i < ChannelList.Length; i++)
        {
            var book = excelPack.Workbook.Worksheets[i]; //.Cells["A1:N129"].ToCollection<Channel>();
            // Console.WriteLine(book.Name);
            BankName[i] = book.Name;
            var res = book.Cells["A1:L65"].ToCollectionWithMappings<Channel>(
                row =>
                {
                    var channel = new Channel();
                    channel.Id = row.GetValue<int>(0);
                    channel.RxFreq = row.GetValue<string>(1);
                    channel.StrRxCtsDcs = row.GetValue<string>(2);
                    channel.TxFreq = row.GetValue<string>(3);
                    channel.StrTxCtsDcs = row.GetValue<string>(4);
                    channel.TxPower = ChanChoice.Power.IndexOf(row.GetValue<string>(5));
                    channel.Bandwide = ChanChoice.Bandwidth.IndexOf(row.GetValue<string>(6));
                    channel.ScanAdd = ChanChoice.Scanadd.IndexOf(row.GetValue<string>(7));
                    channel.BusyLock = ChanChoice.BusyLock.IndexOf(row.GetValue<string>(8));
                    channel.Pttid = ChanChoice.Pttid.IndexOf(row.GetValue<string>(9));
                    channel.SignalGroup = ChanChoice.SigGrp.IndexOf(row.GetValue<string>(10));
                    channel.Name = row.GetValue<string>(11);

                    channel.IsVisable = !string.IsNullOrEmpty(channel.RxFreq);
                    return channel;
                },
                options => options.HeaderRow = 0);
            // foreach (var t in res)
            // {
            //     if (!string.IsNullOrEmpty(t.RxFreq))t.IsVisable = true;
            // }

            ChannelList[i] = res.ToArray();
        }
        // }
        // catch(Exception ex)
        // {
        //     DebugWindow.GetInstance().UpdateDebugContent($"Failed to load from excel: {ex.Message}");
        // }
    }

    public static AppData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new AppData();
        return Instance;
    }

    public static AppData ForceNewInstance()
    {
        Instance = new AppData();
        return Instance;
    }


    public static void CreatObjFromFile(Stream s)
    {
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var res = streamReader.ReadToEnd();
            AppData tmp;
            try
            {
                var jsonSerializer = new JsonSerializer();
                var stringReader = new JsonTextReader(new StringReader(res));
                tmp = jsonSerializer.Deserialize<AppData>(stringReader);
                Instance.Dtmfs = tmp.Dtmfs;
                Instance.FunCfgs = tmp.FunCfgs;
                Instance.Fms = tmp.Fms;
                Instance.Mdcs = tmp.Mdcs;
                Instance.Vfos = tmp.Vfos;
                Instance.BankName = tmp.BankName;
                Instance.ChannelList = tmp.ChannelList;
            }
            catch (Exception e)
            {
                // DebugWindow.GetInstance().updateDebugContent(e.Message);
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
            }
        }
    }
}