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
using VRUtilityBelt.Utility;

namespace VRUtilityBelt
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
            LogHelper.SetupLogfile("output.log");
            Console.WriteLine("[DEBUG] Command Line Args: " + string.Join(" ", Environment.GetCommandLineArgs()));
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
            LogHelper.CloseHandle();
            SteamManager.Shutdown();
            _addonManager.Stop();

            Application.Exit();
        }
    }
}
