using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using Newtonsoft.Json.Linq;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class SatelliteHelperWindow : Window
{
    public delegate void InsertChannelMethod(string rx, string rxDec, string tx, string txDec, string name);

    private string[] _currentChannel = new string[14];

    private JArray _satelliteJson = new();

    public InsertChannelMethod InsertData;

    private string _loadedJson = "";

    public List<string> SatelliteList = new();

    private Settings _settings = Settings.Load();
    
    public event EventHandler<WindowClosingEventArgs> CloseEvent = (sender, args) =>
    {
        args.Cancel = true;
    } ;

    public SatelliteHelperWindow()
    {
        InitializeComponent();
    }

    public SatelliteHelperWindow(InsertChannelMethod func)
    {
        InitializeComponent();
        InsertData = func;
        DataContext = this;
        if (!SysFile.CheckDefaultDirectory())
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                selectedSatelliteInfo.Text += "无法存储json！切换到Mem模式...\n";
                DebugWindow.GetInstance().UpdateDebugContent("无法读取json文件！#无法新建目录！切换到Mem模式...");
            });
            Task.Run(() => { FetchData(true); });
        }
        else
        {
            Task.Run(() =>
            {
                if (!LoadJson())
                {
                    Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "正在为您更新卫星数据...\n"; });
                    FetchData();
                }
            });
        }
    }

    public ObservableCollection<string> Namelist { get; set; } = new();

    private bool LoadJson(bool useMem = false)
    {
        var satelliteData = "";
        if (useMem)
        {
            satelliteData = _loadedJson;
        }
        else
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (!File.Exists($"{_settings.DataDir}/amsat-all-frequencies.json"))
                {
                    DebugWindow.GetInstance().UpdateDebugContent("未找到json");
                    Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "未找到卫星数据,请点击更新星历！\n"; });
                    return false;
                }

                satelliteData = File.ReadAllText($"{_settings.DataDir}/amsat-all-frequencies.json");
            }
            else
            {
                // 更新都是更新到DATA_DIR，只有当用户没点过更新的话才使用包里附带的
                if (File.Exists($"{_settings.DataDir}/amsat-all-frequencies.json"))
                {
                    satelliteData = File.ReadAllText($"{_settings.DataDir}/amsat-all-frequencies.json");
                }
                else if (File.Exists(Path.Join(AppContext.BaseDirectory, "amsat-all-frequencies.json")))
                {
                    satelliteData = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "amsat-all-frequencies.json"));
                }
                else
                {
                    DebugWindow.GetInstance().UpdateDebugContent("未找到json");
                    Dispatcher.UIThread.Invoke(() => { selectedSatelliteInfo.Text += "未找到卫星数据,请点击更新星历！\n"; });
                    return false;
                }
            }
        }

        if (string.IsNullOrEmpty(satelliteData))
        {
            DebugWindow.GetInstance().UpdateDebugContent("json为空");
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
            DebugWindow.GetInstance().UpdateDebugContent($"ERR：{e.Message}");
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
        centerTx.Text = Checker(uplink);
        centerRx.Text = Checker(downlink);
        satchanname.Text = Checker(sat);
        sattone.Text = tone;
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

    private void FetchData(bool useMem = false)
    {
        var url =
            "https://raw.githubusercontent.com/palewire/amateur-satellite-database/main/data/amsat-all-frequencies.json";
        try
        {
            Dispatcher.UIThread.Post(() =>
            {
                ProgressRing.IsActive = true;
                FetchSatText.Text = "更新中...";
                FetchSat.IsEnabled = false;
                Closing += CloseEvent;
            });
            DownloadSatData(url, useMem);
            LoadJson(useMem);
            Dispatcher.UIThread.Post(() =>
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
            });
        }
        catch (Exception w)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"下载出错：{w.Message}，ppxy...");
            Dispatcher.UIThread.Post(() =>
            {
                selectedSatelliteInfo.Text = $"出错：{w.Message},重试中...";
                FetchSatText.Text = "重试中...";
            });
            url = "https://cdn.jsdelivr.net/gh/palewire/amateur-satellite-database/data/amsat-all-frequencies.json";
            try
            {
                DownloadSatData(url, useMem);
                LoadJson(useMem);
                Dispatcher.UIThread.Post(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "更新完成！").ShowWindowDialogAsync(this);
                });
            }
            catch (Exception a)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    selectedSatelliteInfo.Text = $"出错：{a.Message}...";
                    DebugWindow.GetInstance().UpdateDebugContent($"下载出错：{w.Message}");
                    MessageBoxManager.GetMessageBoxStandard("注意", "更新失败....").ShowWindowDialogAsync(this);
                });
            }
        }
        finally
        {
            Dispatcher.UIThread.Post(() =>
            {
                ProgressRing.IsActive = false;
                selectedSatelliteInfo.Text = "";
                FetchSat.IsEnabled = true;
                FetchSatText.Text = "更新星历";
            });
            Closing -= CloseEvent;
            // while (!handler.IsCompleted)
            // {
            //     Thread.Sleep(50);
            // }
        }
    }

    private void InsertChannelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // ModeListBox_OnSelectionChanged(null, null);
        _currentChannel[2] = centerRx.Text;
        _currentChannel[4] = centerTx.Text;
        _currentChannel[5] = sattone.Text;
        _currentChannel[12] = satchanname.Text;
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
        if (kk > 520 || kk < 100)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率范围有误！").ShowWindowDialogAsync(this);
            return;
        }

        _currentChannel[2] = kk.ToString("0.00000");

        double.TryParse(_currentChannel[4], out kk);
        if (kk > 520 || kk < 100)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率范围有误！").ShowWindowDialogAsync(this);
            return;
        }

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
        return int.TryParse(value, out _) || double.TryParse(value, out _);
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

    private bool DownloadSatData(string url, bool useMem = false)
    {
        var target = new Uri(url);
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(15);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "CSharpHttpClient");
        var resp = httpClient.GetAsync(target).Result;
        if (resp.IsSuccessStatusCode)
        {
            if (useMem)
                _loadedJson = resp.Content.ReadAsStringAsync().Result;
            else
                using (var fs = File.Create($"{_settings.DataDir}/amsat-all-frequencies.json"))
                {
                    var stm = resp.Content.ReadAsStream();
                    stm.CopyTo(fs);
                    return true;
                }
        }

        return false;
    }
}