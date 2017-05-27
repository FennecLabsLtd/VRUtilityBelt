using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUtilityBelt.Overlay;
using VRUtilityBelt.UI;

namespace VRUtilityBelt
{
    static class Program
    {
        static Thread overlayManagerThread;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OverlayManager manager = new OverlayManager();

            overlayManagerThread = new Thread(manager.Run);
            overlayManagerThread.Start();

            Application.Run(new VRUBApplicationContext());
        }
    }
}
