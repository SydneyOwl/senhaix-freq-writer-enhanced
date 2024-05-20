using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.HID;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    public DeviceSelectWindow()
    {
        if (DebugWindow.HasInstance())
        {
            DebugWindow.GetInstance().Close();
        }
        SETTINGS.debugEnabled = false;
        InitializeComponent();
        if (HidTools.IsShxGt12HidExist()) DeviceChooseComboBox.SelectedIndex = 1;
    }

    private async void Device_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !Environment.UserName.Equals("root"))
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "请以sudo权限打开软件!").ShowWindowDialogAsync(this);
            Environment.Exit(0);
            return;
        }
        
        if (SETTINGS.debugEnabled)
        {
            DebugWindow.GetNewInstance().Show();
        }

        switch (DeviceChooseComboBox.SelectedIndex)
        {
            case 0:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8800");
                new Shx8x00.MainWindow().Show();
                break;
            case 1:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯GT12");
                new Gt12.MainWindow().Show();
                break;
            default:
                new Shx8x00.MainWindow().Show();
                break;
        }
        Close();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SETTINGS.debugEnabled = ((CheckBox)sender).IsChecked.Value;
    }
}