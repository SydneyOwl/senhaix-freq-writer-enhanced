using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using OfficeOpenXml;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Interfaces;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public class ClassTheRadioData : IBackupable
{
    [JsonIgnore] public static ClassTheRadioData Instance;

    public DtmfData DtmfData = new();

    public FunCfgData FunCfgData = new();

    public ObservableCollection<ChannelData> ObsChanData = new();

    public OtherImfData OtherImfData = new();

    public void SaveToFile(Stream s)
    {
        var serializer = new JsonSerializer();
        serializer.Formatting = Formatting.Indented;
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            serializer.Serialize(streamWriter, Instance);
        }
    }

    public void SaveAsExcel(string filename, ShxDevice device)
    {
        // try
        // {
        if (File.Exists(filename)) File.Delete(filename);
        using var excelPack = new ExcelPackage(filename);
        var ws = excelPack.Workbook.Worksheets.Add("信道信息");
        // Load the sample data into the worksheet
        var range = ws.Cells["A1"].LoadFromCollection(ObsChanData, options =>
        {
            options.PrintHeaders = true;
            // options.TableStyle = TableStyles.Dark1;
        });

        ws.Cells.AutoFitColumns();

        addValidationTo(ws, "B:B", ChanChoice.Txallow);
        // addValidationTo(ws, "D:D", Constants.Shx8x00.ChanChoice.Qtdqt);
        // addValidationTo(ws, "F:F", Constants.Shx8x00.ChanChoice.Qtdqt);
        addValidationTo(ws, "G:G", device == ShxDevice.Shx8600Pro ? ChanChoice.TxPwrNewShx8600 : ChanChoice.TxPwr);
        addValidationTo(ws, "H:H", ChanChoice.Bandwidth);
        addValidationTo(ws, "I:I", ChanChoice.Pttid);
        addValidationTo(ws, "J:J", ChanChoice.Busylock);
        addValidationTo(ws, "K:K", ChanChoice.Scanadd);
        addValidationTo(ws, "L:L", ChanChoice.SignCode);
        addValidationTo(ws, "N:N", ChanChoice.Encrypt);


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
        var parsed = excelPack.Workbook.Worksheets[0].Cells["A1:N129"].ToCollection<ChannelData>();
        ObsChanData.Clear();
        foreach (var channelData in parsed)
        {
            channelData.IsVisable = !channelData.AllEmpty();
            ObsChanData.Add(channelData.DeepCopy());
            // Console.WriteLine(channelData.ToString());
        }
        // }
        // catch(Exception ex)
        // {
        //     DebugWindow.GetInstance().UpdateDebugContent($"Failed to load from excel: {ex.Message}");
        // }
    }

    public void SaveAsCsv(string filename)
    {
        if (File.Exists(filename)) File.Delete(filename);
        using var excelPack = new ExcelPackage();
        var ws = excelPack.Workbook.Worksheets.Add("信道信息");
        // Load the sample data into the worksheet
        ws.Cells["A1"].LoadFromCollection(ObsChanData, options =>
        {
            options.PrintHeaders = true;
            // options.TableStyle = TableStyles.Dark1;
        }).SaveToText(new FileInfo(filename), new ExcelOutputTextFormat { Delimiter = ';' });
        excelPack.Save();
    }


    public static void CreatObjFromFile(Stream s)
    {
        Instance.ForceNewInstance();
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var res = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            try
            {
                tmp = JsonConvert.DeserializeObject<ClassTheRadioData>(res)!;
                Instance.FunCfgData = tmp.FunCfgData;
                Instance.DtmfData = tmp.DtmfData;
                Instance.OtherImfData = tmp.OtherImfData;

                Instance.ObsChanData.Clear();
                foreach (var cd in tmp.ObsChanData)
                {
                    if (!cd.AllEmpty()) cd.IsVisable = true;
                    Instance.ObsChanData.Add(cd);
                }
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
            }
        }
    }

    public static ClassTheRadioData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new ClassTheRadioData();
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            Instance.ObsChanData.Add(data);
        }

        return Instance;
    }

    public void ForceNewInstance()
    {
        Instance.ObsChanData.Clear();
        for (var i = 0; i < 128; i++)
        {
            var chan = new ChannelData();
            chan.ChanNum = i.ToString();
            chan.IsVisable = false;
            Instance.ObsChanData.Add(chan);
        }

        Instance.DtmfData = new DtmfData();
        Instance.FunCfgData = new FunCfgData();
        Instance.OtherImfData = new OtherImfData();
    }
}