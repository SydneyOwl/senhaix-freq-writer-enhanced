using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Shx8x00;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class DTMFWindow : Window
{
    public DTMFData _dtmf = ClassTheRadioData.getInstance().dtmfData;

    public DTMFWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public DTMFData dtmf
    {
        get => _dtmf;
        set
        {
            ClassTheRadioData.getInstance().dtmfData = value;
            _dtmf = value;
        }
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

    private async void idhost_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (string.IsNullOrEmpty(text)) return;
        if (text.Length > 5)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "最多5位！").ShowWindowDialogAsync(this);
            ClassTheRadioData.getInstance().dtmfData.TheIDOfLocalHost = "80808";
            return;
        }

        if ((text[text.Length - 1] < '0' || text[text.Length - 1] > '9') && text.Length == 5)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "最后一位只能是数字").ShowWindowDialogAsync(this);
            ClassTheRadioData.getInstance().dtmfData.TheIDOfLocalHost = "80808";
            return;
        }

        if (text[text.Length - 1] >= 'a' && text[text.Length - 1] <= 'd')
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字/A-D的字母/*/#").ShowWindowDialogAsync(this);
            textBox.Text = "";
        }

        //TODO 优化
        foreach (var c in textBox.Text)
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'D') ||
                  (c >= 'a' && c <= 'd') || c == '*' || c == '#' || c == '\b'))
            {
                await MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字/A-D的字母/*/#").ShowWindowDialogAsync(this);
                textBox.Text = "";
            }
    }


    private void Input_OnKeyDown(object? sender, KeyEventArgs e)
    {
        // if (string.IsNullOrEmpty(e.KeySymbol))
        // {
        //     return;
        // }
        // var key = e.KeySymbol[0];
        // if ((key >= '0' && key <= '9') || (key >= 'A' && key <= 'D') ||
        //     (key >= 'a' && key <= 'd') || key == '*' || key== '#' || key == '\b')
        // {
        //     if (key >= 'a' && key <= 'd') MessageBoxManager.GetMessageBoxStandard("注意", "只能输入大写字母").ShowWindowDialogAsync(this);
        // }
        // else
        // {
        //     e.Handled = true;
        // }
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text)) return;
        if (textBox.Text.Length > 5)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "最多5位").ShowWindowDialogAsync(this);
            textBox.Text = "";
            return;
        }

        var key = textBox.Text[textBox.Text.Length - 1];
        if ((key >= '0' && key <= '9') || (key >= 'A' && key <= 'D') ||
            (key >= 'a' && key <= 'd') || key == '*' || key == '#' || key == '\b')
        {
            if (key >= 'a' && key <= 'd')
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "只能输入大写A-D字母").ShowWindowDialogAsync(this);
                textBox.Text = "";
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字/A-D的字母/*/#").ShowWindowDialogAsync(this);
            textBox.Text = "";
        }

        //TODO 优化
        foreach (var c in textBox.Text)
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'D') ||
                  (c >= 'a' && c <= 'd') || c == '*' || c == '#' || c == '\b'))
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字/A-D的字母/*/#").ShowWindowDialogAsync(this);
                textBox.Text = "";
            }
    }
}