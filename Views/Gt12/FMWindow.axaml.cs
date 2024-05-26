using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class FmWindow : Window
{
    private ObservableCollection<FmObject> _fmchannels = new();

    private string _curFreq = AppData.GetInstance().Fms.CurFreq.ToString()
        .Insert(AppData.GetInstance().Fms.CurFreq.ToString().Length - 1, ".");

    public ObservableCollection<FmObject> Fmchannels
    {
        get => _fmchannels;
        set => _fmchannels = value;
    }

    public string CurFreq
    {
        get => _curFreq;
        set => _curFreq = value;
    }

    public FmWindow()
    {
        InitializeComponent();
        DataContext = this;
        for (var i = 0; i < AppData.GetInstance().Fms.Channels.Length; i++)
        {
            var tmp = new FmObject();
            tmp.Id = i + 1;
            var freq = AppData.GetInstance().Fms.Channels[i].ToString();
            if (freq != "0")
            {
                tmp.Freq = freq.Insert(freq.Length - 1, ".");
                ;
            }

            Fmchannels.Add(tmp);
        }

        ;
        Closing += (sender, args) =>
        {
            for (var i = 0; i < AppData.GetInstance().Fms.Channels.Length; i++)
            {
                var tmp = Fmchannels[i].Freq.Replace(".", "");
                int cache;
                if (!int.TryParse(tmp, out cache)) continue;
                AppData.GetInstance().Fms.Channels[i] = cache;
            }

            if (string.IsNullOrEmpty(CurFreq)) return;
            AppData.GetInstance().Fms.CurFreq = int.Parse(CurFreq.Replace(".", ""));
        };
    }

    private void CalcSeq()
    {
        for (var i = 0; i < AppData.GetInstance().Fms.Channels.Length; i++) Fmchannels[i].Id = i;
    }

    private void Freq_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
    }

    private void FreqInputElement_OnLostFocus(object? sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var dataContext = textBox.DataContext as FmObject;
        var text = dataContext.Freq;
        double fmfreq;
        if (string.IsNullOrEmpty(text)) return;
        if (!double.TryParse(text, out fmfreq))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率格式错误！").ShowWindowDialogAsync(this);
            textBox.Text = "";
            return;
        }

        if (text.Length < 2 || (text.Length == 2 && text.StartsWith("10"))) return;
        text = fmfreq.ToString("0.0");

        var array = text.Split('.');
        var list = new List<int>();
        int num3;
        list.Add(int.Parse(array[0]));
        if (array.Length > 1)
        {
            list.Add(int.Parse(array[1]));
            num3 = list[0] * 10 + list[1];
        }
        else
        {
            num3 = list[0] * 10;
        }

        if (num3 < 650 || num3 > 1080)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率要在65.0-108.0之间").ShowWindowDialogAsync(this);
            textBox.Text = "";
            return;
        }

        text = num3.ToString();
        textBox.Text = text.Insert(text.Length - 1, ".");
    }

    private void currFreqInputElement_OnLostFocus(object? sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        double fmfreq;
        if (string.IsNullOrEmpty(text)) return;
        if (!double.TryParse(text, out fmfreq))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率格式错误！").ShowWindowDialogAsync(this);
            textBox.Text = "";
            return;
        }

        if (text.Length < 2 || (text.Length == 2 && text.StartsWith("10"))) return;
        text = fmfreq.ToString("0.0");

        var array = text.Split('.');
        var list = new List<int>();
        int num3;
        list.Add(int.Parse(array[0]));
        if (array.Length > 1)
        {
            list.Add(int.Parse(array[1]));
            num3 = list[0] * 10 + list[1];
        }
        else
        {
            num3 = list[0] * 10;
        }

        if (num3 < 650 || num3 > 1080)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "频率要在65.0-108.0之间").ShowWindowDialogAsync(this);
            textBox.Text = "";
            return;
        }

        text = num3.ToString();
        textBox.Text = text.Insert(text.Length - 1, ".");
    }
}