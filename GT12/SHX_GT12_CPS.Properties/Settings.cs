using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SHX_GT12_CPS.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
    public static Settings Default { get; } = (Settings)Synchronized(new Settings());

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("Chinese")]
    public string LANG
    {
        get => (string)this["LANG"];
        set => this["LANG"] = value;
    }
}