using System;
using System.Runtime.InteropServices;

namespace SenhaixFreqWriter.Properties;

public static class SETTINGS
{
    // Deprecated!
    public const string RPC_URL = "http://127.0.0.1:8563/";

    public const string WS_RPC_URL = "ws://127.0.0.1:8563/rpc";
    public static bool DISABLE_DEBUG_CHAN_DATA_OUTPUT = true;

    public static string DATA_DIR = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? $"/Users/{Environment.UserName}/Library/Containers/com.sydneyowl/Data"
        : ".";

    public static string WINDOWS_BLE_PLUGIN_NAME = "BLEPlugin_windows_x64.exe";

    public static string OSX_BLE_PLUGIN_NAME = "BLEPlugin_macos_x64";

    public static string LINUX_BLE_PLUGIN_NAME = "BLEPlugin_linux_x64";

    public static string RPC_CLIENT_PROCESS_ARGS = "--verbose --no-color --inside-call";
}