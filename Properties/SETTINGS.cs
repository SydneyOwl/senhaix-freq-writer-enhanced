using System;
using System.Runtime.InteropServices;

namespace SenhaixFreqWriter.Properties;

public static class SETTINGS
{
    public static bool DEBUG_ENABLED;
    
    public static string DATA_DIR = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)? $"/Users/{Environment.UserName}/Library/Containers/com.sydneyowl/Data":"./";

    public const string RPC_URL = "http://127.0.0.1:8563";
}