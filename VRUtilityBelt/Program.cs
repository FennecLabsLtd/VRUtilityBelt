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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += Application_ApplicationExit;

            SteamManager.Init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UI.Workshop.WorkshopUploader uploader = new UI.Workshop.WorkshopUploader();
            uploader.Show();

            AddonManager manager = new AddonManager();
            manager.StartAsync();

            VRUBApplicationContext context = new VRUBApplicationContext();
            Application.Run(context);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            SteamManager.Shutdown();
        }
    }
}
