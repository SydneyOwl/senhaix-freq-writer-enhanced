using System.Collections.ObjectModel;
using System.IO.Ports;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SenhaixFreqWriter.Utils.Serial;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class PortSelectionWindow : Window
{
    private string _portName = MySerialPort.GetInstance().TargetPort;

    public ObservableCollection<string> PortList { get; set; }

    public string PortName
    {
        get => _portName;
        set
        {
            if (string.IsNullOrEmpty(value)) return;

            _portName = value;
        }
    }

    public PortSelectionWindow()
    {
        string[] portNames = SerialPort.GetPortNames();
        PortList = new ObservableCollection<string>(portNames);
        if (!PortList.Contains(PortName))
        {
            PortName = "";
            MySerialPort.GetInstance().TargetPort = "";
        }

        InitializeComponent();
        DataContext = this;
    }

    private void confirm_OnClick(object? sender, RoutedEventArgs e)
    {
        MySerialPort.GetInstance().TargetPort = PortName;
        MySerialPort.GetInstance().WriteBle = null;
        Close();
    }

    private void abortd_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        string[] portNames = SerialPort.GetPortNames();
        PortList.Clear();
        foreach (var name in portNames) PortList.Add(name);
    }
}