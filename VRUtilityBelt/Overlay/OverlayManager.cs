using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamVR_WebKit;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace VRUtilityBelt.Overlay
{
    class OverlayManager
    {
        Dictionary<string, IOverlay> _overlays = new Dictionary<string, IOverlay>();

        string[] paths =
        {
            Environment.CurrentDirectory + "\\overlays\\builtin\\",
            Environment.CurrentDirectory + "\\overlays\\indev\\",
        };

        public void Run()
        {
            SteamVR_WebKit.SteamVR_WebKit.FPS = 30;
            Refresh();

            SteamVR_WebKit.SteamVR_WebKit.RunOverlays();
        }

        public void Refresh() {
            if(_overlays != null)
            {
                foreach(IOverlay ov in _overlays.Values)
                {
                    ov.Destroy();
                }
            }

            _overlays = new Dictionary<string, IOverlay>();

            PopulateOverlays();
        }

        public void PopulateOverlays()
        {
            // TODO: Get workshop paths from SteamVR

            foreach(string path in paths)
            {
                foreach(string folder in Directory.EnumerateDirectories(path))
                {
                    HandleOverlay(folder);
                }
            }
        }

        void HandleOverlay(string path)
        {
            // TODO: Handle loading an existing overlay (refreshing rather than adding)

            if(!File.Exists(path + "\\config.json"))
            {
                Console.WriteLine("WARNING: Failed to locate config.json for " + path);
                return;
            }

            BasicOverlay overlay;

            try
            {
                overlay = JsonConvert.DeserializeObject<BasicOverlay>(File.ReadAllText(path + "\\config.json"));
            } catch(JsonException e)
            {
                Console.WriteLine("ERROR: Failed to parse" + path + "\\config.json: " + e.Message);
                return;
            }

            if(!overlay.Validate())
            {
                Console.WriteLine("ERROR: " + path + "\\config.json does not contain the required fields");
                return;
            }

            overlay.Setup();
            _overlays.Add(overlay.Identifier, overlay);
        }
    }
}
