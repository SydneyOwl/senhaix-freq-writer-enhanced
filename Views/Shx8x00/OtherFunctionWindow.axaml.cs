using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class OtherFunctionWindow : Window
{
    public OtherImfData Imf { get; set; } = ClassTheRadioData.GetInstance().OtherImfData;

    public OtherFunctionWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void WriteConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Check() == 0) await new ProgressBarWindow(OperationType.WriteConfig).ShowDialog(this);
    }

    private async void ReadConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Check() == 0) await new ProgressBarWindow(OperationType.ReadConfig).ShowDialog(this);
    }

    private int Check()
    {
        // Check frequency! DO NOT BYPASS THIS CHECK!! USE AT YOUR OWN RISK!!
        var minUhf = UhfMinTextBox.Text;
        var maxUhf = UhfMaxTextBox.Text;
        var minVhf = VhfMinTextBox.Text;
        var maxVhf = VhfMaxTextBox.Text;

        int minUhfTmp;
        int maxUhfTmp;
        int minVhfTmp;
        int maxVhfTmp;
        if (int.TryParse(minUhf, out minUhfTmp))
        {
            if (minUhfTmp < 400)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "U段发射频率必须高于400！").ShowWindowDialogAsync(this);
                return -1;
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "UHF最小值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (int.TryParse(maxUhf, out maxUhfTmp))
        {
            if (maxUhfTmp > 520)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "U段发射频率必须低于520！").ShowWindowDialogAsync(this);
                return -1;
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "UHF最大值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (int.TryParse(minVhf, out minVhfTmp))
        {
            if (minVhfTmp < 136)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "V段发射频率必须高于136！").ShowWindowDialogAsync(this);
                return -1;
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "VHF最小值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (int.TryParse(maxVhf, out maxVhfTmp))
        {
            if (maxVhfTmp > 174)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "V段发射频率必须低于174！").ShowWindowDialogAsync(this);
                return -1;
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "VHF最大值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (!(minUhfTmp is >= 400 and <= 520 && maxUhfTmp is >= 400 and <= 520))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "U段输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (!(minVhfTmp is >= 136 and <= 174 && maxVhfTmp is >= 136 and <= 174))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "V段输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        if (!(minUhfTmp < maxUhfTmp && minVhfTmp < maxVhfTmp))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "大小段输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        return 0;
    }

    private void tb_PowerUpChar_TextChanged(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        foreach (var c in text)
            if ((c < '0' || c > '9') && (c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '-')
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字和字母！").ShowWindowDialogAsync(this);
                textBox.Text = "";
                return;
            }
    }
}