using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.Overlay
{
    class BasicOverlay : IOverlay
    {
        WebKitOverlay _wkOverlay;
        public string Name { get; set; }
        public string Identifier { get; set; }

        public string StartPath { get; set; }

        public OverlayType Type { get; set; }

        public bool DebugMode { get; set; }

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public void Setup()
        {
            _wkOverlay = new WebKitOverlay(new Uri(StartPath), 1024, 768, "vrub.overlays." + Identifier, Name, Type);
            _wkOverlay.BrowserReady += _wkOverlay_BrowserReady;

        }

        private void _wkOverlay_BrowserReady(object sender, EventArgs e)
        {
            if(DebugMode)
            {
                ((WebKitOverlay)sender).Browser.GetBrowser().GetHost().ShowDevTools();
            }
        }

        public bool Validate()
        {
            if(Name == null || Identifier == null || StartPath == null)
            {
                return false;
            } else
            {
                return true;
            }
        }

        public void Destroy()
        {
            if(Type == OverlayType.Both || Type == OverlayType.Dashboard)
                _wkOverlay.DestroyDashboardOverlay();

            if (Type == OverlayType.Both || Type == OverlayType.InGame)
                _wkOverlay.DestroyInGameOverlay();
        }
    }
}
