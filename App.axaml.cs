using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.Args != null)
            {
                if (desktop.Args.Contains("--bypass-root-check"))
                {
                    CMD_SETTINGS.BypassRootCheck = true;
                }

                if (desktop.Args.Contains("--crash-report"))
                {
                    desktop.MainWindow = new ErrorReportWindow();
                }
                else
                {
                    desktop.MainWindow = new DeviceSelectWindow();
                }
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void QuitMenuItem_OnClick(object? sender, EventArgs e)
    {
        Environment.Exit(-1);
    }
}