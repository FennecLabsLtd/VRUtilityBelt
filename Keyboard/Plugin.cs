using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.API;
using CefSharp.OffScreen;

namespace Keyboard
{
    public class Plugin : IPlugin
    {
        Addon _owner;

        public override void OnLoad(AddonManager manager, Addon owner)
        {
            _owner = owner;
        }

        public override void OnRegister(Addon parentAddon, Overlay overlay)
        {
            overlay.Bridge.RegisterLink("VRUB_Core_Keyboard", new KeyboardAPI(overlay));
        }

        public override void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            overlay.InjectJsFile("plugin://" + _owner.Key + "_Keyboard/keyboard.js");
        }
    }
}
