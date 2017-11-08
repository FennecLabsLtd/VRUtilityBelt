using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.API;

namespace Config
{
    public class Plugin : IPlugin
    {
        Addon _owner;
        Dictionary<Addon, ConfigAPI> _containers;

        public override void OnLoad(AddonManager manager, Addon owner)
        {
            _containers = new Dictionary<Addon, ConfigAPI>();
            _owner = owner;
        }

        public override void OnRegister(Addon parentAddon, Overlay overlay)
        {
            if (!_containers.ContainsKey(parentAddon))
            {
                _containers.Add(parentAddon, new ConfigAPI(parentAddon));

                parentAddon.Disabled += (o, e) =>
                {
                    _containers.Remove(parentAddon);
                };
            }

            overlay.Bridge.RegisterLink("VRUB_Core_Config", _containers[parentAddon]);
        }

        public override void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            overlay.InjectJsFile("plugin://" + _owner.Key + "_Config/config.js");
        }
    }
}
