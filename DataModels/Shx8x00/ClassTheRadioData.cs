using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using OfficeOpenXml;
using SenhaixFreqWriter.DataModels.Interfaces;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public class ClassTheRadioData : IBackupable
{
    [JsonIgnore] public static ClassTheRadioData Instance;

    //TODO 无法直接反序列化到chanData, 只能这样一下
    public List<ChannelData> ChanneldataList = new();

    public DtmfData DtmfData = new();

    public FunCfgData FunCfgData = new();

    [JsonIgnore] public ObservableCollection<ChannelData> ObsChanData = new();

    public OtherImfData OtherImfData = new();

    public ClassTheRadioData()
    {
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            ObsChanData.Add(data);
        }
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new JsonSerializer();
        serializer.Formatting = Formatting.Indented;
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            Instance.ChanneldataList = Instance.ObsChanData.ToList();
            serializer.Serialize(streamWriter, Instance);
        }
    }

    public void SaveAsExcel(string filename)
    {
        try
        {
            if (File.Exists(filename)) File.Delete(filename);
            using var excelPack = new ExcelPackage(filename);
            var ws = excelPack.Workbook.Worksheets.Add("信道信息");
            // Load the sample data into the worksheet
            var range = ws.Cells["A1"].LoadFromCollection(ObsChanData, options =>
            {
                options.PrintHeaders = true;
                // options.TableStyle = TableStyles.Dark1;
            });
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
            var parsed = excelPack.Workbook.Worksheets[0].Cells["A1:N129"].ToCollection<ChannelData>();
            ObsChanData.Clear();
            foreach (var channelData in parsed)
            {
                channelData.IsVisable = !channelData.AllEmpty();
                ObsChanData.Add(channelData.DeepCopy());
                Console.WriteLine(channelData.ToString());
            }
        }
        catch(Exception ex)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"Failed to load from excel: {ex.Message}");
        }
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
        }).SaveToText(new FileInfo(filename),new ExcelOutputTextFormat { Delimiter = ';'});
        excelPack.Save();
    }



    public static void CreatObjFromFile(Stream s)
    {
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var res = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            try
            {
                var jsonSerializer = new JsonSerializer();
                var stringReader = new JsonTextReader(new StringReader(res));
                tmp = jsonSerializer.Deserialize<ClassTheRadioData>(stringReader);
                Instance.FunCfgData = tmp.FunCfgData;
                Instance.DtmfData = tmp.DtmfData;
                Instance.OtherImfData = tmp.OtherImfData;
                Instance.ObsChanData.Clear();
                foreach (var cd in tmp.ChanneldataList)
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
        return Instance;
    }

    public void ForceNewChannel()
    {
        Instance.ObsChanData.Clear();
        for (var i = 0; i < 128; i++)
        {
            var chan = new ChannelData();
            chan.ChanNum = i.ToString();
            chan.IsVisable = false;
            Instance.ObsChanData.Add(chan);
        }
    }
}