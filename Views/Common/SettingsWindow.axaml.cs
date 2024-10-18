using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter.Views.Common;

public partial class SettingsWindow : Window
{
    // 有空再改
    public Settings Settings { get; set; } = Settings.Load();

    public SettingsWindow()
    {
        InitializeComponent();
        // backupCheckbox.IsChecked = SETTINGS.ENABLE_AUTO_BACKUP;
        // backupIntervalTextbox.Text = SETTINGS.BACKUP_INTERVAL.ToString();
        // comAutoChooseCheckbox.IsChecked = SETTINGS.ENABLE_SELECT_PORT_IN_ADVANCE;
        // detailedOutputCheckBox.IsChecked = SETTINGS.ENABLE_DEBUG_CHAN_DATA_OUTPUT;
        // rpcURLTextbox.Text = SETTINGS.WS_RPC_URL;
        // dataPathTextbox.Text = SETTINGS.DATA_DIR;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            btPluginNameTextbox.Text = Settings.WindowsBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) btPluginNameTextbox.Text = Settings.LinuxBlePluginName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) btPluginNameTextbox.Text = Settings.OsXBlePluginName;
        // btPluginArgsTextbox.Text = SETTINGS.RPC_CLIENT_PROCESS_ARGS;
        DataContext = this;
    }

    private void SaveConfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // SETTINGS.ENABLE_AUTO_BACKUP =  backupCheckbox.IsChecked.Value;
        // int interval;
        // if (!int.TryParse(backupIntervalTextbox.Text, out interval))
        // {
        //     MessageBoxManager.GetMessageBoxStandard("注意", "保存间隔只能为数字！");
        //     return;
        // }
        // SETTINGS.BACKUP_INTERVAL = interval;
        // SETTINGS.ENABLE_SELECT_PORT_IN_ADVANCE = comAutoChooseCheckbox.IsChecked.Value;
        // SETTINGS.ENABLE_DEBUG_CHAN_DATA_OUTPUT = detailedOutputCheckBox.IsChecked.Value;
        // SETTINGS.WS_RPC_URL = rpcURLTextbox.Text;
        // SETTINGS.DATA_DIR = dataPathTextbox.Text;
        // SETTINGS.RPC_CLIENT_PROCESS_ARGS = btPluginArgsTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Settings.WindowsBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) Settings.LinuxBlePluginName = btPluginNameTextbox.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Settings.OsXBlePluginName = btPluginNameTextbox.Text;
        Settings.Save();
        Close();
    }

    private async void ChoosePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
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
                Process.Start("nautilus", $"\"{Settings.GetBackupPath()}\"");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"\"{Settings.GetBackupPath()}\"");
        }
        catch (Exception ee)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "打开失败！" + ee.Message).ShowWindowDialogAsync(this);
            DebugWindow.GetInstance().UpdateDebugContent(ee.Message);
        }
    }
}