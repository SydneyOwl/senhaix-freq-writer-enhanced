using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using Newtonsoft.Json;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Views.Common;

public partial class SettingsWindow : Window
{
    public Settings ScopeSettings { get; set; } = Settings.Load();

    private string _originalSettingsJson = "";

    public SettingsWindow()
    {
        // Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh"); 
        InitializeComponent();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            btPluginNameTextbox.Text = ScopeSettings.WindowsBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) btPluginNameTextbox.Text = ScopeSettings.LinuxBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) btPluginNameTextbox.Text = ScopeSettings.OsXBlePluginName;
        DataContext = this;
        LanguageChooseComboBox.SelectedIndex = Thread.CurrentThread.CurrentUICulture.Name.ToLower() switch
        {
            "zh" => 0,
            "en-us" => 1,
            _ => 0
        };
        _originalSettingsJson = JsonConvert.SerializeObject(ScopeSettings);
    }

    private async void SaveConfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ScopeSettings.WindowsBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ScopeSettings.LinuxBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) ScopeSettings.OsXBlePluginName = btPluginNameTextbox.Text;
        ScopeSettings.Save();
        // 切换语言
        Thread.CurrentThread.CurrentUICulture = LanguageChooseComboBox.SelectedIndex switch
        {
            0 => new CultureInfo("zh"),
            1 => new CultureInfo("en-us"),
            _ => new CultureInfo("zh")
        };
        if (JsonConvert.SerializeObject(ScopeSettings) != _originalSettingsJson)
        {
            await MessageBoxManager
                .GetMessageBoxStandard(Language.GetString("warning"), Language.GetString("available_after_restart"))
                .ShowWindowDialogAsync(this);
        }
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
        ScopeSettings.DataDir = files[0].Path.LocalPath;
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
                Process.Start("explorer.exe", $"\"{ScopeSettings.BackupPath}\"");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", $"\"{ScopeSettings.BackupPath}\"");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"\"{ScopeSettings.BackupPath}\"");
        }
        catch (Exception ee)
        {
            MessageBoxManager
                .GetMessageBoxStandard(Language.GetString("warning"), Language.GetString("failed") + ee.Message)
                .ShowWindowDialogAsync(this);
            DebugWindow.GetInstance().UpdateDebugContent(ee.Message);
        }
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ScopeSettings.ResetSettings();
        ScopeSettings = Settings.Load();
        Close();
        var newWindow = new SettingsWindow();
        newWindow._originalSettingsJson = "AFTER_RESET";
        newWindow.Show();
    }
}