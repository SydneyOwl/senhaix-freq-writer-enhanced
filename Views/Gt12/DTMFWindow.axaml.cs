using System;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class DtmfWindow : Window
{
    private ObservableCollection<DtmpObject> _dtmfs = new();

    private int _idleTime = AppData.GetInstance().Dtmfs.IdleTime;

    private string _myId = AppData.GetInstance().Dtmfs.LocalId;

    private int _wordTime = AppData.GetInstance().Dtmfs.WordTime;

    public DtmfWindow()
    {
        InitializeComponent();
        DataContext = this;
        var dtmfOrig = AppData.GetInstance().Dtmfs;
        for (var i = 0; i < 20; i++)
        {
            var tmp = new DtmpObject();
            tmp.Id = (i + 1).ToString();
            tmp.GroupName = dtmfOrig.GroupName[i];
            tmp.Group = dtmfOrig.Group[i];
            Dtmfs.Add(tmp);
        }

        Closing += async (sender, args) =>
        {
            for (var i = 0; i < 20; i++)
                if (string.IsNullOrEmpty(Dtmfs[i].Group) || string.IsNullOrEmpty(Dtmfs[i].GroupName))
                {
                    args.Cancel = true;
                    DebugWindow.GetInstance().UpdateDebugContent("阻止窗口关闭：有空字段");
                    await MessageBoxManager.GetMessageBoxStandard("注意", "未填写完整，不能有为空的字段！")
                        .ShowWindowDialogAsync(this);
                    return;
                }

            for (var j = 0; j < 20; j++)
            {
                AppData.GetInstance().Dtmfs.Group[j] = Dtmfs[j].Group;
                AppData.GetInstance().Dtmfs.GroupName[j] = Dtmfs[j].GroupName;
            }
        };
    }

    public int WordTime
    {
        get => _wordTime;
        set
        {
            _wordTime = value;
            AppData.GetInstance().Dtmfs.WordTime = value;
        }
    }

    public int IdleTime
    {
        get => _idleTime;
        set
        {
            _idleTime = value;
            AppData.GetInstance().Dtmfs.IdleTime = value;
        }
    }


    public string MyId
    {
        get => _myId;
        set
        {
            _myId = value ?? throw new ArgumentNullException(nameof(value));
            AppData.GetInstance().Dtmfs.LocalId = value;
        }
    }

    public ObservableCollection<DtmpObject> Dtmfs
    {
        get => _dtmfs;
        set => _dtmfs = value ?? throw new ArgumentNullException(nameof(value));
    }

    private void GroupCodeInputElement_OnLostFocus(object? sender, TextChangedEventArgs e)
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
            if ((c < '0' || c > '9') && (c < 'A' || c > 'D') && c != '*' && c != '#')
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "码只能是数字、大写字母以及*#").ShowWindowDialogAsync(this);
                textbox.Text = "";
                return;
            }
    }

    private void GroupNameInputElement_OnLostFocus(object? sender, TextChangedEventArgs e)
    {
        var textbox = (TextBox)sender;
        var inputText = textbox.Text;
        var bytes = Encoding.GetEncoding("gb2312").GetBytes(inputText);
        if (bytes.Length > 12) textbox.Text = Encoding.GetEncoding("gb2312").GetString(bytes, 0, 12);
    }
}