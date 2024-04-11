using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using shx8x00.Utils.Serial;

namespace shx8x00.Views;

public partial class PortSelectionWindow : Window
{
    public ObservableCollection<string> portList
    {
        get;
        set;
    }

    private string _portName = MySerialPort.getInstance().TargetPort;
    public string portName
    {
        get
        {
            return _portName;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            _portName = value;
        }
    }

    public PortSelectionWindow()
    {
        string[] portNames = SerialPort.GetPortNames();
        portList = new ObservableCollection<string>(portNames);
        if (!portList.Contains(portName))
        {
            portName = "";
            MySerialPort.getInstance().TargetPort = "";
        }
        InitializeComponent();
        DataContext = this;
    }

    private void confirm_OnClick(object? sender, RoutedEventArgs e)
    {
        MySerialPort.getInstance().TargetPort = portName;
        Close();
    }

    private void abortd_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        string[] portNames = SerialPort.GetPortNames();
        portList.Clear();
        foreach (var name in portNames)
        {
            portList.Add(name);
        }
    }
}