﻿using System.Collections.ObjectModel;
using System.IO.Ports;
using Avalonia.Controls;
using Avalonia.Interactivity;
using shx8x00.Utils.Serial;

namespace shx8x00.Views;

public partial class PortSelectionWindow : Window
{
    private string _portName = MySerialPort.getInstance().TargetPort;

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

    public ObservableCollection<string> portList { get; set; }

    public string portName
    {
        get => _portName;
        set
        {
            if (string.IsNullOrEmpty(value)) return;

            _portName = value;
        }
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
        foreach (var name in portNames) portList.Add(name);
    }
}