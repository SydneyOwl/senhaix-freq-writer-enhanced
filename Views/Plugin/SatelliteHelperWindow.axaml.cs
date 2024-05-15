using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using Newtonsoft.Json.Linq;
using SenhaixFreqWriter.Constants.Common;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class SatelliteHelperWindow : Window
{
    private string[] currentChannel = new string[14];
    public ObservableCollection<string> namelist { get; set; } = new();

    public List<string> satelliteList = new List<string>();
    
    private JArray satelliteJson = new();

    public delegate void InsertChannelMethod(string rx, string rxDec, string tx, string txDec, string name);

    public InsertChannelMethod InsertData;
    public SatelliteHelperWindow(InsertChannelMethod func)
    {
        InitializeComponent();
        DataContext = this;
        loadJSON();
        InsertData = func;
    }
    
    private void loadJSON()
    {
        if (!File.Exists("./amsat-all-frequencies.json"))
        {
            selectedSatelliteInfo.Text = "未找到卫星数据,请点击更新星历！";
            return;
        }
        var satelliteData = File.ReadAllText("./amsat-all-frequencies.json");
        if (satelliteData == "")
        {
            selectedSatelliteInfo.Text = "卫星数据无效,请点击更新星历！";
            return;
        }

        try
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                selectedSatelliteInfo.Text = "";
                modeListBox.Items.Clear();
            });
            satelliteList.Clear();
            namelist.Clear();
            List<string> nameListCache = new List<string>();
            satelliteJson = JArray.Parse(satelliteData);
            foreach (var b in satelliteJson) nameListCache.Add((string)b["name"]);
            nameListCache = nameListCache.Distinct().ToList();
            foreach (var se in nameListCache)
            {
                satelliteList.Add(se);
                namelist.Add(se);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            selectedSatelliteInfo.Text = "卫星数据无效,请点击更新星历！";
            return;
        }
    }

    private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var current = ((TextBox)sender).Text;
        var query = from item in satelliteList where item.ToLower().Contains(current.ToLower()) select item;
        namelist.Clear();
        modeListBox.Selection.Clear();
        modeListBox.Items.Clear();
        selectedSatelliteInfo.Text = "";
        foreach (var a in query) namelist.Add(a);
    }

    private void SatListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (satListBox.SelectedItem == null) return;
        modeListBox.SelectedItems?.Clear();
        // listBox1.SelectedItem
        // throw new System.NotImplementedException();
        var current = satListBox.SelectedItem.ToString();
        var query = from item in satelliteJson where (string)item["name"] == current select item;
        modeListBox.Selection.Clear();
        modeListBox.Items.Clear();
        selectedSatelliteInfo.Text = current;
        foreach (var a in query)
            if (a["mode"].Value<string>() != null)
                modeListBox.Items.Add(a["mode"].ToString());
    }

    private void ModeListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        insertChannel.IsEnabled = modeListBox.SelectedItem != null;
        if (satListBox.SelectedItem == null) return;
        var currentSat = satListBox.SelectedItem.ToString();
        var currentMode = modeListBox.SelectedItem?.ToString();
        var query = from item in satelliteJson
            where (string)item["name"] == currentSat && (string)item["mode"] == currentMode
            select item;
        if (!query.Any()){return;}
        var sat = query.First()["name"];
        var uplink = query.First()["uplink"];
        var downlink = query.First()["downlink"];
        var callsign = query.First()["callsign"];
        var status = query.First()["status"];
        var satnogId = query.First()["satnogs_id"];
        var tone = matchTone(checker(query.First()["mode"]));
        selectedSatelliteInfo.Text = string.Format("上行频率:{0}\n下行频率：{1}\n亚音：{2}\n呼号：{3}\n状态：{4}\nid:{5}", checker(uplink),
            checker(downlink), tone, checker(callsign), checker(status), checker(satnogId));
        // 
        currentChannel = new string[14]
        {
            "", "Yes", checker(downlink), "OFF", checker(uplink), tone, "H", "W", "OFF", "OFF",
            "ON", "1", checker(sat), "OFF"
        };
    }
    
    private string matchTone(string mode)
    {
        var pattern = @"(?i)\b(?:tone|ctcss)\s+(\d+(?:\.\d+)?)?(?=hz\b)";

        var regex = new Regex(pattern);

        var match = regex.Match(mode);

        if (match.Success)
            // 返回匹配的浮点数部分
            return match.Groups[1].Value;
        return "";
    }
    
    private string checker(IEnumerable<JToken> res)
    {
        return res.Value<string>() == null ? "" : res.ToString();
    }

    private void FetchSatDataButton_OnClick(object? sender, RoutedEventArgs e)
    {
        new Task(()=>{fetchData();}).Start();
    }

    private void fetchData ()
    {
        var url =
            "https://raw.githubusercontent.com/palewire/amateur-satellite-database/main/data/amsat-all-frequencies.json";
        var proxyPrefix = "https://mirror.ghproxy.com/";
        try
        {
            Dispatcher.UIThread.Post(() =>
            {
                FetchSatText.Text = "更新中...";
                FetchSat.IsEnabled = false;
            });
            new WebClient().DownloadFile(url, "./amsat-all-frequencies.json");
            loadJSON();
            Dispatcher.UIThread.Post(() =>
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
            });
        }
        catch (Exception w)
        {
            url = proxyPrefix + url;
            try
            {
                new WebClient().DownloadFile(url, "./amsat-all-frequencies.json");
                loadJSON();
                Dispatcher.UIThread.Post(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
                });
            }
            catch
            {
                Dispatcher.UIThread.Post(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "更新失败....").ShowWindowDialogAsync(this);
                });
            }
        }
        finally
        {
            Dispatcher.UIThread.Post(() =>
            {
                FetchSat.IsEnabled = true;
                FetchSatText.Text = "更新星历";
            });
            // Console.WriteLine("Waiting...");
            // while (!handler.IsCompleted)
            // {
            //     Thread.Sleep(50);
            // }
        }
    }

    private void InsertChannelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // 直接从之前的winform移植来的，不想改了
        if (currentChannel[2] == "")
            if (checkIsFloatOrNumber(currentChannel[4]))
                currentChannel[2] = currentChannel[4];
        if (currentChannel[4] == "")
            if (checkIsFloatOrNumber(currentChannel[2]))
                currentChannel[4] = currentChannel[2];
        if (currentChannel[5] == "") currentChannel[5] = "OFF";

        if (int.TryParse(currentChannel[5], out var number)) currentChannel[5] = number + ".0";
        
        if (!(checkIsFloatOrNumber(currentChannel[2]) &&
              checkIsFloatOrNumber(currentChannel[4]) && CheckTones(currentChannel[5])))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "当前卫星不支持插入！").ShowWindowDialogAsync(this);
            return;
        }
        double.TryParse(currentChannel[2], out var kk);
        currentChannel[2] = kk.ToString("0.00000");

        double.TryParse(currentChannel[4], out kk);
        currentChannel[4] = kk.ToString("0.00000");

        try
        {
            if (!doppler.IsChecked.Value)
            {
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                MessageBoxManager.GetMessageBoxStandard("注意", "插入成功！").ShowWindowDialogAsync(this);
            }
            else
            {
                var uChg = uoffset.Text;
                var vChg = voffset.Text;
                int parsedVnumber, parsedUnumber;
                var a = int.TryParse(vChg, out parsedVnumber);
                var b = int.TryParse(uChg, out parsedUnumber);
                if (!(a && b))
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "多普勒数值有误！").ShowWindowDialogAsync(this);
                    return;
                }
                var tmp = (string[])currentChannel.Clone(); 
                currentChannel[2] = calcDop(double.Parse(currentChannel[2]), parsedUnumber, parsedVnumber, -2, 1)
                    .ToString("0.00000");
                currentChannel[4] = calcDop(double.Parse(currentChannel[4]), parsedUnumber, parsedVnumber, -2, 0)
                    .ToString("0.00000");
                currentChannel[12] += "-A1";
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                currentChannel = (string[])tmp.Clone();
                currentChannel[2] = calcDop(double.Parse(currentChannel[2]), parsedUnumber, parsedVnumber, -1, 1)
                    .ToString("0.00000");
                currentChannel[4] = calcDop(double.Parse(currentChannel[4]), parsedUnumber, parsedVnumber, -1, 0)
                    .ToString("0.00000");
                currentChannel[12] += "-A2";
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                currentChannel = (string[])tmp.Clone();
                currentChannel[2] = calcDop(double.Parse(currentChannel[2]), parsedUnumber, parsedVnumber, 0, 1)
                    .ToString("0.00000");
                currentChannel[4] = calcDop(double.Parse(currentChannel[4]), parsedUnumber, parsedVnumber, 0, 0)
                    .ToString("0.00000");
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                currentChannel = (string[])tmp.Clone();
                currentChannel[2] = calcDop(double.Parse(currentChannel[2]), parsedUnumber, parsedVnumber, 1, 1)
                    .ToString("0.00000");
                currentChannel[4] = calcDop(double.Parse(currentChannel[4]), parsedUnumber, parsedVnumber, 1, 0)
                    .ToString("0.00000");
                currentChannel[12] += "-L1";
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                currentChannel = (string[])tmp.Clone();
                currentChannel[2] = calcDop(double.Parse(currentChannel[2]), parsedUnumber, parsedVnumber, 2, 1)
                    .ToString("0.00000");
                currentChannel[4] = calcDop(double.Parse(currentChannel[4]), parsedUnumber, parsedVnumber, 2, 0)
                    .ToString("0.00000");
                currentChannel[12] += "-L2";
                InsertData(currentChannel[2], currentChannel[3], currentChannel[4], currentChannel[5],
                    currentChannel[12]);
                MessageBoxManager.GetMessageBoxStandard("注意", "插入成功！").ShowWindowDialogAsync(this);
            }
        }
        catch (Exception ae)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", $"出错:{ae.Message}！").ShowWindowDialogAsync(this);
        }
    }
    private bool checkIsFloatOrNumber(string value)
    {
        int tempInt;
        if (int.TryParse(value, out tempInt)) return true;

        // 浮点数验证
        double tempDouble;
        if (double.TryParse(value, out tempDouble)) return true;

        // 如果既不是整数也不是浮点数，则返回false
        return false;
    }

    private bool CheckTones(string value)
    {
        var ctcss = new[]
        {
            "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
            "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
            "123.0", "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "162.2", "167.9",
            "173.8", "179.9", "186.2", "192.8", "203.5", "210.7", "218.1", "225.7", "229.1", "233.6",
            "241.8", "250.3"
        };
        return ctcss.Contains(value);
    }
    private double calcDop(double band, int U_step, int V_step, int level, int direction)
        //direction==0: uplink
    {
        // level should be -2~~2
        if (band < 300)
        {
            // V band
            if (direction == 1)
                return band - 0.0005 * V_step * level;
            return band + 0.0005 * V_step * level;
        }

        if (direction == 1)
            return band - 0.0005 * U_step * level;
        return band + 0.0005 * U_step * level;
    }
}