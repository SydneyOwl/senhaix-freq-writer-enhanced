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
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class SatelliteHelperWindow : Window
{
    private string[] _currentChannel = new string[14];
    public ObservableCollection<string> Namelist { get; set; } = new();

    public List<string> SatelliteList = new();

    private JArray _satelliteJson = new();

    public delegate void InsertChannelMethod(string rx, string rxDec, string tx, string txDec, string name);

    public InsertChannelMethod InsertData;

    public SatelliteHelperWindow(InsertChannelMethod func)
    {
        InitializeComponent();
        InsertData = func;
        DataContext = this;
        Task.Run(() =>
        {
            if (!LoadJson())
            {
                Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "正在为您更新卫星数据...\n"; });
                FetchData();
            }
        });
    }

    private bool LoadJson()
    {
        if (!File.Exists("./amsat-all-frequencies.json"))
        {
            DebugWindow.GetInstance().updateDebugContent($"未找到json");
            Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "未找到卫星数据,请点击更新星历！\n"; });
            return false;
        }

        var satelliteData = File.ReadAllText("./amsat-all-frequencies.json");
        if (satelliteData == "")
        {        
            DebugWindow.GetInstance().updateDebugContent($"json为空");
            Dispatcher.UIThread.Invoke(() => { return selectedSatelliteInfo.Text += "卫星数据无效,请点击更新星历！\n"; });
            return false;
        }

        try
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                selectedSatelliteInfo.Text = "";
                modeListBox.Items.Clear();
            });
            SatelliteList.Clear();
            Namelist.Clear();
            List<string> nameListCache = new();
            _satelliteJson = JArray.Parse(satelliteData);
            foreach (var b in _satelliteJson) nameListCache.Add((string)b["name"]);
            nameListCache = nameListCache.Distinct().ToList();
            foreach (var se in nameListCache)
            {
                SatelliteList.Add(se);
                Namelist.Add(se);
            }
        }
        catch (Exception e)
        {
            DebugWindow.GetInstance().updateDebugContent($"ERR：{e.Message}");
            Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "卫星数据无效,请点击更新星历！\n"; });
            return false;
        }

        return true;
    }

    private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var current = ((TextBox)sender).Text;
        var query = from item in SatelliteList where item.ToLower().Contains(current.ToLower()) select item;
        Namelist.Clear();
        modeListBox.Selection.Clear();
        modeListBox.Items.Clear();
        selectedSatelliteInfo.Text = "";
        foreach (var a in query) Namelist.Add(a);
    }

    private void SatListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (satListBox.SelectedItem == null) return;
        modeListBox.SelectedItems?.Clear();
        // listBox1.SelectedItem
        // throw new System.NotImplementedException();
        var current = satListBox.SelectedItem.ToString();
        var query = from item in _satelliteJson where (string)item["name"] == current select item;
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
        var query = from item in _satelliteJson
            where (string)item["name"] == currentSat && (string)item["mode"] == currentMode
            select item;
        if (!query.Any()) return;
        var sat = query.First()["name"];
        var uplink = query.First()["uplink"];
        var downlink = query.First()["downlink"];
        var callsign = query.First()["callsign"];
        var status = query.First()["status"];
        var satnogId = query.First()["satnogs_id"];
        var tone = MatchTone(Checker(query.First()["mode"]));
        selectedSatelliteInfo.Text = string.Format("上行频率:{0}\n下行频率：{1}\n亚音：{2}\n呼号：{3}\n状态：{4}\nid:{5}",
            Checker(uplink),
            Checker(downlink), tone, Checker(callsign), Checker(status), Checker(satnogId));
        // 
        _currentChannel = new string[14]
        {
            "", "Yes", Checker(downlink), "OFF", Checker(uplink), tone, "H", "W", "OFF", "OFF",
            "ON", "1", Checker(sat), "OFF"
        };
    }

    private string MatchTone(string mode)
    {
        var pattern = @"(?i)\b(?:tone|ctcss)\s+(\d+(?:\.\d+)?)?(?=hz\b)";

        var regex = new Regex(pattern);

        var match = regex.Match(mode);

        if (match.Success)
            // 返回匹配的浮点数部分
            return match.Groups[1].Value;
        return "";
    }

    private string Checker(IEnumerable<JToken> res)
    {
        return res.Value<string>() == null ? "" : res.ToString();
    }

    private void FetchSatDataButton_OnClick(object? sender, RoutedEventArgs e)
    {
        new Task(() => { FetchData(); }).Start();
    }

    private void FetchData()
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
            LoadJson();
            Dispatcher.UIThread.Post(() =>
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
            });
        }
        catch (Exception w)
        {
            DebugWindow.GetInstance().updateDebugContent($"下载出错：{w.Message}，ppxy...");
            Dispatcher.UIThread.Post(() => { FetchSatText.Text = $"出错：{w.Message},重试中..."; });
            url = proxyPrefix + url;
            try
            {
                new WebClient().DownloadFile(url, "./amsat-all-frequencies.json");
                LoadJson();
                Dispatcher.UIThread.Post(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
                });
            }
            catch (Exception a)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    FetchSatText.Text = $"出错：{a.Message}...";
                    DebugWindow.GetInstance().updateDebugContent($"下载出错：{w.Message}");
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
        ModeListBox_OnSelectionChanged(null, null);
        // 直接从之前的winform移植来的，不想改了
        if (_currentChannel[2] == "")
            if (CheckIsFloatOrNumber(_currentChannel[4]))
                _currentChannel[2] = _currentChannel[4];
        if (_currentChannel[4] == "")
            if (CheckIsFloatOrNumber(_currentChannel[2]))
                _currentChannel[4] = _currentChannel[2];
        if (_currentChannel[5] == "") _currentChannel[5] = "OFF";

        if (int.TryParse(_currentChannel[5], out var number)) _currentChannel[5] = number + ".0";

        if (!(CheckIsFloatOrNumber(_currentChannel[2]) &&
              CheckIsFloatOrNumber(_currentChannel[4]) && CheckTones(_currentChannel[5])))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "当前卫星不支持插入！").ShowWindowDialogAsync(this);
            return;
        }

        double.TryParse(_currentChannel[2], out var kk);
        _currentChannel[2] = kk.ToString("0.00000");

        double.TryParse(_currentChannel[4], out kk);
        _currentChannel[4] = kk.ToString("0.00000");

        try
        {
            if (!doppler.IsChecked.Value)
            {
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
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

                var tmp = (string[])_currentChannel.Clone();
                _currentChannel[2] = CalcDop(double.Parse(_currentChannel[2]), parsedUnumber, parsedVnumber, -2, 1)
                    .ToString("0.00000");
                _currentChannel[4] = CalcDop(double.Parse(_currentChannel[4]), parsedUnumber, parsedVnumber, -2, 0)
                    .ToString("0.00000");
                _currentChannel[12] += "-A1";
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
                _currentChannel = (string[])tmp.Clone();
                _currentChannel[2] = CalcDop(double.Parse(_currentChannel[2]), parsedUnumber, parsedVnumber, -1, 1)
                    .ToString("0.00000");
                _currentChannel[4] = CalcDop(double.Parse(_currentChannel[4]), parsedUnumber, parsedVnumber, -1, 0)
                    .ToString("0.00000");
                _currentChannel[12] += "-A2";
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
                _currentChannel = (string[])tmp.Clone();
                _currentChannel[2] = CalcDop(double.Parse(_currentChannel[2]), parsedUnumber, parsedVnumber, 0, 1)
                    .ToString("0.00000");
                _currentChannel[4] = CalcDop(double.Parse(_currentChannel[4]), parsedUnumber, parsedVnumber, 0, 0)
                    .ToString("0.00000");
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
                _currentChannel = (string[])tmp.Clone();
                _currentChannel[2] = CalcDop(double.Parse(_currentChannel[2]), parsedUnumber, parsedVnumber, 1, 1)
                    .ToString("0.00000");
                _currentChannel[4] = CalcDop(double.Parse(_currentChannel[4]), parsedUnumber, parsedVnumber, 1, 0)
                    .ToString("0.00000");
                _currentChannel[12] += "-L1";
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
                _currentChannel = (string[])tmp.Clone();
                _currentChannel[2] = CalcDop(double.Parse(_currentChannel[2]), parsedUnumber, parsedVnumber, 2, 1)
                    .ToString("0.00000");
                _currentChannel[4] = CalcDop(double.Parse(_currentChannel[4]), parsedUnumber, parsedVnumber, 2, 0)
                    .ToString("0.00000");
                _currentChannel[12] += "-L2";
                InsertData(_currentChannel[2], _currentChannel[3], _currentChannel[4], _currentChannel[5],
                    _currentChannel[12]);
                MessageBoxManager.GetMessageBoxStandard("注意", "插入成功！").ShowWindowDialogAsync(this);
            }
        }
        catch (Exception ae)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", $"出错:{ae.Message}！").ShowWindowDialogAsync(this);
        }
    }

    private bool CheckIsFloatOrNumber(string value)
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

    private double CalcDop(double band, int uStep, int vStep, int level, int direction)
        //direction==0: uplink
    {
        // level should be -2~~2
        if (band < 300)
        {
            // V band
            if (direction == 1)
                return band - 0.0005 * vStep * level;
            return band + 0.0005 * vStep * level;
        }

        if (direction == 1)
            return band - 0.0005 * uStep * level;
        return band + 0.0005 * uStep * level;
    }
}