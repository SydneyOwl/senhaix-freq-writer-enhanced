using System;
using System.Windows.Forms;
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