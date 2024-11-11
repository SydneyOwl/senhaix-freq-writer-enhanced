using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Avalonia;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // 提前设置语言
        Thread.CurrentThread.CurrentUICulture = Settings.Load().LanguageIndex switch
        {
            0 => new CultureInfo("zh"),
            1 => new CultureInfo("en-us"),
            _ => new CultureInfo("zh")
        };
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            File.WriteAllText(CMD_SETTINGS.CrashLogPath, $@"系统环境：{RuntimeInformation.RuntimeIdentifier}, {RuntimeInformation.OSDescription}
类型：{ex.Message}
堆栈：{ex.StackTrace}");
            var executablePath = Process.GetCurrentProcess().MainModule!.FileName;
            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = "--crash-report",
                UseShellExecute = true
            };
            Process.Start(startInfo);
            Environment.Exit(0);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}