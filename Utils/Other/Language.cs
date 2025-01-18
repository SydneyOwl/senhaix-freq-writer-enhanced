using System.Resources;
using SenhaixFreqWriter.Properties;

namespace SenhaixFreqWriter.Utils.Other;

public class Language
{
    public static ResourceManager manager = new(Resources.ResourceManager.BaseName,
        typeof(Resources).Assembly);

    public static string GetString(string key)
    {
        return manager.GetString(key) ?? "";
    }
}