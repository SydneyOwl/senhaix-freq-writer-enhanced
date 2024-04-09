using System;
using System.Windows.Forms;
using SHX_GT12_CPS.View;

namespace SHX_GT12_CPS;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
    }
}