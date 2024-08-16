using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Other;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Shx8x00;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    private SETTINGS Settings = SETTINGS.Load();
    public DeviceSelectWindow()
    {
        // if (DebugWindow.HasInstance()) DebugWindow.GetInstance().Close();
        InitializeComponent();
        SysFile.CheckDefaultDirectory();
        SysFile.CheckBackupDirectory();
        if (HidTools.IsShxGt12HidExist()) DeviceChooseComboBox.SelectedIndex = 3;
    }

    private async void Device_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !Environment.UserName.Equals("root"))
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "请以sudo权限打开软件!").ShowWindowDialogAsync(this);
            Environment.Exit(0);
            return;
        }
        
        // 提前帮用户选好端口
        _ = Task.Run(()=>
        {
            if (Settings.EnableSelectPortInAdvance)
            {
                try
                {
                    MySerialPort.GetInstance().selectSerialInAdvance();
                }
                catch(Exception qq)
                {
                    DebugWindow.GetInstance().updateDebugContent(qq.Message);
                }
            }
        });

        switch (DeviceChooseComboBox.SelectedIndex)
        {
            case 0:
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("低功率");
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8800");
                new MainWindow(SHX_DEVICE.SHX8800).Show();
                break;
            case 1:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8800新版");
                new Shx8800Pro.MainWindow().Show();
                break;
            case 2:
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add("高功率");
                OptionalChoice.TxPwr.Add("低功率");

                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯8600");
                new MainWindow(SHX_DEVICE.SHX8600).Show();
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
                new MainWindow(SHX_DEVICE.SHX8600PRO).Show();
                break;
            case 4:
                DebugWindow.GetInstance().updateDebugContent("用户选择森海克斯GT12");
                new Gt12.MainWindow().Show();
                break;
            default:
                new MainWindow(SHX_DEVICE.SHX8600).Show();
                break;
        }

        WSRPCUtil.GetInstance().StartWSRPC();
        Close();
    }
}