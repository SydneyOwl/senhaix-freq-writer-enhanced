using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OfficeOpenXml;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Name = "SenhaixFreqWriter";
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            if (desktop.Args != null)
            {
                for (var i = 0; i < desktop.Args.Length; i++)
                {
                    if (desktop.Args[i] == "--bypass-root-check")
                    {
                        CMD_SETTINGS.BypassRootCheck = true;
                        continue;
                    }

                    if (desktop.Args[i] == "--crash-report")
                    {
                        desktop.MainWindow = new ErrorReportWindow(desktop.Args[i + 1]);
                        return;
                    }
                }

                desktop.MainWindow = new DeviceSelectWindow();

                try
                {
                    ExcelPackage.License.SetNonCommercialPersonal("SydneyOwl");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This system does not support excelpackage.");
                }
            }

        base.OnFrameworkInitializationCompleted();
    }

    private void QuitMenuItem_OnClick(object? sender, EventArgs e)
    {
        Environment.Exit(-1);
    }
}