using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SQ5R.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
    public static Settings Default { get; } = (Settings)Synchronized(new Settings());

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("中文")]
    public string language
    {
        get => (string)this["language"];
        set => this["language"] = value;
    }
}