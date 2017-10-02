using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.Utility;

namespace SuperSimplePlugin
{
    public class Plugin : VRUB.API.IPlugin
    {
        public override void OnLoad(AddonManager manager, Addon owner)
        {
            Logger.Info("SUPER SIMPLE OnLoad Fired");
        }

        public override void OnRegister(Addon parentAddon, Overlay overlay)
        {
            Logger.Info("SUPER SIMPLE OnRegister Fired for " + parentAddon.Name + " and " + overlay.Name);
        }

        public override void OnBrowserPreInit(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            Logger.Info("SUPER SIMPLE OnBrowserPreInit Fired for " + parentAddon.Name);
        }

        public override void OnBrowserReady(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            Logger.Info("SUPER SIMPLE OnBrowserReady Fired for " + parentAddon.Name);
        }

        public override void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            Logger.Info("SUPER SIMPLE OnBrowserNavigation Fired for " + parentAddon.Name);
        }
    }
}
