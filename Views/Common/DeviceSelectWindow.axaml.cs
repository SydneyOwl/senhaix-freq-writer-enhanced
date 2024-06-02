using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    public DeviceSelectWindow()
    {
        if (DebugWindow.HasInstance()) DebugWindow.GetInstance().Close();
        SETTINGS.DEBUG_ENABLED = false;
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

        if (SETTINGS.DEBUG_ENABLED) DebugWindow.GetNewInstance().Show();
        
        switch (DeviceChooseComboBox.SelectedIndex)
        {
            case 0:
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");
        
                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("低功率");
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8x00");
                new Shx8x00.MainWindow().Show();
                break;
            case 1:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯GT12");
                new Gt12.MainWindow().Show();
                break;
            case 3:
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("M");
                ChanChoice.TxPwr.Add("H");
        
                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("中功率");
                OptionalChoice.TxPwr.Add("低功率");
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8600新版");
                new Shx8x00.MainWindow().Show();
                break;
            default:
                new Shx8x00.MainWindow().Show();
                break;
        }
        WSRPCUtil.GetInstance().StartWSRPC();
        Close();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SETTINGS.DEBUG_ENABLED = ((CheckBox)sender).IsChecked.Value;
    }
}