using System.Collections.Generic;

namespace SenhaixFreqWriter.Constants.Common;

public class Thankslist
{
    public static List<string> ThankUList = new()
    {
        "BH7EWP - 跨平台开发阶段的大力支持，验证了8800写频部分在mac端的可行性（包括蓝牙及写频线），以及许多富有建设性的建议！",
        "BA4QDP - bug反馈（未修复）：蓝牙写频可能在macOS13及以下存在bug，并对测试给予了大力支持！",
        "BH4HLI - bug反馈（已修复）：无法进行8600pro的开机画面写入，并验证了修正后的软件可以正常写入开机画面！",
        "BH6ROG - 验证了8600pro写频软件在mac端可用，并帮助测试了8800pro写频部分在mac端的可用性，同时提出了许多建设性的建议！",
        "NiZiv - 提出了一些文档修正的意见！"
    };

    public static string ToThankUString()
    {
        var thanks = "";
        for (var i = 0; i < ThankUList.Count; i++) thanks += ThankUList[i] + "\n";

        return thanks;
    }
}