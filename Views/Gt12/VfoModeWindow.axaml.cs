using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class VfoModeWindow : Window
{
    public VfoModeWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public VfoInfos VfoInfos { get; set; } = AppData.GetInstance().Vfos;

    private void currentFreq_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var num = 0;
        var textBox = (TextBox)sender;
        var flag = false;
        var text = textBox.Text;
        if (text == "")
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "不能为空！").ShowWindowDialogAsync(this);
            textBox.Text = "440.62500";
            return;
        }

        if (text.Replace(".", "").Length > 8)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "精度过高！").ShowWindowDialogAsync(this);
            textBox.Text = "440.62500";
            return;
        }

        if (text != "")
        {
            var text2 = text;
            foreach (var c in text2)
            {
                if (c != '.') continue;

                if (!flag)
                {
                    flag = true;
                    continue;
                }

                MessageBoxManager.GetMessageBoxStandard("注意", "频率格式错误！").ShowWindowDialogAsync(this);
                textBox.Text = "440.62500";
                break;
            }
        }

        if (!(text != "")) return;

        var array = text.Split('.');
        var list = new List<int>();
        for (var j = 0; j < array.Length; j++)
        {
            int res;
            if (!int.TryParse(array[j], out res))
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "格式错误" + Freq.MinFreq + "--" + Freq.MaxFreq)
                    .ShowWindowDialogAsync(this);
                textBox.Text = "440.62500";
                return;
            }

            list.Add(res);
        }

        ;

        if (list[0] < Freq.MinFreq || list[0] >= Freq.MaxFreq)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率错误!\n频率范围:" + Freq.MinFreq + "--" + Freq.MaxFreq)
                .ShowWindowDialogAsync(this);
            textBox.Text = "440.62500";
            return;
        }

        num = list[0] * 100000;
        if (list.Count > 1)
        {
            var num2 = 5 - array[1].Length;
            if (num2 > 0)
                for (var k = 0; k < num2; k++)
                    list[1] *= 10;

            num += list[1];
        }

        if (num % 125 != 0)
        {
            num /= 125;
            num *= 125;
        }

        textBox.Text = num.ToString().Insert(3, ".");
    }

    private void freqOffset_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var flag = false;
        var num = 0;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        var text2 = "";
        if (text == "")
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "不能为空！").ShowWindowDialogAsync(this);
            textBox.Text = "00.0000";
            return;
        }

        if (text.Replace(".", "").Length > 6)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "精度过高！").ShowWindowDialogAsync(this);
            textBox.Text = "00.0000";
            return;
        }

        if (text != "")
        {
            var text3 = text;
            foreach (var c in text3)
            {
                if (c != '.') continue;

                if (!flag)
                {
                    flag = true;
                    continue;
                }

                MessageBoxManager.GetMessageBoxStandard("注意", "频率格式错误!").ShowWindowDialogAsync(this);
                textBox.Text = "00.0000";
                break;
            }
        }

        if (!(text != "")) return;

        var array = text.Split('.');
        var list = new List<int>();
        for (var j = 0; j < array.Length; j++)
        {
            int res;
            if (!int.TryParse(array[j], out res))
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "格式错误" + Freq.MinFreq + "--" + Freq.MaxFreq)
                    .ShowWindowDialogAsync(this);
                textBox.Text = "440.62500";
                return;
            }

            list.Add(res);
        }

        ;

        if (list[0] >= 100)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率错误!\n频率范围:" + Freq.MinFreq + "--" + Freq.MaxFreq)
                .ShowWindowDialogAsync(this);
            textBox.Text = "00.0000";
            return;
        }

        num = list[0] * 10000;
        if (list.Count > 1)
        {
            var num2 = 4 - array[1].Length;
            if (num2 > 0)
                for (var k = 0; k < num2; k++)
                    list[1] *= 10;

            num += list[1];
        }

        if (num % 5 != 0)
        {
            num /= 5;
            num *= 5;
        }

        text2 = num.ToString();
        text2 = num >= 100000
            ? text2.Insert(2, ".")
            : num <= 0
                ? "00.0000"
                : num >= 10000
                    ? "0" + text2.Insert(1, ".")
                    : num >= 1000
                        ? "00." + text2
                        : num >= 100
                            ? "00.0" + text2
                            : num < 10
                                ? "00.000" + text2
                                : "00.00" + text2;
        textBox.Text = text2;
    }
}