using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.Properties;
public partial class SETTINGS :ObservableObject
{
    private static readonly object _lock = new object();

    [ObservableProperty] private string version = Properties.Version.VersionTag;
    [ObservableProperty] private string rpcUrl  = "http://127.0.0.1:8563/";
    [ObservableProperty] private string wsRpcUrl  = "ws://127.0.0.1:8563/rpc";
    [ObservableProperty] private bool enableSelectPortInAdvance = true;
    [ObservableProperty] private bool enableDebugChanDataOutput = false;
    [ObservableProperty] private bool enableAutoBackup = true;
    [ObservableProperty] private int backupInterval = 200;
    [ObservableProperty] private int maxBackupNumber = 150;
    [ObservableProperty] private string dataDir= AppContext.BaseDirectory;
    [ObservableProperty] private string windowsBlePluginName = "BLEPlugin_windows_x64.exe";
    [ObservableProperty] private string osXBlePluginName  = "BLEPlugin_macos_x64";
    [ObservableProperty] private string linuxBlePluginName  = "BLEPlugin_linux_x64";
    [ObservableProperty] private string rpcClientProcessArgs= "--verbose --no-color --inside-call";
    
    // public string BackupRootPath = Path.Join(Load().DataDir, "backup");
    
    // 设置的json不能变更位置
    private static readonly string SettingJsonFilePath = Path.Join(AppContext.BaseDirectory, "settings.json");

    public SETTINGS() { }

    public string GetBackupPath()
    {
        return Path.Join(DataDir, "backup");
    }

    public static SETTINGS Load()
    {
        if (!File.Exists(SettingJsonFilePath))
            return new SETTINGS(); // Return default settings if file does not exist

        var json = File.ReadAllText(SettingJsonFilePath);
        var res = JsonSerializer.Deserialize<SETTINGS>(json);
        if (res == null)
        {
            return new SETTINGS();
        }
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