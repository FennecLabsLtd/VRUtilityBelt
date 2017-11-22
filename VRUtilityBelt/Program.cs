using SharpRaven;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        const string SentryKey = "STUB_KEY";

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

            if (!Environment.GetCommandLineArgs().Contains("-debug") && SentryKey.Length > 10)
            {
                SharpRaven.RavenClient ravenClient = new RavenClient("https://" + SentryKey + "@sentry.io/227668");
                ravenClient.Release = Application.ProductVersion.ToString();
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    try
                    {
                        ravenClient.Capture(new SharpRaven.Data.SentryEvent((Exception)e.ExceptionObject));
                    }
                    finally
                    {
                        Application.Exit();
                    }
                };

                Logger.Info("Sentry Reporting is Enabled");
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            SteamManager.Init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ConfigUtility.Load();

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
