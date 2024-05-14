using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.BLE.Platforms.Generic;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Views.Common;
using SenhaixFreqWriter.Views.Gt12.Plugin;

#if WINDOWS
using shx.Utils.BLE.Platforms.Windows;
#endif

namespace SenhaixFreqWriter.Views.Gt12;

public partial class MainWindow : Window
{
    public ObservableCollection<Channel> ListItems { get; set; } = new();

    public int CurrentArea = 0;

    private bool _devSwitchFlag = false;

    private string _filePath = "";

    private Channel _copiedChannel;

    private IBluetooth _osBle;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        SetArea(0);
        ListItems.CollectionChanged += CollectionChangedHandler;
        Closed += OnWindowClosed;
        if (HidTools.GetInstance().IsDeviceConnected)
            statusLabel.Content = "连接状态：已连接";
        else
            statusLabel.Content = "连接状态：未连接";
        HidTools.GetInstance().UpdateLabel = connected =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (connected)
                    statusLabel.Content = "连接状态：已连接";
                else
                    statusLabel.Content = "连接状态：未连接";
            });
        };
        HidTools.GetInstance().FindAndConnect();
    }


    private void About_OnClick(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Close();
        if (!_devSwitchFlag) Environment.Exit(0);
    }

    private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
        // if (e.Action.Equals(NotifyCollectionChangedAction.Add) ||
        //     e.Action.Equals(NotifyCollectionChangedAction.Remove))
        // {
        CalcSeq();
        AppData.GetInstance().ChannelList[CurrentArea] = ListItems.ToArray();
        // }
    }

    private void CalcSeq()
    {
        for (var i = 0; i < ListItems.Count; i++) ListItems[i].Id = i + 1;
    }

    private string CalcNameSize(string name)
    {
        var num = 0;
        var num2 = 0;
        var text = name;
        var bytes = Encoding.GetEncoding("gb2312").GetBytes(text);
        if (bytes.Length > 12)
        {
            var num3 = 0;
            while (num3 < 12)
                if (bytes[num3] >= 47 && bytes[num3] < 127)
                {
                    num++;
                    num3++;
                }
                else
                {
                    num2++;
                    num3 += 2;
                }

            text = num % 2 == 0
                ? Encoding.GetEncoding("gb2312").GetString(bytes, 0, 12)
                : Encoding.GetEncoding("gb2312").GetString(bytes, 0, 11);
        }

        return text;
    }

    private void SetArea(int area)
    {
        CurrentArea = area;
        var tmpChannel = AppData.GetInstance().ChannelList[area];
        ListItems.Clear();
        for (var i = 0; i < tmpChannel.Length; i++) ListItems.Add(tmpChannel[i]);
        areaName.Text = AppData.GetInstance().BankName[area];
        areaLabel.Content = $"{area + 1}/30";
    }

    private async void readChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        await new ProgressBarWindow(OpType.Read).ShowDialog(this);
        SetArea(0);
    }

    private async void writeChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        await new ProgressBarWindow(OpType.Write).ShowDialog(this);
        SetArea(0);
    }

    private void foreChan_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CurrentArea > 0) SetArea(CurrentArea - 1);
    }

    private void nextChan_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CurrentArea < 29) SetArea(CurrentArea + 1);
    }

    private void jumpChan_OnClick(object? sender, RoutedEventArgs e)
    {
        var tarChan = jumpTextBox.Text;
        int res;
        if (int.TryParse(tarChan, out res))
            if (res is > 0 and < 31)
                SetArea(res - 1);

        jumpTextBox.Text = "";
    }

    private void Light_OnClick(object? sender, RoutedEventArgs e)
    {
        RequestedThemeVariant = ThemeVariant.Light;
    }

    private void Dark_OnClick(object? sender, RoutedEventArgs e)
    {
        RequestedThemeVariant = ThemeVariant.Dark;
    }

    private void AreaName_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(areaName.Text))
        {
            areaName.Text = AppData.GetInstance().BankName[CurrentArea];
            return;
        }

        areaName.Text = CalcNameSize(areaName.Text);
        AppData.GetInstance().BankName[CurrentArea] = CalcNameSize(areaName.Text);
    }

    private string FreqChecker(string text)
    {
        var num = 0;
        // 检查频率范围
        if (!double.TryParse(text, out _))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "输入频率格式有误").ShowWindowDialogAsync(this);
            return "-1";
        }

        var array = text.Split('.');
        var list = new List<int>();
        for (var j = 0; j < array.Length; j++) list.Add(int.Parse(array[j]));

        if (list[0] < Freq.MinFreq || list[0] >= Freq.MaxFreq)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "输入频率格式有误").ShowWindowDialogAsync(this);
            return "-1";
        }

        num = list[0] * 100000;
        if (list.Count > 1)
        {
            var num5 = 5 - array[1].Length;
            if (num5 > 0)
                for (var k = 0; k < num5; k++)
                    list[1] *= 10;

            num += list[1];
        }

        if (num % 125 != 0)
        {
            num /= 125;
            num *= 125;
        }

        return num.ToString().Insert(3, ".");
    }


    private void txFreq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as Channel;
        var id = dataContext.Id;
        if (string.IsNullOrEmpty(dataContext.TxFreq)) return;

        var parsed = FreqChecker(dataContext.TxFreq);
        if (parsed.Equals("-1"))
        {
            dataContext.TxFreq = "";
            ListItems[id - 1] = dataContext;
            return;
        }

        dataContext.TxFreq = parsed;
        ListItems[id - 1] = dataContext;
    }

    private void rxfreq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as Channel;
        var id = dataContext.Id;
        if (string.IsNullOrEmpty(dataContext.RxFreq)) return;

        var parsed = FreqChecker(dataContext.RxFreq);
        if (parsed.Equals("-1"))
        {
            dataContext.RxFreq = "";
            ListItems[id - 1] = dataContext;
            return;
        }

        // 写入默认值
        if (!dataContext.IsVisable)
        {
            var data = new Channel
            {
                Id = id,
                RxFreq = parsed,
                StrRxCtsDcs = "OFF",
                TxFreq = parsed,
                StrTxCtsDcs = "OFF",
                TxPower = 0,
                Bandwide = 0,
                ScanAdd = 0,
                SignalGroup = 0,
                SqMode = 0,
                Pttid = 0,
                SignalSystem = 0,
                IsVisable = true
            };
            ListItems[id - 1] = data;
        }
        else
        {
            dataContext.RxFreq = parsed;
            ListItems[id - 1] = dataContext;
        }
    }

    private void SwitchDevice_OnClick(object? sender, RoutedEventArgs e)
    {
        _devSwitchFlag = true;
        new DeviceSelectWindow().Show();
        Close();
    }

    private void MenuCopyChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = channelDataGrid.SelectedIndex;
        _copiedChannel = ListItems[selected];
    }

    private void MenuCutChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = channelDataGrid.SelectedIndex;
        _copiedChannel = ListItems[selected].DeepCopy();
        ListItems[selected] = new Channel();
        CalcSeq();
    }

    private void MenuPasteChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_copiedChannel == null) return;
        var selected = channelDataGrid.SelectedIndex;
        ListItems[selected] = _copiedChannel.DeepCopy();
        CalcSeq();
    }

    private void MenuClrChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = new List<int>();
        foreach (var selectedItem in channelDataGrid.SelectedItems) selected.Add(((Channel)selectedItem).Id - 1);
        foreach (var o in selected) ListItems[o] = new Channel();
        // var selected = channelDataGrid.SelectedIndex;
        // ListItems[selected] = new Channel();
        CalcSeq();
    }

    private void MenuDelChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = channelDataGrid.SelectedIndex;
        for (var i = selected; i < 31; i++) ListItems[i] = ListItems[i + 1];
        ListItems[31] = new Channel();
        CalcSeq();
    }

    private void MenuInsChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = channelDataGrid.SelectedIndex;
        if (ListItems[31].IsVisable)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "信道32不为空无法插入！").ShowWindowDialogAsync(this);
            return;
        }

        var lastEmp = 0;
        for (var i = 0; i < 31; i++)
            if (ListItems[i].IsVisable)
                lastEmp = i;

        for (var i = lastEmp; i > selected; i--) ListItems[i + 1] = ListItems[i];

        ListItems[selected + 1] = new Channel();
        CalcSeq();
    }

    private void MenuComChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var cachedChannel = new ObservableCollection<Channel>();
        for (var i = 0; i < 32; i++) cachedChannel.Add(new Channel());
        var channelCursor = 0;
        for (var i = 0; i < 32; i++)
            if (ListItems[i].IsVisable)
                cachedChannel[channelCursor++] = ListItems[i].DeepCopy();

        for (var i = 0; i < channelCursor; i++) ListItems[i] = cachedChannel[i].DeepCopy();

        for (var i = channelCursor; i < 32; i++) ListItems[i] = new Channel();
        CalcSeq();
    }

    private void VfoMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new VfoModeWindow().ShowDialog(this);
    }


    private void OptionalMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new OptionalWindow().ShowDialog(this);
    }

    private void FMMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new FmWindow().ShowDialog(this);
    }

    private void DTMFMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new DtmfWindow().ShowDialog(this);
    }

    private async void NewFileMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("注意", "该操作将清空编辑中的信道，确定继续？",
                ButtonEnum.YesNo);

        var result = await box.ShowWindowDialogAsync(this);
        if (result == ButtonResult.No) return;
        AppData.ForceNewInstance();
        SetArea(0);
    }

    private void SaveFileMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_filePath))
        {
            Stream stream = new FileStream(_filePath, FileMode.OpenOrCreate);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            AppData.GetInstance().SaveToFile(stream);
            stream.Close();
        }
        else
        {
            SaveAsMenuItem_OnClick(null, null);
        }
    }

    private async void SaveAsMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存配置文件",
            SuggestedFileName = "Backup-GT12-" + ts + ".dat"
        });
        if (file is not null)
        {
            _filePath = new Uri(file.Path.ToString()).LocalPath;
            await using var stream = await file.OpenWriteAsync();
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            AppData.GetInstance().SaveToFile(stream);
            stream.Close();
        }
    }

    private void ExitMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
        Environment.Exit(0);
    }

    private async void OpenFileMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开备份",
            AllowMultiple = false
        });
        if (files.Count > 0)
        {
            await using var stream = await files[0].OpenReadAsync();
            AppData.CreatObjFromFile(stream);
            SetArea(0);
        }
    }

    private void ConnectMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var hint = new HintWindow();
        hint.SetLabelStatus("(此为调试功能，正常情况软件会自动连接GT12)");
        hint.SetButtonStatus(false);
        hint.ShowDialog(this);

        hint.SetLabelStatus("-------所有HID设备-------");
        foreach (var hidDevice in HidTools.GetAllHidDevices())
        {
            hint.SetLabelStatus($"设备名：{hidDevice.GetProductName()}");
            // hint.setLabelStatus($"序列号：{hidDevice.GetSerialNumber()}");
            hint.SetLabelStatus($"VID：{hidDevice.VendorID}");
            hint.SetLabelStatus($"PID：{hidDevice.ProductID}");
            hint.SetLabelStatus($"路径：{hidDevice.DevicePath}");
            hint.SetLabelStatus("----------------");
        }

        if (HidTools.GetInstance().FindAndConnect() == HidStatus.Success)
        {
            hint.SetLabelStatus("连接成功!");
            hint.SetLabelStatus("-------设备信息-------");
            var gt12 = HidTools.GetInstance().Gt12Device;
            hint.SetLabelStatus($"最大输入长度：{gt12.GetMaxInputReportLength()}");
            hint.SetLabelStatus($"最大输出长度：{gt12.GetMaxOutputReportLength()}");
            hint.SetLabelStatus($"PID：{gt12.ProductID}");
            hint.SetLabelStatus($"VID：{gt12.VendorID}");
            hint.SetLabelStatus($"设备路径：{gt12.DevicePath}");
            // hint.setLabelStatus($"序列号：{gt12.GetSerialNumber()}");
            hint.SetLabelStatus($"设备名：{gt12.GetProductName()}");
            // hint.setLabelStatus($"文件系统名：{gt12.GetFileSystemName()}");
            // hint.setLabelStatus($"ReNumber：{gt12.ReleaseNumber}");
        }
        else
        {
            hint.SetLabelStatus("连接失败！");
        }

        hint.SetButtonStatus(true);
    }

    private async void BTMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        _osBle?.Dispose();
        _osBle = new GenerticShxble();
#if WINDOWS
        _osBle = new WindowsShxble();
#endif
        _osBle.SetStatusUpdater(status =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (status)
                    statusLabel.Content = "连接状态：蓝牙已连接";
                else
                    statusLabel.Content = "连接状态：蓝牙未连接";
            });
        });
        // for windows and macoos
        try
        {
            var available = await _osBle.GetBleAvailabilityAsync();
            // var available = true;
            if (!available)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "您的系统不受支持或蓝牙未打开！").ShowWindowDialogAsync(this);
                return;
            }
        }
        catch (Exception ed)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "您的系统不受支持或蓝牙未打开:" + ed.Message).ShowWindowDialogAsync(this);
            return;
        }

        var hint = new HintWindow();
        hint.SetLabelStatus("自动搜索中...");
        hint.SetButtonStatus(false);
        hint.ShowDialog(this);

        if (!await _osBle.ScanForShxAsync())
        {
            hint.SetLabelStatus("未找到设备！\n您可能需要重启软件！");
            hint.SetButtonStatus(true);
            return;
        }

        hint.SetLabelStatus("已找到设备,尝试连接中...");
        // Get Char.....
        try
        {
            await _osBle.ConnectShxDeviceAsync();
        }
#if __LINUX__
        catch (Tmds.DBus.DBusException)
        {
            hint.setLabelStatus("连接失败！\n请在设置-蓝牙中取消对walkie-talkie的连接。\n如果您是初次连接，请在设置中手动配对\nwalkie-talkie并点击配对！");
            hint.setButtonStatus(true);
            return;
        }
#endif
        catch (Exception ea)
        {
            hint.SetLabelStatus("连接失败！" + ea.Message);
            hint.SetButtonStatus(true);
            return;
        }

        // Console.WriteLine("Connected");
        if (!await _osBle.ConnectShxRwServiceAsync())
        {
            hint.SetLabelStatus("未找到写特征\n确认您使用的是GT12");
            hint.SetButtonStatus(true);
            return;
        }


        if (!await _osBle.ConnectShxRwCharacteristicAsync())
        {
            hint.SetLabelStatus("未找到写特征\n确认您使用的是GT12");
            hint.SetButtonStatus(true);
            return;
        }

        _osBle.RegisterHid();
        hint.SetLabelStatus("连接成功！\n请点击关闭，并进行读写频");
        hint.SetButtonStatus(true);
    }

    private void BootImageMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new BootImageImportWindow().ShowDialog(this);
    }
}