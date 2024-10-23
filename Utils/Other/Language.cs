using System.Resources;

namespace SenhaixFreqWriter.Utils.Other;

public class Language
{
    public static ResourceManager manager = new(Properties.Resources.ResourceManager.BaseName,
        typeof(Properties.Resources).Assembly);

    public static string GetString(string key)
    {
        return manager.GetString(key) ?? "";
    }
}