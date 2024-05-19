using Avalonia.Controls;
using Avalonia.Interactivity;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter.Views.Common;

public partial class DispInfoWindow : Window
{
    public DispInfoWindow()
    {
        InitializeComponent();
    }

    public void SetLabelStatus(string stat,bool debugInfo = false)
    {
        if (!debugInfo)
        {
            tbContent.Text += "\n" + stat;
        }
        else
        {
            DebugWindow.GetInstance().updateDebugContent(stat);
        }
        Viewer.ScrollToEnd();
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