using Newtonsoft.Json;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CefSharp.OffScreen;
using VRUB.JsInterop;
using VRUB.Addons.Data;
using CefSharp;
using VRUB.Utility;
using VRUB.Addons.Plugins;
using CefSharp.ModelBinding;

namespace VRUB.Addons.Overlays
{
    public class Overlay
    {
        WebKitOverlay _wkOverlay;

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

        [JsonProperty("plugins")]
        public List<string> Plugins { get; set; } = new List<string>();
        
        [JsonIgnore]
        public List<PluginContainer> RegisteredPlugins { get; set; } = new List<PluginContainer>();

        public IBinder Binder { get { return new DefaultBinder(new DefaultFieldNameConverter()); } }

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public WebKitOverlay InternalOverlay { get { return _wkOverlay; } }

        public Overlay(Addon addon)
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
            _wkOverlay.CachePath = Path.Combine(PathUtilities.Constants.BaseCachePath, _addon.DerivedKey);

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "addon",
                SchemeHandlerFactory = new OverlaySchemeHandlerFactory(_addon),
            });

            _wkOverlay.EnableKeyboard = HasFlag("vr_keyboard");

            if(Type == OverlayType.Dashboard || Type == OverlayType.Both)
                _wkOverlay.DashboardOverlay.Width = MeterWidth;
            else
                _wkOverlay.InGameOverlay.Width = MeterWidth;
        }

        public void Start()
        {
            _wkOverlay.StartBrowser();
        }

        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading && e.Browser.HasDocument)
            {
                InsertInjectableFiles();

                foreach (PluginContainer p in RegisteredPlugins)
                {
                    p.LoadedPlugin.OnBrowserNavigation(_addon, this, _wkOverlay.Browser);
                }
            }
        }

        void InsertInjectableFiles()
        {
            if(Inject != null)
            {
                if(Inject.CSS != null)
                {
                    foreach (string CSSFile in Inject.CSS) {
                        InjectCssFile(CSSFile);
                    }
                }

                if (Inject.JS != null)
                {
                    foreach (string JSFile in Inject.JS)
                    {
                        InjectJsFile(JSFile);
                    }
                }
            }
        }

        public void InjectCssFile(string CSSFile)
        {
            if (CSSFile.StartsWith("addon://") || PathUtilities.IsInFolder(_addon.BasePath, PathUtilities.GetTruePath(_addon.BasePath, CSSFile)))
            {
                _wkOverlay.TryExecAsyncJS(@"
                            var insertCss = document.createElement('link');
                            insertCss.rel = 'stylesheet';
                            insertCss.href = '" + TranslatePath(CSSFile, _addon.BasePath).Replace("\\", "/") + @"';
                            document.head.appendChild(insertCss);
                        ");
            }
            else
            {
                Logger.Warning("[OVERLAY] Not injecting " + CSSFile + " as it is not in the addon path");
            }
        }

        public void InjectJsFile(string JSFile)
        {
            if (JSFile.StartsWith("addon://") || PathUtilities.IsInFolder(_addon.BasePath, PathUtilities.GetTruePath(_addon.BasePath, JSFile)))
            {
                _wkOverlay.TryExecAsyncJS(@"
                            var insertJs = document.createElement('script');
                            insertJs.src = '" + TranslatePath(JSFile, _addon.BasePath).Replace("\\", "/") + @"';
                            document.head.appendChild(insertJs);
                        ");
            }
            else
            {
                Logger.Warning("[OVERLAY] Not injecting " + JSFile + " as it is not in the addon path");
            }
        }

        private void _wkOverlay_BrowserPreInit(object sender, EventArgs e)
        {
            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.OnBrowserPreInit(_addon, this, _wkOverlay.Browser);
            }

            if (HasFlag("steamauth")) _wkOverlay.Browser.RegisterJsObject("VRUB_Plugins_SteamAuth", new SteamAuth());
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

            Logger.Debug("[OVERLAY] EntryPoint for " + Key + ":" + EntryPoint);
        }

        public void RegisterPlugins()
        {
            RegisteredPlugins = new List<PluginContainer>();

            List<PluginContainer> plugins = PluginManager.FetchPlugins(Plugins.ToArray());

            foreach(PluginContainer p in plugins)
            {
                if (p.LoadedPlugin != null)
                {
                    RegisteredPlugins.Add(p);
                    p.LoadedPlugin.OnRegister(_addon, this);
                }
            }
        }

        private void _wkOverlay_BrowserReady(object sender, EventArgs e)
        {
            _wkOverlay.Browser.LoadError += Browser_LoadError;
            _wkOverlay.Browser.LoadingStateChanged += Browser_LoadingStateChanged;

            foreach(PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.OnBrowserReady(_addon, this, _wkOverlay.Browser);
            }

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
            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.Draw(_addon, this);
            }
        }

        public void Update()
        {
            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.Update(_addon, this);
            }
        }
    }
}
