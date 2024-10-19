using System;
using System.Globalization;
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
            0 => new CultureInfo("zh-hans"),
            1 => new CultureInfo("en-us"),
            _ => new CultureInfo("zh-hans")
        };
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
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