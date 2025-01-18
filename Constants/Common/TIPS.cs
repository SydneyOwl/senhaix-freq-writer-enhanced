using System.Collections.Generic;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter.Constants.Common;

public class Tips
{
    private static readonly string _backupRootPath = Settings.Load().BackupPath;

    public static List<string> TipList = new()
    {
        "Tips: 对信道进行拖动或清空等操作后序号可能会出现序号异常或字段被清空等问题，此时上下拉动右侧滚动条刷新界面即可！",
        "Tips: 如遇到任何问题，欢迎在github中提出issue或PR!",
        $"Tips: 软件版本：{(Version.VersionTag == "@TAG_NAME@" ? "（内部版本）" : Version.VersionTag)}，可在github上检查、获取更新！",
        "Tips: 体谅、忠诚、进步、友爱、适度、爱国",
        "Tips: 目前仅8800支持蓝牙写频！",
        $"可以在{_backupRootPath}查看到自动备份记录！"
    };
}