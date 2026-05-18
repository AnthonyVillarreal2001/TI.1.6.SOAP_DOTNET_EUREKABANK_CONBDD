using System;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }
    }
}