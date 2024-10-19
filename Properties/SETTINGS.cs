using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.Properties;

public partial class Settings : ObservableObject
{
    private static readonly object Lock = new();

    [ObservableProperty] private string _version = Properties.Version.VersionTag;
    [ObservableProperty] private string _rpcUrl = "http://127.0.0.1:8563/";
    [ObservableProperty] private string _wsRpcUrl = "ws://127.0.0.1:8563/rpc";
    [ObservableProperty] private bool _enableSelectPortInAdvance = true;
    [ObservableProperty] private bool _enableDebugChanDataOutput = false;
    [ObservableProperty] private bool _enableAutoBackup = true;
    [ObservableProperty] private int _languageIndex = 0;
    [ObservableProperty] private int _backupInterval = 200;
    [ObservableProperty] private int _maxBackupNumber = 150;
    [ObservableProperty] private string _dataDir = AppContext.BaseDirectory;
    [ObservableProperty] private string _windowsBlePluginName = "BLEPlugin_windows_x64.exe";
    [ObservableProperty] private string _osXBlePluginName = "BLEPlugin_macos_x64";
    [ObservableProperty] private string _linuxBlePluginName = "BLEPlugin_linux_x64";
    [ObservableProperty] private string _rpcClientProcessArgs = "--verbose --no-color --inside-call";

    // public string BackupRootPath = Path.Join(Load().DataDir, "backup");

    // 设置的json不能变更位置
    private static readonly string SettingJsonFilePath = Path.Join(AppContext.BaseDirectory, "settings.json");

    public Settings()
    {
    }

    public string GetBackupPath()
    {
        return Path.Join(DataDir, "backup");
    }

    public static Settings Load()
    {
        if (!File.Exists(SettingJsonFilePath))
            return new Settings(); // Return default settings if file does not exist

        var json = File.ReadAllText(SettingJsonFilePath);
        var res = JsonSerializer.Deserialize<Settings>(json);
        if (res == null) return new Settings();
        // 版本号不符合，重置设置
        // if (res.Version != Properties.Version.VersionTag)
        // {
        //     try
        //     {
        //         File.Delete(FilePath);
        //     }
        //     catch
        //     {
        //         //ignored
        //     }
        //     
        //     return new SETTINGS();
        // }

        return res;
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingJsonFilePath, json);
    }
}