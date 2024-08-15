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
    [ObservableProperty] private int backupInterval = 180;
    [ObservableProperty] private int maxBackupNumber = 200;
    [ObservableProperty] private string dataDir= AppContext.BaseDirectory;
    [ObservableProperty] private string windowsBlePluginName = "BLEPlugin_windows_x64.exe";
    [ObservableProperty] private string osXBlePluginName  = "BLEPlugin_macos_x64";
    [ObservableProperty] private string linuxBlePluginName  = "BLEPlugin_linux_x64";
    [ObservableProperty] private string rpcClientProcessArgs= "--verbose --no-color --inside-call";
    
    // public string BackupRootPath = Path.Join(Load().DataDir, "backup");
    
    private static readonly string FilePath = Path.Join(AppContext.BaseDirectory, "settings.json");

    public SETTINGS() { }

    public static SETTINGS Load()
    {
        if (!File.Exists(FilePath))
            return new SETTINGS(); // Return default settings if file does not exist

        var json = File.ReadAllText(FilePath);
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
        File.WriteAllText(FilePath, json);
    }
}