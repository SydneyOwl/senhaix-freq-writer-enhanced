using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SenhaixFreqWriter.Views.Common;

public partial class DeviceSelectWindow : Window
{
    public DeviceSelectWindow()
    {
        InitializeComponent();
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