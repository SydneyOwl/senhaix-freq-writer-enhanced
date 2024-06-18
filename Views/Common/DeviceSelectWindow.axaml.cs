using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Views.Shx8x00;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    public DeviceSelectWindow()
    {
        // if (DebugWindow.HasInstance()) DebugWindow.GetInstance().Close();
        InitializeComponent();
        if (HidTools.IsShxGt12HidExist()) DeviceChooseComboBox.SelectedIndex = 2;
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
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("低功率");
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8x00");
                new MainWindow().Show();
                break;
            case 1:
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("M");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("中功率");
                OptionalChoice.TxPwr.Add("低功率");
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8600新版");
                new MainWindow().Show();
                break;
            case 2:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯GT12");
                new Gt12.MainWindow().Show();
                break;
            default:
                new MainWindow().Show();
                break;
        }

        WSRPCUtil.GetInstance().StartWSRPC();
        Close();
    }
}