using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Views.Common;

public partial class SettingsWindow : Window
{
    // 有空再改
    public Settings Settings { get; set; } = Settings.Load();

    public SettingsWindow()
    {
        // Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh"); 
        InitializeComponent();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            btPluginNameTextbox.Text = Settings.WindowsBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) btPluginNameTextbox.Text = Settings.LinuxBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) btPluginNameTextbox.Text = Settings.OsXBlePluginName;
        DataContext = this;
        LanguageChooseComboBox.SelectedIndex = Thread.CurrentThread.CurrentUICulture.Name.ToLower() switch
        {
            "zh" => 0,
            "en-us" => 1,
            _ => 0
        };
    }

    private void SaveConfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Settings.WindowsBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) Settings.LinuxBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Settings.OsXBlePluginName = btPluginNameTextbox.Text;
        Settings.Save();
        // 切换语言
        Thread.CurrentThread.CurrentUICulture = LanguageChooseComboBox.SelectedIndex switch
        {
            0 => new CultureInfo("zh"),
            1 => new CultureInfo("en-us"),
            _ => new CultureInfo("zh")
        };
        Close();
    }

    private async void ChoosePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false
        });
        if (files.Count == 0) return;
        Settings.DataDir = files[0].Path.LocalPath;
    }

    private void AbortButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenBackupButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start("explorer.exe", $"\"{Settings.GetBackupPath()}\"");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", $"\"{Settings.GetBackupPath()}\"");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"\"{Settings.GetBackupPath()}\"");
        }
        catch (Exception ee)
        {
            MessageBoxManager
                .GetMessageBoxStandard(Language.GetString("warning"), Language.GetString("failed") + ee.Message)
                .ShowWindowDialogAsync(this);
            DebugWindow.GetInstance().UpdateDebugContent(ee.Message);
        }
    }
}