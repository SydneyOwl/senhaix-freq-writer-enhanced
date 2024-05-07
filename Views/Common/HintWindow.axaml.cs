using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SenhaixFreqWriter.Views.Common;

public partial class HintWindow : Window
{
    public HintWindow()
    {
        InitializeComponent();
    }

    public void SetLabelStatus(string stat)
    {
        label.Text += "\n" + stat;
    }

    public void SetButtonStatus(bool show)
    {
        button.IsEnabled = show;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}