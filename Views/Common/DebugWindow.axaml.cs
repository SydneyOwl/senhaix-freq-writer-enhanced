using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;

namespace SenhaixFreqWriter.Views.Common;

public partial class DebugWindow : Window
{
    private static DebugWindow _instance;

    private DebugWindow()
    {
        InitializeComponent();
        UpdateDebugContent("开始调试");
        UpdateDebugContent($"操作系统：{Environment.OSVersion}");
        UpdateDebugContent($"是否64位：{Environment.Is64BitOperatingSystem}");
        Closing += (sender, args) =>
        {
            args.Cancel = true;
            Hide();
        };
    }

    // public DebugWindow()
    // {
    //     InitializeComponent();
    // }

    public override void Show()
    {
        if (IsActive) return;
        base.Show();
    }

    public void UpdateDebugContent(string content)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            tbContent.Text += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{content}\n";
            ScViewer.ScrollToEnd();
        });
    }

    public static DebugWindow GetInstance()
    {
        if (_instance == null) _instance = new DebugWindow();

        return _instance;
    }

    public static DebugWindow GetNewInstance()
    {
        _instance?.Close();
        _instance = new DebugWindow();
        return _instance;
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

    private async void LogSaveInputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存配置文件",
            SuggestedFileName = "Log-" + ts + ".txt"
        });
        if (file is not null)
        {
            var filePath = new Uri(file.Path.ToString()).LocalPath;
            try
            {
                File.WriteAllText(filePath, _instance.tbContent.Text);
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "保存失败！");
            }
        }
    }
}