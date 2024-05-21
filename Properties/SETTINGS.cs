using System;
using System.Runtime.InteropServices;

namespace SenhaixFreqWriter.Properties;

public static class SETTINGS
{
    public static bool debugEnabled;
    
    public static string DATA_DIR = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)? $"/Users/{Environment.UserName}/Library/Containers/com.sydneyowl/Data":"./";
}