using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SQ5R.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
    private static ResourceManager resourceMan;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan == null)
            {
                var resourceManager =
                    new ResourceManager("SQ5R.Properties.Resources", typeof(Resources).Assembly);
                resourceMan = resourceManager;
            }

            return resourceMan;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture { get; set; }

    internal static Bitmap _new
    {
        get
        {
            var @object = ResourceManager.GetObject("new", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap myTextBox
    {
        get
        {
            var @object = ResourceManager.GetObject("myTextBox", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap New_Eng
    {
        get
        {
            var @object = ResourceManager.GetObject("New_Eng", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Open
    {
        get
        {
            var @object = ResourceManager.GetObject("Open", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Open_Eng
    {
        get
        {
            var @object = ResourceManager.GetObject("Open_Eng", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Read
    {
        get
        {
            var @object = ResourceManager.GetObject("Read", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Read_Eng
    {
        get
        {
            var @object = ResourceManager.GetObject("Read_Eng", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Save
    {
        get
        {
            var @object = ResourceManager.GetObject("Save", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Save_Eng
    {
        get
        {
            var @object = ResourceManager.GetObject("Save_Eng", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Write
    {
        get
        {
            var @object = ResourceManager.GetObject("Write", Culture);
            return (Bitmap)@object;
        }
    }

    internal static Bitmap Write_Eng
    {
        get
        {
            var @object = ResourceManager.GetObject("Write_Eng", Culture);
            return (Bitmap)@object;
        }
    }
}