using System;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class DTMFWindow : Window
{
    private ObservableCollection<DTMPObject> _dtmfs = new();

    public ObservableCollection<DTMPObject> Dtmfs
    {
        get => _dtmfs;
        set => _dtmfs = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DTMFWindow()
    {
        InitializeComponent();
        DataContext = this;
        var dtmfOrig = AppData.getInstance().dtmfs;
        for (int i = 0; i < 20; i++)
        {
            var tmp = new DTMPObject();
            tmp.Id = (i+1).ToString();
            tmp.GroupName = dtmfOrig.GroupName[i];
            tmp.Group = dtmfOrig.Group[i];
            Dtmfs.Add(tmp);
        }

        Closing += (async (sender, args) =>
        {
            for (int i = 0; i < 20; i++)
            {
                if (string.IsNullOrEmpty(Dtmfs[i].Group) || string.IsNullOrEmpty(Dtmfs[i].GroupName))
                {
                    args.Cancel = true;
                    await MessageBoxManager.GetMessageBoxStandard("注意", "未填写完整，不能有为空的字段！")
                        .ShowWindowDialogAsync(this);
                    return;
                }
            }

            for (int j = 0; j <20; j++)
            {
                AppData.getInstance().dtmfs.Group[j] = Dtmfs[j].Group;
                AppData.getInstance().dtmfs.GroupName[j] = Dtmfs[j].GroupName;
            }
        });
    }

    private void GroupCodeInputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;
        var inputText = textbox.Text;
        if (inputText.Length > 8)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "最多8位！").ShowWindowDialogAsync(this);
            textbox.Text = "";
            return;
        }
        foreach (var c in inputText)
        {
            if ((c < '0' || c > '9') && (c < 'A' || c > 'D') && c != '*' && c != '#')
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "码只能是数字、大写字母以及*#").ShowWindowDialogAsync(this);
                textbox.Text = "";
                return;
            }
        }
    }

    private void GroupNameInputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;
        var inputText = textbox.Text;
        var bytes = Encoding.GetEncoding("gb2312").GetBytes(inputText);
        if (bytes.Length > 12) textbox.Text = Encoding.GetEncoding("gb2312").GetString(bytes, 0, 12);
    }
}