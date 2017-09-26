using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.Steam;
using VRUB.UI;
using VRUB.Utility;

namespace VRUB
{
    static class Program
    {
        static AddonManager _addonManager;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.SetupLogfile("output.log");
            Logger.Info("[DEBUG] Command Line Args: " + string.Join(" ", Environment.GetCommandLineArgs()));
            Application.ApplicationExit += Application_ApplicationExit;

            SteamManager.Init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _addonManager = new AddonManager();
            _addonManager.StartAsync();

            VRUBApplicationContext context = new VRUBApplicationContext();
            Application.Run(context);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            SteamManager.Shutdown();
        }

        public static void Quit()
        {
            Logger.CloseHandle();
            SteamManager.Shutdown();
            _addonManager.Stop();

            Application.Exit();
        }
    }
}
