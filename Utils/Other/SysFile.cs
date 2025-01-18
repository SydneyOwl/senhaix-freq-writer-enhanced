using System;
using System.IO;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.DataModels.Interfaces;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Other;

public class SysFile
{
    private static readonly Settings _settings = Settings.Load();


    private static bool DirCheck(string path)
    {
        if (Directory.Exists(path))
        {
            DebugWindow.GetInstance().UpdateDebugContent($"{path}已存在");
            return true;
        }

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"新建失败：{path}，${e.Message}");
            return false;
        }

        return true;
    }

    public static bool CheckDefaultDirectory()
    {
        return DirCheck(_settings.DataDir);
    }

    public static bool CheckBackupDirectory()
    {
        return DirCheck(_settings.BackupPath);
    }

    public static void CreateBackup<T>(T data) where T : IBackupable
    {
        if (!_settings.EnableAutoBackup) return;
        try
        {
            // 如果总备份数大于，则删除最后一个
            var dirInfo = new DirectoryInfo(_settings.BackupPath);
            var datInfo = dirInfo.GetFiles("*.dat");
            Array.Sort(datInfo, (x, y) =>
                x.LastWriteTime.CompareTo(y.LastWriteTime));
            for (var i = 0; i < datInfo.Length - _settings.MaxBackupNumber; i++) datInfo[i].Delete();
            var identifier = GetIdentifier<T>();
            var filePath = Path.Join(_settings.BackupPath,
                $"Autobackup-{identifier}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.dat");

            using (Stream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                stream.Seek(0L, SeekOrigin.Begin);
                stream.SetLength(0L);
                data.SaveToFile(stream);
            }
        }
        catch (Exception aa)
        {
            DebugWindow.GetInstance().UpdateDebugContent(aa.Message);
        }
    }

    private static string GetIdentifier<T>()
    {
        if (typeof(T) == typeof(AppData))
            return "GT12";
        if (typeof(T) == typeof(DataModels.Shx8800Pro.AppData))
            return "8800Pro";
        if (typeof(T) == typeof(ClassTheRadioData))
            return "8800-8600";
        return "UNKNOWN";
    }
}