using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Overlays;
using VRUB.Utility;

namespace VRUB.Addons.Overlays
{
    class OverlayRequestContextHandler : IRequestContextHandler
    {
        public Overlay Overlay;

        public OverlayRequestContextHandler(Overlay overlay)
        {
            Overlay = overlay;
        }

        public ICookieManager GetCookieManager()
        {
            return new CookieManager(Overlay.GetCloudStoragePath() + "\\Cookies", Overlay.PersistSessionCookies, null);
        }

        public bool OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
        {
            Logger.Trace("[CEF] OnBeforePluginLoad: " + pluginInfo.Name);
            return false; // Use recommended policy.
        }
    }
}
