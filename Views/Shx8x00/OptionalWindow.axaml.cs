using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.DataModels.Shx8x00;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class OptionalWindow : Window
{
    private SHX_DEVICE _device = SHX_DEVICE.SHX8600;

    public OptionalWindow(SHX_DEVICE device)
    {
        _device = device;
        InitializeComponent();
        if (device == SHX_DEVICE.SHX8800)
        {
            alarmEndCombo.IsEnabled = true;
            alarmModeCombo.IsEnabled = true;
            alarmSoundCombo.IsEnabled = true;
            rptFrqCombo.IsEnabled = true;
        }
        else
        {
            // 在8600和pro上该项不可用，强制覆盖
            Fun.CbB1750Hz = 0;
        }

        DataContext = this;
    }

    public OptionalWindow()
    {
        InitializeComponent();
        alarmEndCombo.IsEnabled = true;
        alarmModeCombo.IsEnabled = true;
        alarmSoundCombo.IsEnabled = true;
        rptFrqCombo.IsEnabled = true;
        DataContext = this;
    }


    public FunCfgData Fun { get; set; } = ClassTheRadioData.GetInstance().FunCfgData;

    private void close_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void restore_OnClick(object? sender, RoutedEventArgs e)
    {
        ClassTheRadioData.GetInstance().FunCfgData = new FunCfgData();
        var tmp = new OptionalWindow(_device);
        Close();
        tmp.Show();
    }

    private string ParseCurFreq(string frq)
    {
        var texter = frq;
        if (!string.IsNullOrEmpty(texter))
        {
            var num = double.Parse(texter);
            var flag = false;
            if (num < 100.0 || num >= 520.0)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "范围 100 - 520MHz").ShowWindowDialogAsync(this);
                return "400.12500";
            }

            var length = texter.Length;
            var text = texter;
            if (length <= 9 && length >= 4)
            {
                for (var i = 0; i < 9 - length; i++) text += "0";
            }
            else
            {
                text += ".";
                for (var j = 0; j < 9 - (length + 1); j++) text += "0";
            }

            var num2 = double.Parse(text) * 100000.0;
            if (num2 % 625.0 != 0.0 && num2 % 500.0 != 0.0)
            {
                var num3 = (short)(num2 % 625.0);
                var num4 = (short)(num2 % 500.0);
                short num5 = 0;
                num5 = (short)(num3 < num4 ? 625 : 500);
                var num6 = (int)(num2 / num5);
                num2 = num6 * num5;
                var text2 = (num2 / 100000.0).ToString();
                for (var k = text2.Length; k < 9; k++) text2 = k != 3 ? text2.Insert(k, "0") : text2.Insert(k, ".");

                texter = text2;
            }
            else
            {
                texter = text;
            }
        }
        else
        {
            texter = "400.12500";
        }

        return texter;
    }

    private string ParsePinFreq(string freq)
    {
        var text = freq;
        var num = text.IndexOf('.');
        if (text != "")
        {
            if (num != -1)
            {
                if (num == 0) text = "0" + text;

                var num2 = double.Parse(text);
                var num3 = (int)(num2 * 10000.0);
                num3 /= 5;
                num3 *= 5;
                if (num3 > 100000)
                {
                    text = num3.ToString().Insert(2, ".");
                }
                else if (num3 > 0)
                {
                    if (num3 < 10000)
                    {
                        text = num3.ToString();
                        text = num3 >= 1000
                            ? "00." + text
                            : num3 >= 100
                                ? "00.0" + text
                                : num3 < 10
                                    ? "00.000" + text
                                    : "00.00" + text;
                    }
                    else
                    {
                        text = "0" + num3.ToString().Insert(1, ".");
                    }
                }
                else
                {
                    text = "00.0000";
                }


                return text;
            }

            var num4 = int.Parse(text);
            if (num4 > 99)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "范围 100 - 520MHz").ShowWindowDialogAsync(this);
                return "00.0000";
            }

            if (num4 > 0)
            {
                text = (num4 * 1000).ToString();
                text.Insert(text.Length - 3, ".");
            }
            else
            {
                text = "00.0000";
            }

            return text;
        }

        return "00.0000";
    }

    private void A_Freq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ClassTheRadioData.GetInstance().FunCfgData.TBACurFreq = ParseCurFreq(textBox.Text);
    }

    private void B_Freq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ClassTheRadioData.GetInstance().FunCfgData.TBBCurFreq = ParseCurFreq(textBox.Text);
    }


    private void A_RmFq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ClassTheRadioData.GetInstance().FunCfgData.TBARemainFreq = ParsePinFreq(textBox.Text);
    }

    private void B_RmFq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ClassTheRadioData.GetInstance().FunCfgData.TBBRemainFreq = ParsePinFreq(textBox.Text);
    }
}