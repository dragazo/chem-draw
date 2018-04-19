using System;
using System.Windows.Forms;
//using System.Runtime.Hosting;

namespace Chemipad
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Editor e = new Editor();
            string[] args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments?.ActivationData;
            if (args != null && args.Length > 0)
                e.Molecule.Open(args[0]);

            Application.Run(e);
        }
    }
}
