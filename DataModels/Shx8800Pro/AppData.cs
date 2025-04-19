using System;
using System.IO;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using OfficeOpenXml;
using SenhaixFreqWriter.DataModels.Interfaces;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Views.Common;

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
        try
        {
            if (File.Exists(filename)) File.Delete(filename);
            using var excelPack = new ExcelPackage(filename);
            for (var i = 0; i < ChannelList.Length; i++)
            {
                var ws = excelPack.Workbook.Worksheets.Add(BankName[i]);
                // Load the sample data into the worksheet
                ws.Cells["A1"].LoadFromCollection(ChannelList[i], options =>
                {
                    options.PrintHeaders = true;
                    // options.TableStyle = TableStyles.Dark1;
                });
            }
            excelPack.Save();
        }
        catch(Exception ex)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"Failed to read from excel: {ex.Message}");
        }
    }

    public void LoadFromExcel(string filename)
    {
        try
        {
            if (!File.Exists(filename))return;
            using var excelPack = new ExcelPackage(filename);
            // _ = excelPack.Workbook.Worksheets[0];
            // _ = excelPack.Workbook.Worksheets[1];
            // _ = excelPack.Workbook.Worksheets[2];
            for (var i = 0; i < ChannelList.Length; i++)
            {
                var book = excelPack.Workbook.Worksheets[i];//.Cells["A1:N129"].ToCollection<Channel>();
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
                        channel.TxPower = row.GetValue<int>(5);
                        channel.Bandwide = row.GetValue<int>(6);
                        channel.ScanAdd = row.GetValue<int>(7);
                        channel.BusyLock = row.GetValue<int>(8);
                        channel.Pttid = row.GetValue<int>(9);
                        channel.SignalGroup = row.GetValue<int>(10);
                        channel.Name = row.GetValue<string>(11);

                        return channel;
                    }, 
                    options => options.HeaderRow = 0);
                foreach (var t in res)
                {
                    if (!string.IsNullOrEmpty(t.RxFreq))t.IsVisable = true;
                }

                ChannelList[i] = res.ToArray();
            }
        }
        catch(Exception ex)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"Failed to load from excel: {ex.Message}");
        }
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