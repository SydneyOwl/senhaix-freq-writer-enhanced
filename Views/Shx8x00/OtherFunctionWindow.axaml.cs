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
    public OtherImfData _imf = ClassTheRadioData.getInstance().otherImfData;

    public OtherImfData Imf
    {
        get => _imf;
        set => _imf = value;
    }

    public OtherFunctionWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void WriteConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        if (check() == 0)
        {
            await new ProgressBarWindow(OPERATION_TYPE.WRITE_CONFIG).ShowDialog(this);
        }
    }
    private async void ReadConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        if (check() == 0)
        {
            await new ProgressBarWindow(OPERATION_TYPE.READ_CONFIG).ShowDialog(this);
        }
    }
    private int check()
    {
        // Check frequency! DO NOT BYPASS THIS CHECK!! USE AT YOUR OWN RISK!!
        var minUHF = UhfMinTextBox.Text;
        var maxUHF = UhfMaxTextBox.Text;
        var minVHF = VhfMinTextBox.Text;
        var maxVHF = VhfMaxTextBox.Text;

        int minUHFTmp;
        int maxUHFTmp;
        int minVHFTmp;
        int maxVHFTmp;
        if (int.TryParse(minUHF, out minUHFTmp))
        {
            if (minUHFTmp < 400)
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
        if (int.TryParse(maxUHF, out maxUHFTmp))
        {
            if (maxUHFTmp > 520)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "U段发射频率必须低于520！").ShowWindowDialogAsync(this);
                return -1;
            }
        }else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "UHF最大值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }
        if (int.TryParse(minVHF, out minVHFTmp))
        {
            if (minVHFTmp < 136)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "V段发射频率必须高于136！").ShowWindowDialogAsync(this);
                return -1;
            }
        }else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "VHF最小值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }
        if (int.TryParse(maxVHF, out maxVHFTmp))
        {
            if (maxVHFTmp > 174)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "V段发射频率必须低于174！").ShowWindowDialogAsync(this);
                return -1;
            }
        }else
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "VHF最大值输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }
        if (!((minUHFTmp is >= 400 and <= 520)&&(maxUHFTmp is >= 400 and <= 520)))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "U段输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }
        if (!((minVHFTmp is >= 136 and <= 174)&&(maxVHFTmp is >= 136 and <= 174)))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "V段输入错误！！").ShowWindowDialogAsync(this);
            return-1;
        }
        if (!((minUHFTmp < maxUHFTmp) && (minVHFTmp < maxVHFTmp)))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "大小段输入错误！！").ShowWindowDialogAsync(this);
            return -1;
        }

        return 0;
    }
    
    private void tb_PowerUpChar_TextChanged(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string text = textBox.Text;
        foreach (var c in text)
        {
            if ((c < '0' || c > '9') && (c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c!='-')
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "只能是数字和字母！").ShowWindowDialogAsync(this);
                textBox.Text = "";
                return;
            }
        }
    }
}