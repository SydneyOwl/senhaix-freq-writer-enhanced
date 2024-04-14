using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using InTheHand.Bluetooth;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using shx8x00.Constants;
using shx8x00.DataModels;
using shx8x00.Utils.Serial;
using SHX8X00.Views;

namespace shx8x00.Views;

public partial class MainWindow : Window
{
    private ChannelData tmpChannel;
    private ObservableCollection<ChannelData> _listItems = ClassTheRadioData.getInstance().chanData;
    public ObservableCollection<ChannelData> listItems
    {
        get => _listItems;
        set
        {
            _listItems = value;
            ClassTheRadioData.getInstance().chanData = value;
        }
    }
    //无法正常更新序号，感觉是他的bug!
    

    private string savePath = "";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        _listItems.CollectionChanged += CollectionChangedHandler;
        // 不太优雅，之后改
        // 始终关不掉几个线程 先这样好了......
        Closed += OnWindowClosed;
    }

    private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action.Equals(NotifyCollectionChangedAction.Add)||e.Action.Equals(NotifyCollectionChangedAction.Remove))
        {
            calcSequence();
        }
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Close();
        Environment.Exit(0);
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
        // _TODO: 优雅一些，不知道为啥直接更新整个ObserItem的话界面不会更新
        // ClassTheRadioData.forceNew();
        // this.listItems = ClassTheRadioData.getInstance().chanData;
        for (var i = 0; i < listItems.Count; i++)
        {
            var tmp = new ChannelData();
            tmp.ChanNum = i.ToString();
            listItems[i] = tmp;
        }
        ClassTheRadioData.getInstance().channeldata = listItems.ToList();
        ClassTheRadioData.getInstance().dtmfData = new DTMFData();
        ClassTheRadioData.getInstance().funCfgData = new FunCFGData();
        ClassTheRadioData.getInstance().otherImfData = new OtherImfData();
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
        if (MySerialPort.getInstance().TargetPort == "" && MySerialPort.getInstance().Characteristic == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择,请连接蓝牙或写频线").ShowWindowDialogAsync(this);
            return;
        }
        // await new PortSelectionWindow().ShowDialog(this);

        // if (!string.IsNullOrEmpty(MySerialPort.getInstance().TargetPort))
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

        if (MySerialPort.getInstance().TargetPort == "" && MySerialPort.getInstance().Characteristic == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择，请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
            return;
        }
        
        // await new PortSelectionWindow().ShowDialog(this);

        // if (!string.IsNullOrEmpty(MySerialPort.getInstance().TargetPort))
        await new ProgressBarWindow(1).ShowDialog(this);
    }

    private void portSel_OnClick(object? sender, RoutedEventArgs e)
    {
        new PortSelectionWindow().ShowDialog(this);
    }


    // private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    // {
    //     ClassTheRadioData.getInstance().sort();
    //     Console.Write("ca;;ed");
    // }
    private void MenuCopyChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        tmpChannel = listItems[selected];
    }

    private void MenuCutChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        tmpChannel = listItems[selected].DeepCopy();
        listItems[selected] = new ChannelData();
        calcSequence();
    }

    private void MenuPasteChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        listItems[selected] = tmpChannel.DeepCopy();
        calcSequence();
    }

    private void MenuClrChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        listItems[selected] = new ChannelData();
        calcSequence();
    }

    private void MenuDelChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        for (var i = selected; i < 127; i++) listItems[i] = listItems[i + 1];
        listItems[127] = new ChannelData();
        calcSequence();
    }

    private void MenuInsChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        int selected = channelDataGrid.SelectedIndex;
        if (!listItems[127].allEmpty())
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "信道127不为空无法插入！").ShowWindowDialogAsync(this);
            return;
        }
        var lastEmp = 0;
        for (var i = 0; i < 127; i++)
            if (!listItems[i].allEmpty())
                lastEmp = i;

        for (var i = lastEmp; i > selected; i--) listItems[i + 1] = listItems[i];

        listItems[selected + 1] = new ChannelData();
        calcSequence();
    }

    private void MenuComChannel_OnClick(object? sender, RoutedEventArgs e)
    {
        var cached_channel = new ObservableCollection<ChannelData>();
        for (int i = 0; i < 128; i++)
        {
            cached_channel.Add(new ChannelData());
        }
        var channel_cursor = 0;
        for (var i = 0; i < 128; i++)
            //check it via "TxAllow"
            if (!listItems[i].allEmpty())
            {
                cached_channel[channel_cursor++] = listItems[i].DeepCopy();
            }

        for (var i = 0; i < channel_cursor; i++)
        {
            listItems[i] = cached_channel[i].DeepCopy();
        }

        for (var i = channel_cursor; i < 128; i++)
        {
            listItems[i] = new ChannelData();
        }
    }
    private void calcSequence()
    {
        for (var i = 0; i < listItems.Count; i++)
        {
            listItems[i].ChanNum = i.ToString();
        }
    }
    
    private async void MenuConnectBT_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var ans = MessageBoxManager.GetMessageBoxStandard("注意", "Linux下蓝牙写频Bug多多，您确定要继续吗", ButtonEnum.OkCancel);
            var result =  await ans.ShowWindowDialogAsync(this);
            if (result != ButtonResult.Yes)
            {
                return;
            }
        }
        Console.WriteLine("Requesting Bluetooth Device...");
        // for windows and macoos
        try
        {
            var available = await Bluetooth.GetAvailabilityAsync();
            // var available = true;
            if (!available)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "您的系统不受支持或蓝牙未打开！").ShowWindowDialogAsync(this);
                return;
            }
        }
        catch (Exception ed)
        {
            Console.WriteLine(ed.Message);
            MessageBoxManager.GetMessageBoxStandard("注意", "您的系统不受支持或蓝牙未打开！").ShowWindowDialogAsync(this);
            return;
        }

        var hint = new HintBT();
        hint.setLabelStatus("自动搜索中...");
        hint.setButtonStatus(false);
        hint.ShowDialog(this);
        BluetoothDevice device = null;
        var filter = new BluetoothLEScanFilter()
        {
            Name = BLE.BTNAME_SHX8800
        };
        try
        {
            // 过滤名称
            device = await Bluetooth.RequestDeviceAsync(new RequestDeviceOptions { Filters = { filter } });
        }
        catch
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var discoveredDevices = await Bluetooth.ScanForDevicesAsync(new RequestDeviceOptions()
            {
                Filters = { filter }
            },cts.Token);
            foreach (var discoveredDevice in discoveredDevices)
            {
                if (discoveredDevice.Name.Equals(BLE.BTNAME_SHX8800))
                {
                    device = discoveredDevice;
                    break;
                }
            }
        }

        if (device == null)
        {
           hint.setLabelStatus("未找到设备！");
           hint.setButtonStatus(true);
           return;
        }
        hint.setLabelStatus("已找到设备\nMAC:"+device.Id+"\n尝试连接中...");
        // Get Char.....
        await device.Gatt.ConnectAsync();
        Console.WriteLine("Connected");
        var service = await device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromShortId(Convert.ToUInt16(BLE.RW_SERVICE_UUID.ToUpper(), 16)));
        if (service == null)
        {
            hint.setLabelStatus("未找到写特征\n确认您使用的是8800");
            hint.setButtonStatus(true);
            return;
        }

        var character = await service.GetCharacteristicAsync(
            BluetoothUuid.FromShortId(Convert.ToUInt16(BLE.RW_CHARACTERISTIC_UUID.ToUpper(), 16)));
        
        if (character == null)
        {
            hint.setLabelStatus("未找到写特征\n确认您使用的是8800");
            hint.setButtonStatus(true);
            return;
        }
        else
        {
            character.CharacteristicValueChanged += Characteristic_CharacteristicValueChanged;
            await character.StartNotificationsAsync();
            MySerialPort.getInstance().Characteristic = character;
            MySerialPort.getInstance().BtDeviceMtu = device.Gatt.Mtu;
            hint.setLabelStatus("连接成功！\n请点击关闭，并进行读写频");
            hint.setButtonStatus(true);
        }
        // cable.IsVisible = false;
    }
    private void Characteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        foreach (var b in e.Value)
        {
            MySerialPort.getInstance().RxData.Enqueue(b);
        }
    }
}