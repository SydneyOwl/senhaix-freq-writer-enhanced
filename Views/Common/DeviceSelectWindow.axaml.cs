using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.Utils.HID;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    public DeviceSelectWindow()
    {
        InitializeComponent();
        if (HidTools.IsShxhidExist()) DeviceChooseComboBox.SelectedIndex = 1;
    }

    private async void Device_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !Environment.UserName.Equals("root"))
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "请以sudo权限打开软件!").ShowWindowDialogAsync(this);
            Environment.Exit(0);
            return;
        }

        switch (DeviceChooseComboBox.SelectedIndex)
        {
            case 0:
                new Shx8x00.MainWindow().Show();
                break;
            case 1:
                new Gt12.MainWindow().Show();
                break;
            default:
                new Shx8x00.MainWindow().Show();
                break;
        }

        Close();
    }
}