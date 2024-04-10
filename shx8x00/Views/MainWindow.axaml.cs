using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using shx8x00.DataModels;

namespace shx8x00.Views;

public partial class MainWindow : Window
{
    public ObservableCollection<ChannelData> listItems { get; set; } = ClassTheRadioData.getInstance().channelData;
    public MainWindow()
    {
        InitializeComponent(); 
        DataContext = this;
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine(listItems[0].TxAllow);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine(listItems[0].ToString());
    }

    private void AvaloniaObject_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        Console.WriteLine(e.ToString());
    }

    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as ChannelData; // 替换为实际的数据类型
        var id = dataContext.ChanNum;
        if (string.IsNullOrEmpty(dataContext.TxFreq))
        {
            return;
        }
        var data = new ChannelData
        {
            RxFreq = dataContext.TxFreq,
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
            TxFreq = dataContext.TxFreq
        };
        listItems[int.Parse(id)] = data;
    }
}