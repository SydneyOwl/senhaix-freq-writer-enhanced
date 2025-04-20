using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8800Pro;
using SenhaixFreqWriter.DataModels.Shx8800Pro;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Views.Common;
using SenhaixFreqWriter.Views.Plugin;

namespace SenhaixFreqWriter.Views.Shx8800Pro;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _cancelBackup;

    private readonly CancellationTokenSource _cancelTips;
    private readonly List<Channel> _copiedChannel = new();

    private readonly Settings _settings = Settings.Load();

    private bool _devSwitchFlag;

    private string _filePath = "";

    public int CurrentArea;

    public MainWindow()
    {
        InitializeComponent();
        Title = Language.GetString("app_name") + "(8800pro)";
        _cancelTips = new CancellationTokenSource();
        _cancelBackup = new CancellationTokenSource();
        Task.Run(() => UpdateTips(_cancelTips.Token));
        Task.Run(() => UpdateBackup(_cancelBackup.Token));
        DataContext = this;
        SetArea(0);
        ListItems.CollectionChanged += CollectionChangedHandler;
        Closed += OnWindowClosed;
    }

    public ObservableCollection<Channel> ListItems { get; set; } = new();

    private async void UpdateTips(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Dispatcher.UIThread.Invoke(() => { tipBlock.Text = Tips.TipList[new Random().Next(Tips.TipList.Count)]; });
            await Task.Delay(5000, CancellationToken.None);
        }
    }

    private async void UpdateBackup(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            SysFile.CreateBackup(AppData.GetInstance());
            await Task.Delay(_settings.BackupInterval * 1000, CancellationToken.None);
        }
    }

    private void About_OnClick(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _cancelTips.Cancel();
        _cancelBackup.Cancel();
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
        channelDataGrid.InvalidateVisual();
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
        areaLabel.Content = $"{area + 1}/8";
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
        if (CurrentArea < 7) SetArea(CurrentArea + 1);
    }

    private void jumpChan_OnClick(object? sender, RoutedEventArgs e)
    {
        var tarChan = jumpTextBox.Text;
        int res;
        if (int.TryParse(tarChan, out res))
            if (res is > 0 and < 9)
                SetArea(res - 1);

        jumpTextBox.Text = "";
    }

    private void Light_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current != null) Application.Current.RequestedThemeVariant = ThemeVariant.Light;
    }

    private void Dark_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current != null) Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
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
                BusyLock = 0,
                Pttid = 0,
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
        // var selected = channelDataGrid.SelectedIndex;
        // _copiedChannel = ListItems[selected];

        _copiedChannel.Clear();
        foreach (var selectedItem in channelDataGrid.SelectedItems) _copiedChannel.Add((Channel)selectedItem);
    }

    private void MenuCutChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        // var selected = channelDataGrid.SelectedIndex;
        // _copiedChannel = ListItems[selected].DeepCopy();
        // ListItems[selected] = new Channel();
        // CalcSeq();

        _copiedChannel.Clear();
        foreach (var selectedItem in channelDataGrid.SelectedItems) _copiedChannel.Add((Channel)selectedItem);
        _copiedChannel.ForEach(x => ListItems[x.Id - 1] = new Channel());
        CalcSeq();
    }

    private void MenuPasteChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        // if (_copiedChannel == null) return;
        // var selected = channelDataGrid.SelectedIndex;
        // ListItems[selected] = _copiedChannel.DeepCopy();
        // CalcSeq();

        if (_copiedChannel.Count == 0) return;
        var selected = channelDataGrid.SelectedIndex;
        for (var i = 0; i < _copiedChannel.Count; i++) ListItems[selected + i] = _copiedChannel[i].DeepCopy();
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
        for (var i = selected; i < 63; i++) ListItems[i] = ListItems[i + 1];
        ListItems[63] = new Channel();
        CalcSeq();
    }

    private void MenuInsChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var selected = channelDataGrid.SelectedIndex;
        if (ListItems[63].IsVisable)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "信道64不为空无法插入！").ShowWindowDialogAsync(this);
            return;
        }

        var lastEmp = 0;
        for (var i = 0; i < 63; i++)
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
            SuggestedFileName = "Backup-shx8800pro-" + ts + ".dat"
        });
        if (file is not null)
        {
            _filePath = file.Path.LocalPath;
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

    private void BootImageMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new BootImageImportWindow(ShxDevice.Shx8800Pro).ShowDialog(this);
    }

    private void SatMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new SatelliteHelperWindow(InsertNewChannel).ShowDialog(this);
    }

    private void InsertNewChannel(string rx, string dec, string tx, string enc, string name)
    {
        var lastEmptyIndex = -1;
        for (var i = ListItems.Count - 1; i >= 0; i--)
            if (!ListItems[i].IsVisable)
                lastEmptyIndex = i;
            else
                break;

        if (lastEmptyIndex == -1) throw new IndexOutOfRangeException("信道空间已满，无法插入！");

        var data = new Channel
        {
            Id = lastEmptyIndex,
            RxFreq = rx,
            StrRxCtsDcs = dec,
            TxFreq = tx,
            StrTxCtsDcs = enc,
            TxPower = 0,
            Bandwide = 0,
            ScanAdd = 0,
            SignalGroup = 0,
            BusyLock = 0,
            Pttid = 0,
            IsVisable = true,
            Name = name
        };
        ListItems[lastEmptyIndex] = data;
    }

    private void DebugWindowMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        DebugWindow.GetInstance().Show();
    }

    private void portSel_OnClick(object? sender, RoutedEventArgs e)
    {
        new PortSelectionWindow().ShowDialog(this);
    }

    private void MenuConnectBT_OnClick(object? sender, RoutedEventArgs e)
    {
        new BluetoothDeviceSelectionWindow(ShxDevice.Shx8800Pro).ShowDialog(this);
    }

    private void SettingMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog(this);
    }
    private async void SaveAsExcelMenuItem_OnClick(object? sender, RoutedEventArgs e)
    { 
        var ts = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "导出信道信息到",
            SuggestedFileName = "Channels-" + ts + ".xlsx",
            // DefaultExtension = ".xlsx",
            // FileTypeChoices = new []{new FilePickerFileType(".xlsx")}
        });
        if (file is not null)
        {
            var savePath = file.Path.LocalPath;
            try
            {
                AppData.GetInstance().SaveAsExcel(savePath);
            }
            catch (Exception er)
            {
                DebugWindow.GetInstance().UpdateDebugContent(er.Message);
                await MessageBoxManager.GetMessageBoxStandard("注意", "导出失败，请检查文件格式！").ShowWindowDialogAsync(this);
            }
        }
    }

    private async void ReadFromExcelMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开文件",
            AllowMultiple = false
        });
        if (files.Count > 0)
        {
            var loadPath = files[0].Path.LocalPath;
            try
            {
                AppData.GetInstance().LoadFromExcel(loadPath);
            }
            catch (Exception er)
            {
                DebugWindow.GetInstance().UpdateDebugContent(er.Message);
                await MessageBoxManager.GetMessageBoxStandard("注意", "导入失败，请检查文件格式！").ShowWindowDialogAsync(this);
            }
            // AppData.ForceNewInstance();
            SetArea(0);
        }
    }
    
}