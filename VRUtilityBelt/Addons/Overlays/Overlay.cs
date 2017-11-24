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
using VRUB.Bridge;
using VRUB.Addons.Overlays;
using Valve.VR;
using OpenTK;
using System.Drawing;

namespace VRUB.Addons.Overlays
{
    public class Overlay
    {
        WebKitOverlay _wkOverlay;

        Addon _addon;

        public string BasePath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("entrypoint")]
        public string EntryPoint { get; set; } = "index.html";

        [JsonProperty("width")]
        public int Width { get; set; } = 1200;

        [JsonProperty("height")]
        public int Height { get; set; } = 800;

        [JsonProperty("meters")]
        public float MeterWidth { get; set; } = 2.5f;

        [JsonProperty("floating_meters")]
        public float FloatingMeterWidth { get; set; } = 2.5f;

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
        public bool PersistSessionCookies { get; set; } = false;

        [JsonProperty("mouse_delta_tolerance")]
        public int MouseDeltaTolerance { get; set; } = 20;

        [JsonProperty("opacity")]
        public float Opacity { get; set; } = 0.9f;

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = null;

        [JsonProperty("attachment")]
        public OverlayAttachment OverlayAttachment { get; set; } = new OverlayAttachment() { Type = AttachmentType.Absolute, Position = Vector3.Zero, Rotation = Vector3.Zero };

        [JsonProperty("show_as_dashboard")]
        public bool ShowAsDashboard { get; set; } = true;

        [JsonProperty("show_as_floating")]
        public bool ShowAsFloating { get; set; } = false;

        [JsonProperty("render_models")]
        public List<OverlayRenderModel> RenderModels { get; set; } = new List<OverlayRenderModel>();

        [JsonProperty("disable_scrolling")]
        public bool DisableScrolling { get; set; } = false;

        [JsonProperty("alphamask")]
        public string AlphaMask { get; set; }

        [JsonProperty("fragment_shader")]
        public string FragmentShader { get; set; }
        
        [JsonIgnore]
        public List<PluginContainer> RegisteredPlugins { get; set; } = new List<PluginContainer>();

        public IBinder Binder => new DefaultBinder(new DefaultFieldNameConverter());

        public class AssetsContainer
        {
            public List<string> SASS { get; set; }
        }

        public AssetsContainer Assets { get; set; }

        public WebKitOverlay InternalOverlay => _wkOverlay;

        public BridgeHandler Bridge { get; set; }

        public Addon Addon { get { return _addon; } }

        bool _doDestroy = false;

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

            if(!ShowAsDashboard && !ShowAsFloating)
            {
                Logger.Fatal("[OVERLAY] Overlay must have show_as_dashboard and/or show_as_floating set to true, both as false is not permitted.");
                return;
            }

            if (!Environment.GetCommandLineArgs().Contains("-debug"))
                DebugMode = false;

            _wkOverlay = new WebKitOverlay(new Uri(EntryPoint), Width, Height, "vrub." + DerivedKey, Name, ShowAsDashboard ? (ShowAsFloating ? OverlayType.Both : OverlayType.Dashboard) : OverlayType.InGame);
            _wkOverlay.BrowserReady += _wkOverlay_BrowserReady;
            _wkOverlay.BrowserPreInit += _wkOverlay_BrowserPreInit;
            _wkOverlay.CachePath = Path.Combine(GetLocalStoragePath(), "Cache");
            _wkOverlay.RequestContextHandler = new OverlayRequestContextHandler(this);
            _wkOverlay.MouseDeltaTolerance = MouseDeltaTolerance;
            _wkOverlay.MessageHandler.DebugMode = DebugMode;
            _wkOverlay.FragmentShaderPath = FragmentShader;

            if (AlphaMask != null)
                SetAlphaMask(AlphaMask);

            _wkOverlay.AllowScrolling = !DisableScrolling;

            _wkOverlay.UpdateInputSettings();

            Attach();
            SetupRenderModels();

            if(Thumbnail != null && _wkOverlay.DashboardOverlay != null)
            {
                string thumbPath = Path.IsPathRooted(Thumbnail) ? Thumbnail : Path.Combine(path, Thumbnail);

                if (File.Exists(thumbPath))
                    _wkOverlay.DashboardOverlay.SetThumbnail(thumbPath);
                else
                    Logger.Warning("[OVERLAY] Failed to locate thumbnail for " + DerivedKey + ": " + thumbPath);
            }

            SetOpacity(Opacity);

            if (ShowAsFloating)
                _wkOverlay.EnableNonDashboardInput = EnableMouseInput;

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "addon",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("addon", _addon.BasePath),
            });

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "vrub",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("vrub", PathUtilities.Constants.GlobalStaticResourcesPath),
            });

            _wkOverlay.SchemeHandlers.Add(new CefCustomScheme()
            {
                SchemeName = "plugin",
                SchemeHandlerFactory = new PluginSchemeHandler(),
            });

            _wkOverlay.EnableKeyboard = EnableKeyboard;

            UpdateWidths();
        }

        public void SetAlphaMask(string path)
        {
            path = PathUtilities.GetTruePath(BasePath, path);

            if(File.Exists(path))
            {
                try
                {
                    Bitmap bmp = new Bitmap(path);
                    _wkOverlay.AlphaMask = bmp;
                } catch(Exception e)
                {
                    Logger.Error("[OVERLAY] Failed to set Alpha Mask BMP: " + e.Message);
                }
            }
        }

        public void UpdateWidths()
        {
            if (ShowAsDashboard)
                _wkOverlay.DashboardOverlay.Width = MeterWidth;

            if (ShowAsFloating)
                _wkOverlay.InGameOverlay.Width = FloatingMeterWidth;
        }

        void Attach()
        {
            if(_wkOverlay.InGameOverlay != null)
            {
                _wkOverlay.InGameOverlay.SetAttachment(OverlayAttachment.Type, OverlayAttachment.Position, OverlayAttachment.Rotation, OverlayAttachment.AttachmentKey);
            }
        }

        public void Start()
        {
            _wkOverlay.StartBrowser();
        }

        public void Stop()
        {
            _doDestroy = true;
        }

        void SetupRenderModels()
        {
            foreach(OverlayRenderModel rm in RenderModels)
            {
                try
                {
                    rm.Setup(this);
                } catch(Exception e)
                {
                    Logger.Error("[RENDERMODEL] Failed to produce render model overlay for " + rm.Key + ": " + e.Message);
                }
            }
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

        void SetOpacity(float opacity)
        {
            if (_wkOverlay.InGameOverlay != null)
                _wkOverlay.InGameOverlay.Alpha = opacity;

            if (_wkOverlay.DashboardOverlay != null)
                _wkOverlay.DashboardOverlay.Alpha = opacity;
        }

        void InsertInjectableFiles()
        {
            InjectJsFile("vrub://VRUB.js");

            if (Inject != null)
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
            _wkOverlay.Browser.DragHandler = new OverlayDragHandler(this);
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
            _wkOverlay.Destroy();
            Bridge.Deregister();
            _doDestroy = false;

            foreach(OverlayRenderModel rm in RenderModels)
            {
                rm.Destroy();
            }
        }

        public void Draw()
        {
            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.Draw(_addon, this);
            }

            foreach (OverlayRenderModel model in RenderModels)
            {
                model.Draw();
            }
        }

        public void Update()
        {
            foreach (PluginContainer p in RegisteredPlugins)
            {
                p.LoadedPlugin.Update(_addon, this);
            }

            foreach(OverlayRenderModel model in RenderModels)
            {
                model.Update();
            }

            if (_doDestroy)
                Destroy();
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
