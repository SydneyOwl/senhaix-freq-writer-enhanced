using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SHX8X00.Views;

public partial class HintBTWindow : Window
{
    public HintBTWindow()
    {
        InitializeComponent();
    }

    public void setLabelStatus(string stat)
    {
        label.Content += "\n" + stat;
    }

    public void setButtonStatus(bool show)
    {
        button.IsEnabled = show;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}