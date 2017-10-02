﻿using Newtonsoft.Json;
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
using VRUB.Bridge;
using VRUB.Addons.Overlays;

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

        [JsonProperty("meters")]
        public float MeterWidth { get; set; } = 2f;

        [JsonProperty("debug")]
        public bool DebugMode { get; set; } = false;

        [JsonProperty("keyboard")]
        public bool EnableKeyboard { get; set; }

        [JsonProperty("mouse")]
        public bool EnableMouseInput { get; set; }

        [JsonProperty("inject")]
        public InjectableFiles Inject { get; set; }

        [JsonProperty("plugins")]
        public List<string> Plugins { get; set; } = new List<string>();

        [JsonProperty("persist_session_cookies")]
        public bool PersistSessionCookies { get; set; }
        
        [JsonIgnore]
        public List<PluginContainer> RegisteredPlugins { get; set; } = new List<PluginContainer>();

        public IBinder Binder { get { return new DefaultBinder(new DefaultFieldNameConverter()); } }

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public WebKitOverlay InternalOverlay { get { return _wkOverlay; } }

        public BridgeHandler Bridge { get; set; }

        public string DerivedKey
        {
            get
            {
                return _addon.DerivedKey + "." + Key;
            }
        }

        public Overlay(Addon addon)
        {
            Bridge = new BridgeHandler(this);
            Bridge.RegisterLink("VRUB_Core_PermissionManager", addon.Interops["permissions"]);
            _addon = addon;
        }

        public void Setup(string path, string keyPrefix = "builtin")
        {
            BasePath = path;
            ParseManifest(path);

            if (!Environment.GetCommandLineArgs().Contains("-debug"))
                DebugMode = false;

            _wkOverlay = new WebKitOverlay(new Uri(EntryPoint), Width, Height, "vrub." + DerivedKey, Name, Type);
            _wkOverlay.BrowserReady += _wkOverlay_BrowserReady;
            _wkOverlay.BrowserPreInit += _wkOverlay_BrowserPreInit;
            _wkOverlay.CachePath = Path.Combine(GetLocalStoragePath(), "Cache");
            _wkOverlay.RequestContextHandler = new OverlayRequestContextHandler(this);

            if (Type == OverlayType.InGame || Type == OverlayType.Both)
                _wkOverlay.EnableNonDashboardInput = EnableMouseInput;

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "addon",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("addon", _addon.BasePath),
            });

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "vrub",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("vrub", PathUtilities.Constants.GlobalResourcesPath),
            });

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "plugin",
                SchemeHandlerFactory = new PluginSchemeHandler(),
            });

            _wkOverlay.EnableKeyboard = EnableKeyboard;

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

            InjectJsFile("vrub://VRUB.js");
        }

        public void InjectCssFile(string CSSFile)
        {
            if (CSSFile.StartsWith("addon://") || CSSFile.StartsWith("vrub://") || CSSFile.StartsWith("plugin://") || PathUtilities.IsInFolder(_addon.BasePath, PathUtilities.GetTruePath(_addon.BasePath, CSSFile)))
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
            if (JSFile.StartsWith("addon://") || JSFile.StartsWith("vrub://") || JSFile.StartsWith("plugin://") || PathUtilities.IsInFolder(_addon.BasePath, PathUtilities.GetTruePath(_addon.BasePath, JSFile)))
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
            _wkOverlay.Browser.RequestHandler = new PassThroughRequestHandler(this);
            _wkOverlay.Browser.RegisterAsyncJsObject("VRUB_Interop_Bridge", Bridge, new BindingOptions() { CamelCaseJavascriptNames = false, Binder = Binder });

            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.OnBrowserPreInit(_addon, this, _wkOverlay.Browser);
            }

            //if (HasFlag("steamauth")) _wkOverlay.Browser.RegisterJsObject("VRUB_Plugins_SteamAuth", new SteamAuth());
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
            if (e.ErrorCode == CefErrorCode.Aborted) // Don't wanna spam for steam:// links.
                return;

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

        public string GetCloudStoragePath()
        {
            return Path.Combine(PathUtilities.Constants.BaseRoamingPath, "Addons\\" + _addon.DerivedKey);
        }

        public string GetLocalStoragePath()
        {
            return Path.Combine(PathUtilities.Constants.BaseLocalAppDataPath, "Addons\\" + _addon.DerivedKey);
        }

        public void ShowKeyboard(bool toggle = true, string value = "")
        {
            if (toggle)
                InternalOverlay.ShowKeyboard(value);
            else
                InternalOverlay.HideKeyboard();
        }
    }
}
