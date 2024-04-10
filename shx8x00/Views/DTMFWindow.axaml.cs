using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using shx8x00.DataModels;

namespace shx8x00.Views;

public partial class DTMFWindow : Window
{
    public DTMFData _dtmf = ClassTheRadioData.getInstance().dtmfData;

    public DTMFData dtmf
    {
        get { return _dtmf; }
        set
        {
            ClassTheRadioData.getInstance().dtmfData = value;
            _dtmf = value;
        }
    }
    public DTMFWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void restore_OnClick(object? sender, RoutedEventArgs e)
    {
        ClassTheRadioData.getInstance().dtmfData = new DTMFData();
        Close();
        var newWindow = new DTMFWindow();
        newWindow.Show();
    }

    private void confirm_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}