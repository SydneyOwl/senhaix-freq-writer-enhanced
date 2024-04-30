using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class OptionalWindow : Window
{
    public Function Func
    {
        get => _func;
        set => _func = value;
    }

    public MDC1200 Mdc
    {
        get => _mdc;
        set => _mdc = value;
    }

    public Function _func = AppData.getInstance().funCfgs;
    public MDC1200 _mdc = AppData.getInstance().mdcs;

    public OptionalWindow()
    {
        InitializeComponent();
        DataContext = this;
    }


    private void CallsignInputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;
        var cont = textbox.Text;
        if (cont.Length > 6)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "最多6位！").ShowWindowDialogAsync(this);
            textbox.Text = "";
            return;
        }

        foreach (var c in cont)
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') ||
                  (c >= 'a' && c <= 'z')))
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "呼号错误！").ShowWindowDialogAsync(this);
                textbox.Text = "";
                return;
            }

        textbox.Text = textbox.Text.ToUpper();
    }

    private void MDCInputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;
        var cont = textbox.Text;
        if (cont.Length > 4)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "最多4位！").ShowWindowDialogAsync(this);
            textbox.Text = "";
            return;
        }

        foreach (var c in cont)
            if ((c < '0' || c > '9') && (c < 'A' || c > 'F'))
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字或大写字母！").ShowWindowDialogAsync(this);
                textbox.Text = "";
                return;
            }
    }
}