using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SHX8X00.Views;

public partial class HintBT : Window
{
    public HintBT()
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