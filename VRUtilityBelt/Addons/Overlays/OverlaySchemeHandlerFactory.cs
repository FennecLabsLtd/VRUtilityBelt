using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUtilityBelt.Utility;

namespace VRUtilityBelt.Addons.Overlays
{
    class OverlaySchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "addon";
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            if(schemeName == "addon")
            {
                SteamVR_WebKit.SteamVR_WebKit.Log("Got request with addon:// scheme: " + request.Url);

                if(BasicOverlay.BrowserAddonMap.ContainsKey(browser))
                {
                    
                    Addon addon = BasicOverlay.BrowserAddonMap[browser];
                    //Path.GetFullPath(addon.BasePath + "\\" + request.Url);
                }
            }

            return null;
        }
    }
}
