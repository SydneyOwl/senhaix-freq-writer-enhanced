using System.Collections.ObjectModel;
using System.IO.Ports;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.Utils.Serial;

namespace SenhaixFreqWriter.Views.Common;

public partial class PortSelectionWindow : Window
{
    private string _portName = MySerialPort.GetInstance().TargetPort;

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

    private void HelpChoose_OnClick(object? sender, RoutedEventArgs e)
    {
        var helpText = @"如果您在开启软件前已接入写频线，同时没有其他的USB串口设备，那么此时软件应该已帮您自动选择好端口了。
如果软件未帮您选择好端口，请您按照以下步骤选择串口：
Windows系统:
    1、确认好您已安装对应写频线的驱动；
    2、打开设备管理器，在“端口”一栏中查找您的写频线，名称应类似于“USB-SERIAL CH340 (COM12)”
    3、回到软件中，点击下拉框选择对应的端口即可。
macOS:
    1、确认您已安装好写频线的驱动（ch340可不用安装）；
    2、移除其他使用串口的设备；
    3、点击下拉框，选择带“cu”的选项，例如：/dev/cu.usbserial-210即可；";
        MessageBoxManager.GetMessageBoxStandard("注意",helpText).ShowWindowDialogAsync(this);
    }
}