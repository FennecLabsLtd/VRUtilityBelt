using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUtilityBelt.Addons;
using VRUtilityBelt.Addons.Overlays;
using VRUtilityBelt.Steam;
using VRUtilityBelt.UI;

namespace VRUtilityBelt
{
    static class Program
    {
        static Thread addonManagerThread;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += Application_ApplicationExit;

            SteamManager.Init();

            AddonManager manager = new AddonManager();
            manager.StartAsync();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            SteamManager.Shutdown();
        }
    }
}
