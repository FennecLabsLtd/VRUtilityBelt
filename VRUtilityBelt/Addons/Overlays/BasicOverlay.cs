using Newtonsoft.Json;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CefSharp.OffScreen;

namespace VRUtilityBelt.Addons.Overlays
{
    class BasicOverlay : IOverlay
    {
        WebKitOverlay _wkOverlay;

        Addon _addon;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("entrypoint")]
        public string EntryPoint { get; set; } = "index.html";

        [JsonProperty("type")]
        public OverlayType Type { get; set; } = OverlayType.Dashboard;

        [JsonProperty("width")]
        public int Width { get; set; } = 1024;

        [JsonProperty("height")]
        public int Height { get; set; } = 768;

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }

        [JsonProperty("meters")]
        public float MeterWidth { get; set; } = 2f;

        [JsonProperty("debug")]
        public bool DebugMode { get; set; } = false;

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public BasicOverlay(Addon addon)
        {
            _addon = addon;
        }

        public void Setup(string path, string keyPrefix = "builtin")
        {
            ParseManifest(path);
            _wkOverlay = new WebKitOverlay(new Uri(EntryPoint), Width, Height, "vrub." + _addon.Key + "." + Key, Name, Type);
            _wkOverlay.BrowserReady += _wkOverlay_BrowserReady;
            _wkOverlay.BrowserPreInit += _wkOverlay_BrowserPreInit;

            if(Type == OverlayType.Dashboard || Type == OverlayType.Both)
                _wkOverlay.DashboardOverlay.Width = MeterWidth;
            else
                _wkOverlay.InGameOverlay.Width = MeterWidth;

            _wkOverlay.StartBrowser();
        }

        private void _wkOverlay_BrowserPreInit(object sender, EventArgs e)
        {
            
        }

        void ParseManifest(string path)
        {
            if(!File.Exists(path + "\\manifest.json"))
            {
                throw new FileNotFoundException("Could not locate overlay manifest.json in " + path);
            }

            JsonConvert.PopulateObject(File.ReadAllText(path + "\\manifest.json"), this);

            Uri result;

            if (!Uri.TryCreate(EntryPoint, UriKind.Absolute, out result))
            {
                Console.WriteLine("EntryPoint for " + Key + " is not a valid URL, trying file path instead.");

                if(!Path.IsPathRooted(EntryPoint))
                {
                    EntryPoint = "file://" + Path.GetFullPath(path + "\\" + EntryPoint);
                }
            }

            Console.WriteLine("EntryPoint for " + Key + ":" + EntryPoint);
        }

        private void _wkOverlay_BrowserReady(object sender, EventArgs e)
        {
            if(DebugMode)
            {
                ((ChromiumWebBrowser)sender).GetBrowser().GetHost().ShowDevTools();
            }
        }

        public bool Validate()
        {
            if(Name == null || Key == null || EntryPoint == null)
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

        public bool HasFlag(string flag)
        {
            return Flags != null && Flags.Contains(flag);
        }

        public void Draw()
        {

        }

        public void Update()
        {

        }
    }
}
