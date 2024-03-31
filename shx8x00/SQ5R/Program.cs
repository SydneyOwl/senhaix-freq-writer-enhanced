using System;
using System.Windows.Forms;
using BF_H802_Import_Picture_tools;
using SQ5R.View;

namespace SQ5R;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Application.Run(new FormIPT("COM3"));
        Application.Run(new FormMain());
    }
}