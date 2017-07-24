using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUtilityBelt.Addons;
using VRUtilityBelt.Addons.Overlays;
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
            AddonManager manager = new AddonManager();
            manager.StartAsync();
        }
    }
}
