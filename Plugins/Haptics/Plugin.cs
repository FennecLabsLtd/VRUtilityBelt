using CefSharp.OffScreen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.API;
using VRUB.Bridge;

namespace Haptics
{
    public class Plugin : IPlugin
    {
        Addon _owner;

        static List<HapticPulse> _pulses;

        public override void OnLoad(AddonManager manager, Addon owner)
        {
            _owner = owner;
            _pulses = new List<HapticPulse>();

            BridgeHandler.RegisterGlobalLink("VRUB_Core_Haptics", HapticAPI.Instance);
        }

        public override void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            overlay.InjectJsFile("plugin://" + _owner.Key + "_Haptics/haptics.js");
        }

        public override void GlobalUpdate()
        {
            foreach(HapticPulse pulse in _pulses.ToList())
            {
                if(!pulse.CheckAndPulse())
                {
                    _pulses.Remove(pulse);
                }
            }
        }

        public static void AddPulse(HapticPulse pulse)
        {
            _pulses.Add(pulse);
        }

    }
}
