using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using shx8x00.Constants;
using shx8x00.DataModels;
using shx8x00.Utils.Serial;

namespace shx8x00.Views;

public partial class MainWindow : Window
{
    private ObservableCollection<ChannelData> _listItems = ClassTheRadioData.getInstance().chanData;

    private string savePath = "";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public ObservableCollection<ChannelData> listItems
    {
        get => _listItems;
        set
        {
            _listItems = value;
            ClassTheRadioData.getInstance().chanData = value;
        }
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void txFreq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as ChannelData;
        var id = dataContext.ChanNum;
        if (string.IsNullOrEmpty(dataContext.TxFreq)) return;

        var parsed = freqParser(dataContext.TxFreq);
        if (parsed.Equals("-1"))
        {
            dataContext.TxFreq = "";
            listItems[int.Parse(id)] = dataContext;
            return;
        }

        dataContext.TxFreq = parsed;
        listItems[int.Parse(id)] = dataContext;
    }

    private void rxFreq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as ChannelData;
        var id = dataContext.ChanNum;
        if (string.IsNullOrEmpty(dataContext.RxFreq)) return;

        var parsed = freqParser(dataContext.RxFreq);
        if (parsed.Equals("-1"))
        {
            dataContext.RxFreq = "";
            listItems[int.Parse(id)] = dataContext;
            return;
        }

        // 写入默认值
        if (dataContext.allEmpty())
        {
            var data = new ChannelData
            {
                RxFreq = parsed,
                TxAllow = "Yes",
                Encrypt = "OFF",
                Pttid = "OFF",
                BandWidth = "W",
                BusyLock = "OFF",
                QtDec = "OFF",
                QtEnc = "OFF",
                ScanAdd = "ON",
                TxPwr = "H",
                SigCode = "1",
                ChanNum = id,
                TxFreq = parsed
            };
            listItems[int.Parse(id)] = data;
        }
        else
        {
            dataContext.RxFreq = parsed;
            listItems[int.Parse(id)] = dataContext;
        }
    }

    private string freqParser(string freqChk)
    {
        double PFreq;
        // 检查频率范围
        if (!double.TryParse(freqChk, out PFreq))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "输入频率格式有误").ShowWindowDialogAsync(this);
            // dataContext.RxFreq = "";
            // listItems[int.Parse(id)] = dataContext;
            return "-1";
        }

        if (PFreq < FREQ.theMinFreq || PFreq > FREQ.theMaxFreq)
        {
            MessageBoxManager.GetMessageBoxStandard("注意",
                "频率错误!\n频率范围:" + FREQ.theMinFreq + "--" + FREQ.theMaxFreq).ShowWindowDialogAsync(this);
            return "-1";
        }

        if (freqChk.Length < 9)
        {
            for (var j = freqChk.Length; j < 9; j++)
                freqChk = j != 3 ? freqChk.Insert(j, "0") : freqChk.Insert(j, ".");
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "精度过高！").ShowWindowDialogAsync(this);
            // dataContext.RxFreq = "";
            // listItems[int.Parse(id)] = dataContext;
            return "-1";
        }

        var s = freqChk.Replace(".", "");
        var num5 = uint.Parse(s);
        if (num5 % 125 != 0)
        {
            ushort num7 = 125;
            var num8 = num5 / num7;
            freqChk = (num8 * num7).ToString();
        }

        return freqChk;
    }

    private void About_OnClick(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }

    private async void new_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("注意", "该操作将清空编辑中的信道，确定继续？",
                ButtonEnum.YesNo);

        var result = await box.ShowWindowDialogAsync(this);
        if (result == ButtonResult.No) return;
        // TODO: 优雅一些，不知道为啥直接更新整个ObserItem的话界面不会更新
        ClassTheRadioData.forceNew();
        for (var i = 0; i < listItems.Count; i++)
        {
            var tmp = new ChannelData();
            tmp.ChanNum = i.ToString();
            listItems[i] = tmp;
        }
    }

    private async void saveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存配置文件",
            SuggestedFileName = "Backup-" + ts + ".dat"
        });
        if (file is not null)
        {
            savePath = new Uri(file.Path.ToString()).LocalPath;
            await using var stream = await file.OpenWriteAsync();
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            ClassTheRadioData.getInstance().SaveToFile(stream);
            stream.Close();
        }
    }

    private void save_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(savePath))
        {
            Stream stream = new FileStream(savePath, FileMode.OpenOrCreate);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            ClassTheRadioData.getInstance().SaveToFile(stream);
            stream.Close();
        }
        else
        {
            saveAs_OnClick(null, null);
        }
    }

    private void exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void dtmfset_OnClick(object? sender, RoutedEventArgs e)
    {
        var dtmfWindow = new DTMFWindow();
        dtmfWindow.ShowDialog(this);
    }

    private void option_OnClick(object? sender, RoutedEventArgs e)
    {
        new OptionalWindow().ShowDialog(this);
    }

    private async void open_OnClick(object? sender, RoutedEventArgs e)
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
            ClassTheRadioData.CreatObjFromFile(stream);
            var inst = ClassTheRadioData.getInstance().chanData;

            // TODO: 优雅一些，不知道为啥直接更新整个ObserItem的话界面不会更新
            for (var i = 0; i < listItems.Count; i++) listItems[i] = inst[i];
        }
    }


    private async void readChannel_OnClick(object? sendser, RoutedEventArgs e)
    {
        var tmp = ClassTheRadioData.getInstance();
        tmp.channeldata = tmp.chanData.ToList();
        if (MySerialPort.getInstance().TargetPort == "")
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择！").ShowWindowDialogAsync(this);
            await new PortSelectionWindow().ShowDialog(this);
        }

        if (!string.IsNullOrEmpty(MySerialPort.getInstance().TargetPort))
            await new ProgressBarWindow(0).ShowDialog(this);
    }

    private async void writeChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var flag = false;
        var tmp = ClassTheRadioData.getInstance();
        tmp.channeldata = tmp.chanData.ToList();
        // 检查信道
        for (var a = 0; a < listItems.Count; a++)
        {
            if (listItems[a].allEmpty() || listItems[a].filled()) continue;
            // 不写入不完整的信道
            tmp.channeldata[a] = new ChannelData();
            flag = true;
        }

        if (flag)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("注意", "您有信道未完全填写，写入时将忽略!", ButtonEnum.YesNo);
            var result = await box.ShowWindowDialogAsync(this);
            if (result == ButtonResult.No)
            {
                tmp.channeldata = tmp.chanData.ToList();
                return;
            }
        }

        if (MySerialPort.getInstance().TargetPort == "")
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择！").ShowWindowDialogAsync(this);
            await new PortSelectionWindow().ShowDialog(this);
        }

        if (!string.IsNullOrEmpty(MySerialPort.getInstance().TargetPort))
            await new ProgressBarWindow(1).ShowDialog(this);
    }

    private void portSel_OnClick(object? sender, RoutedEventArgs e)
    {
        new PortSelectionWindow().ShowDialog(this);
    }
}