using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
            desktop.MainWindow = new DeviceSelectWindow();

        base.OnFrameworkInitializationCompleted();
    }

    private void QuitMenuItem_OnClick(object? sender, EventArgs e)
    {
        Environment.Exit(-1);
    }
}