using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SenhaixFreqWriter.Properties;

// 通过命令行传入的参数
public static class CMD_SETTINGS
{
    public static bool BypassRootCheck = false;
    
    // 默认设置，不允许更改
    public static string CrashLogWindowsPath = Path.Join(AppContext.BaseDirectory,"sfw-crashlog");
    public static string CrashLogMacOSPath = "/tmp/sfw-crashlog";
    public static string CrashLogLinuxPath = "/tmp/sfw-crashlog";

    public static string CrashLogPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CrashLogWindowsPath;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CrashLogLinuxPath;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return CrashLogMacOSPath;
            }

            return CrashLogLinuxPath;
        }
    }
}