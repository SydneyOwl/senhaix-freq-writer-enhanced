using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using shx8x00.constants;
using shx8x00.DataModels;

namespace shx8x00.Views;

public partial class MainWindow : Window
{
    public ObservableCollection<ChannelData> _listItems = ClassTheRadioData.getInstance().channelData;

    public ObservableCollection<ChannelData> listItems
    {
        get { return _listItems; }
        set
        {
            _listItems = value;
            ClassTheRadioData.getInstance().channelData = value;
        }
    }

    public MainWindow()
    {
        InitializeComponent(); 
        DataContext = this;
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
        if (string.IsNullOrEmpty(dataContext.TxFreq))
        {
            return;
        }

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
        if (string.IsNullOrEmpty(dataContext.RxFreq))
        {
            return;
        }

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
            MessageBoxManager.GetMessageBoxStandard("注意", "输入频率格式有误").ShowAsync();
            // dataContext.RxFreq = "";
            // listItems[int.Parse(id)] = dataContext;
            return "-1";
        }

        if (PFreq < freq.theMinFreq || PFreq > freq.theMaxFreq)
        {
            MessageBoxManager.GetMessageBoxStandard("注意",
                "频率错误!\n频率范围:" + freq.theMinFreq + "--" + freq.theMaxFreq).ShowAsync();
            return "-1";
        }

        if (freqChk.Length < 9)
        {
            for (var j = freqChk.Length; j < 9; j++)
                freqChk = j != 3 ? freqChk.Insert(j, "0") : freqChk.Insert(j, ".");
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "小数点最多只能有5位！").ShowAsync();
            // dataContext.RxFreq = "";
            // listItems[int.Parse(id)] = dataContext;
            return "-1";
        }
        var s = freqChk.Replace(".", "");
        var num5 = uint.Parse(s);
        if (num5 % 125 != 0)
        {
            ushort num7 =  125;
            var num8 = num5 / num7;
            freqChk = (num8 * num7).ToString();
        }
        return freqChk;
    }
}