using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Gt12;

namespace SenhaixFreqWriter.Views.Common;

public partial class DebugWindow : Window
{
    private static DebugWindow instance;

    private DebugWindow()
    {
        InitializeComponent();
        updateDebugContent("开始调试");
        updateDebugContent($"操作系统：{Environment.OSVersion}");
        updateDebugContent($"是否64位：{Environment.Is64BitOperatingSystem}");
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
        if (this.IsActive)
        {
            return;
        }
        base.Show();
    }

    public void updateDebugContent(string content)
    {
        Dispatcher.UIThread.Post(() =>
        {
            tbContent.Text += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{content}\n";
            ScViewer.ScrollToEnd();
        });
    }

    public static DebugWindow GetInstance()
    {
        if (instance == null) instance = new DebugWindow();

        return instance;
    }

    public static DebugWindow GetNewInstance()
    {
        instance?.Close();
        instance = new DebugWindow();
        return instance;
    }

    public static bool HasInstance()
    {
        return instance != null;
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
                File.WriteAllText(filePath, instance.tbContent.Text);
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "保存失败！");
            }
        }
    }
}