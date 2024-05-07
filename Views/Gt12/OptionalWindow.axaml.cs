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
    public Function Func { get; set; } = AppData.GetInstance().FunCfgs;
    public Mdc1200 Mdc { get; set; } = AppData.GetInstance().Mdcs;

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