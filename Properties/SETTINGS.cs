using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace SenhaixFreqWriter.Properties;

public static class SETTINGS
{
    public static bool DEBUG_ENABLED;

    public static string DATA_DIR = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? $"/Users/{Environment.UserName}/Library/Containers/com.sydneyowl/Data"
        : ".";

    public const string RPC_URL = "http://127.0.0.1:8563/";

    public static string WINDOWS_BLE_PLUGIN_NAME = "BLEPlugin_windows_x64.exe";
    public static string OSX_BLE_PLUGIN_NAME = "BLEPlugin_macos_x64";
    public static string LINUX_BLE_PLUGIN_NAME = "BLEPlugin_linux_x64";

    //RPC服务端的启动参数，可用--help查看；输出仅在开启调试功能时能看到！
    public static string RPC_SERVER_PROCESS_ARGS = "--verbose --enable-keepalive";
}