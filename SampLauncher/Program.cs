using System;
using System.Windows.Forms;
using SampLauncher.Forms;
using SAMPLauncher.Forms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
