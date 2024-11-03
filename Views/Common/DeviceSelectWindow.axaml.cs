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
    private Settings _settings = Settings.Load();

    public DeviceSelectWindow()
    {
        // if (DebugWindow.HasInstance()) DebugWindow.GetInstance().Close();
        InitializeComponent();
        SysFile.CheckDefaultDirectory();
        SysFile.CheckBackupDirectory();
        if (HidTools.IsShxGt12HidExist()) DeviceChooseComboBox.SelectedIndex = 4;
    }

    private async void Device_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!CMD_SETTINGS.BypassRootCheck && RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !Environment.UserName.Equals("root"))
        {
            await MessageBoxManager.GetMessageBoxStandard(Language.GetString("warning"), Language.GetString("sudo"))
                .ShowWindowDialogAsync(this);
            Environment.Exit(0);
            return;
        }

        // 提前帮用户选好端口
        _ = Task.Run(() =>
        {
            if (!_settings.EnableSelectPortInAdvance) return;
            try
            {
                MySerialPort.GetInstance().SelectSerialInAdvance();
            }
            catch (Exception qq)
            {
                DebugWindow.GetInstance().UpdateDebugContent(qq.Message);
            }
        });

        switch (DeviceChooseComboBox.SelectedIndex)
        {
            case 0:
                DataModels.Shx8x00.ClassTheRadioData.Instance = null!;
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_high"));
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_low"));
                DebugWindow.GetInstance().UpdateDebugContent("用户选择森海克斯8800");
                new MainWindow(ShxDevice.Shx8800).Show();
                break;
            case 1:
                DataModels.Shx8800Pro.AppData.Instance = null!;
                DebugWindow.GetInstance().UpdateDebugContent("用户选择森海克斯8800新版");
                new Shx8800Pro.MainWindow().Show();
                break;
            case 2:
                DataModels.Shx8x00.ClassTheRadioData.Instance = null!;
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_high"));
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_low"));

                DebugWindow.GetInstance().UpdateDebugContent("用户选择森海克斯8600");
                new MainWindow(ShxDevice.Shx8600).Show();
                break;
            case 3:
                DataModels.Shx8x00.ClassTheRadioData.Instance = null!;
                ChanChoice.TxPwr.Clear();
                ChanChoice.TxPwr.Add("L");
                ChanChoice.TxPwr.Add("M");
                ChanChoice.TxPwr.Add("H");

                OptionalChoice.TxPwr.Clear();
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_high"));
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_mid"));
                OptionalChoice.TxPwr.Add(Language.GetString("pwr_low"));
                DebugWindow.GetInstance().UpdateDebugContent("用户选择森海克斯8600新版");
                new MainWindow(ShxDevice.Shx8600Pro).Show();
                break;
            case 4:
                DataModels.Gt12.AppData.Instance = null!;
                DebugWindow.GetInstance().UpdateDebugContent("用户选择森海克斯GT12");
                new Gt12.MainWindow().Show();
                break;
            default:
                DataModels.Shx8x00.ClassTheRadioData.Instance = null!;
                new MainWindow(ShxDevice.Shx8600).Show();
                break;
        }

        WsrpcUtil.GetInstance().StartWsrpc();
        Close();
    }
}