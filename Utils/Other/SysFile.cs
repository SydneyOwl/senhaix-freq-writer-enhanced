using System;
using System.IO;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Other;

public static class SysFile
{
    public static bool CheckDefaultDirectory()
    {
        if (Directory.Exists(SETTINGS.DATA_DIR))
        {
            DebugWindow.GetInstance().updateDebugContent($"{SETTINGS.DATA_DIR}已存在");
            return true;
        }
        try
        {
            DirectoryInfo di = Directory.CreateDirectory(SETTINGS.DATA_DIR);
        }
        catch (Exception e)
        {
            DebugWindow.GetInstance().updateDebugContent($"新建失败：{SETTINGS.DATA_DIR}，${e.Message}");
            return false;
        }
        return true;
    }
}