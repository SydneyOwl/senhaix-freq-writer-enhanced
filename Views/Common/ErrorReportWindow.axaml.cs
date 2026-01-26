using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SenhaixFreqWriter.Views.Common;

public partial class ErrorReportWindow : Window
{
    private readonly string? _errContent = "";

    public ErrorReportWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public ErrorReportWindow(string errLogPath)
    {
        // 获取出错信息
        try
        {
            _errContent = File.ReadAllText(errLogPath);
            File.Delete(errLogPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(@"Unable to read crash log...");
            Environment.Exit(-1);
        }

        InitializeComponent();
        DataContext = this;
        ErrBlock.Text = _errContent;
    }

    private async void LogSaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存日至",
            SuggestedFileName = "CrashLog-" + ts + ".log"
        });
        if (file is not null)
        {
            var openWriteStream = file.OpenWriteAsync().Result;
            var st = new StreamWriter(openWriteStream);
            await st.WriteAsync(_errContent);
            st.Close();
        }
    }

    private void ExitWindowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(-1);
    }
}