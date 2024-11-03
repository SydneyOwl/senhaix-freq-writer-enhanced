using System;
using System.Linq;
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
            desktop.MainWindow = new DeviceSelectWindow();
            if (desktop.Args != null)
            {
                if (desktop.Args.Contains("--bypass-root-check"))
                {
                    CMD_SETTINGS.BypassRootCheck = true;
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