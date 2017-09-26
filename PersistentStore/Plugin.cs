﻿using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.Utility;

namespace PersistentStore
{
    public class Plugin : VRUB.API.IPlugin
    {
        Dictionary<Addon, PersistenceContainer> _containers;
        public override void OnLoad(AddonManager manager)
        {
            _containers = new Dictionary<Addon, PersistenceContainer>();
        }

        public override void OnRegister(Addon parentAddon, Overlay overlay)
        {
            base.OnRegister(parentAddon, overlay);
        }

        public override void OnBrowserPreInit(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            if (!_containers.ContainsKey(parentAddon))
                _containers.Add(parentAddon, new PersistenceContainer(parentAddon.DerivedKey));

            browser.RegisterJsObject("VRUB_Plugins_PersistentStore", _containers[parentAddon], new CefSharp.BindingOptions() { CamelCaseJavascriptNames = false, Binder = overlay.Binder });
        }

        public override void OnBrowserReady(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {

        }

        public override void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            overlay.InjectJsFile("addon://plugins/PersistentStore/persistent_storage.js");
        }
    }
}
