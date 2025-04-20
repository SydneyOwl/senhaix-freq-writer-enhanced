using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using OfficeOpenXml;
using SenhaixFreqWriter.DataModels.Interfaces;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.DataModels.Gt12;

public class AppData : IBackupable
{
    [JsonIgnore] public static AppData Instance;

    public string[] BankName = new string[30]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八", "区域九", "区域十",
        "区域十一", "区域十二", "区域十三", "区域十四", "区域十五", "区域十六", "区域十七", "区域十八", "区域十九", "区域二十",
        "区域二十一", "区域二十二", "区域二十三", "区域二十四", "区域二十五", "区域二十六", "区域二十七", "区域二十八", "区域二十九", "区域三十"
    };

    public Channel[][] ChannelList = new Channel[30][];
    public Dtmf Dtmfs = new();
    public FmChannel Fms = new();
    public Function FunCfgs = new();
    public Mdc1200 Mdcs = new();
    public VfoInfos Vfos = new();

    public AppData()
    {
        for (var i = 0; i < 30; i++)
        {
            ChannelList[i] = new Channel[32];
            for (var j = 0; j < 32; j++)
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
                var ws = excelPack.Workbook.Worksheets.Add(BankName[i]);
                var excelList = ChannelList[i].Select(x => x.ToExcelChannel());
                // Load the sample data into the worksheet
                ws.Cells["A1"].LoadFromCollection(excelList, options =>
                {
                    options.PrintHeaders = true;
                    // options.TableStyle = TableStyles.Dark1;
                });
                
                ws.Cells.AutoFitColumns();
                
                addValidationTo(ws,"F:F",Constants.Gt12.ChanChoice.Power);
                addValidationTo(ws,"G:G",Constants.Gt12.ChanChoice.Bandwidth);
                addValidationTo(ws,"H:H",Constants.Gt12.ChanChoice.Scanadd);
                addValidationTo(ws,"I:I",Constants.Gt12.ChanChoice.SigSys);
                addValidationTo(ws,"J:J",Constants.Gt12.ChanChoice.Sql);
                addValidationTo(ws,"K:K",Constants.Gt12.ChanChoice.Pttid);
                addValidationTo(ws,"L:L",Constants.Gt12.ChanChoice.SigGrp);
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
        foreach (var se in target)
        {
            validation.Formula.Values.Add(se);
        }
        // validation.ShowErrorMessage = true;
        // validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
        // validation.ErrorTitle = "无效值";
        // validation.Error = "请在下拉框中选择！";
    }

    public void LoadFromExcel(string filename)
    {
        // try
        // {
            if (!File.Exists(filename))return;
            using var excelPack = new ExcelPackage(filename);
            for (var i = 0; i < ChannelList.Length; i++)
            {
                var book = excelPack.Workbook.Worksheets[i];//.Cells["A1:N129"].ToCollection<Channel>();
                // Console.WriteLine(book.Name);
                BankName[i] = book.Name;
                var res = book.Cells["A1:M33"].ToCollectionWithMappings<Channel>(
                    row => 
                    {
                        var channel = new Channel();
                        channel.Id = row.GetValue<int>(0);
                        channel.RxFreq = row.GetValue<string>(1);
                        channel.StrRxCtsDcs = row.GetValue<string>(2);
                        channel.TxFreq = row.GetValue<string>(3);
                        channel.StrTxCtsDcs = row.GetValue<string>(4);
                        channel.TxPower = Constants.Gt12.ChanChoice.Power.IndexOf(row.GetValue<string>(5));
                        channel.Bandwide =Constants.Gt12.ChanChoice.Bandwidth.IndexOf(row.GetValue<string>(6));
                        channel.ScanAdd = Constants.Gt12.ChanChoice.Scanadd.IndexOf(row.GetValue<string>(7));
                        channel.SignalSystem = Constants.Gt12.ChanChoice.SigSys.IndexOf(row.GetValue<string>(8));
                        channel.SqMode = Constants.Gt12.ChanChoice.Sql.IndexOf(row.GetValue<string>(9));
                        channel.Pttid = Constants.Gt12.ChanChoice.Pttid.IndexOf(row.GetValue<string>(10));
                        channel.SignalGroup = Constants.Gt12.ChanChoice.SigGrp.IndexOf(row.GetValue<string>(11));
                        channel.Name = row.GetValue<string>(12);
                        
                        
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