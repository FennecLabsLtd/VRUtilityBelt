using Newtonsoft.Json;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CefSharp.OffScreen;
using VRUtilityBelt.JsInterop;
using VRUtilityBelt.Addons.Data;
using CefSharp;

namespace VRUtilityBelt.Addons.Overlays
{
    class BasicOverlay : IOverlay
    {
        WebKitOverlay _wkOverlay;

        public static Dictionary<IBrowser, Addon> BrowserAddonMap = new Dictionary<IBrowser, Addon>();

        Addon _addon;

        string BasePath;

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

        [JsonProperty("inject")]
        public InjectableFiles Inject { get; set; }

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public WebKitOverlay Overlay { get { return _wkOverlay; } }

        public BasicOverlay(Addon addon)
        {
            _addon = addon;
        }

        public void Setup(string path, string keyPrefix = "builtin")
        {
            BasePath = path;
            ParseManifest(path);
            _wkOverlay = new WebKitOverlay(new Uri(EntryPoint), Width, Height, "vrub." + _addon.DerivedKey + "." + Key, Name, Type);
            _wkOverlay.BrowserReady += _wkOverlay_BrowserReady;
            _wkOverlay.BrowserPreInit += _wkOverlay_BrowserPreInit;

            _wkOverlay.EnableKeyboard = HasFlag("vr_keyboard");

            if(Type == OverlayType.Dashboard || Type == OverlayType.Both)
                _wkOverlay.DashboardOverlay.Width = MeterWidth;
            else
                _wkOverlay.InGameOverlay.Width = MeterWidth;

            _wkOverlay.StartBrowser();
        }

        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading && e.Browser.HasDocument)
            {
                InsertInjectableFiles();
            }
        }

        void InsertInjectableFiles()
        {
            if(Inject != null)
            {
                if(Inject.CSS != null)
                {
                    foreach (string CSSFile in Inject.CSS) {
                        _wkOverlay.TryExecAsyncJS(@"
                            var insertCss = document.createElement('link');
                            insertCss.rel = 'stylesheet';
                            insertCss.href = '" + TranslatePath(CSSFile, BasePath).Replace("\\", "/")  + @"';
                            document.head.appendChild(insertCss);
                        ");
                    }
                }

                if (Inject.JS != null)
                {
                    foreach (string JSFile in Inject.JS)
                    {
                        _wkOverlay.TryExecAsyncJS(@"
                            var insertJs = document.createElement('script');
                            insertJs.src = '" + TranslatePath(JSFile, BasePath).Replace("\\", "/") + @"';
                            document.head.appendChild(insertJs);
                        ");
                    }
                }
            }
        }

        private void _wkOverlay_BrowserPreInit(object sender, EventArgs e)
        {
            if (HasFlag("pstore")) _wkOverlay.Browser.RegisterJsObject("PersistentStore", _addon.Interops.ContainsKey("PersistentStore") ? _addon.Interops["PersistentStore"] : _addon.Interops["PersistentStore"] = new JsInterop.PersistentStore(_addon.DerivedKey));

            if (HasFlag("steamauth")) _wkOverlay.Browser.RegisterJsObject("SteamAuth", SteamAuth.GetInstance());
        }

        string TranslatePath(string path, string root)
        {
            Uri result;
            if(!Uri.TryCreate(path, UriKind.Absolute, out result))
            {
                if (!Path.IsPathRooted(path))
                    path = "addon://" + path;
            }

            return path;
        }

        void ParseManifest(string path)
        {
            if(!File.Exists(path + "\\manifest.json"))
            {
                throw new FileNotFoundException("Could not locate overlay manifest.json in " + path);
            }

            JsonConvert.PopulateObject(File.ReadAllText(path + "\\manifest.json"), this);

            EntryPoint = TranslatePath(EntryPoint, path);

            Console.WriteLine("EntryPoint for " + Key + ":" + EntryPoint);
        }

        private void _wkOverlay_BrowserReady(object sender, EventArgs e)
        {
            _wkOverlay.Browser.LoadError += Browser_LoadError;
            BrowserAddonMap.Add(_wkOverlay.Browser.GetBrowser(), _addon);
            _wkOverlay.Browser.LoadingStateChanged += Browser_LoadingStateChanged;
            if (DebugMode)
            {
                ((ChromiumWebBrowser)sender).GetBrowser().GetHost().ShowDevTools();
            }
        }

        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            SteamVR_WebKit.SteamVR_WebKit.Log("Load Error on " + _addon.DerivedKey + "." + Key + ": (" + e.ErrorCode + ") " + e.ErrorText);
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
