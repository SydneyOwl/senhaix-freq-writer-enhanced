using System;
using System.Windows.Forms;

namespace SQ5R;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Application.Run(new FormConnBluetooth());
        Application.Run(new FormMain());
    }
}