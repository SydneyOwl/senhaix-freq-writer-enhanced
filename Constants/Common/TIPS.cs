using System;
using System.Collections.Generic;

namespace SenhaixFreqWriter.Constants.Common;

public class TIPS
{
    public static List<String> TipList = new()
    {
        "Tips: 对信道进行拖动或清空等操作后序号可能会出现“27”，此时上下拉动右侧滚动条即可！",
        "Tips: 如遇到任何问题，欢迎在github中提出issue或PR!",
        $"Tips: 软件版本：{(Properties.Version.VersionTag == "@TAG_NAME@" ? "（内部版本）":Properties.Version.VersionTag)}，可在github上检查、获取更新！",
        "Tips: 体谅、忠诚、进步、友爱、适度、爱国",
        "Tips: 目前仅8800支持蓝牙写频！"
    };
}