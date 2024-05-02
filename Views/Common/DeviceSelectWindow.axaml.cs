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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "请您确认是否已经以sudo权限打开软件~如果是请忽略").ShowWindowDialogAsync(this);
        }
        InitializeComponent();
        if (HIDTools.isSHXHIDExist())
        {
            DeviceChooseComboBox.SelectedIndex = 1;
        }
    }

    private void Device_OnClick(object? sender, RoutedEventArgs e)
    {
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