using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

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
    }

    public void updateDebugContent(string content)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            tbContent.Text += $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}]{content}\n";
        });
        ScViewer.ScrollToEnd();
    }
    
    public static DebugWindow GetInstance()
    {
        if (instance == null)
        {
            instance = new DebugWindow();
        }

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
        return instance!=null;
    }
}