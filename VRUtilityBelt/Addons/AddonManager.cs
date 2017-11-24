using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using VRUB.Steam;
using Steamworks;
using CefSharp;
using VRUB.Addons.Overlays;
using CefSharp.Internals;
using CefSharp.OffScreen;
using VRUB.Utility;
using VRUB.Desktop;
using InternalOverlay = SteamVR_WebKit.Overlay;
using Valve.VR;
using VRUB.Addons.Plugins;
using VRUB.API;

namespace VRUB.Addons
{
    public class AddonManager
    {
        public static AddonManager Instance { get; set; }
        private bool _isRunning;
        private readonly object _isRunningLock = new object();

        DesktopMirrorManager _displayMirrorManager;

        Dictionary<string, Addon> _addons = new Dictionary<string, Addon>();

        public static bool HasInit { get; private set; } = false;

        public List<Addon> Addons { get { return _addons.Values.ToList(); } }

        Dictionary<string, string> searchPaths = new Dictionary<string, string>() {
            { "builtin", Path.Combine(PathUtilities.Constants.AddonPath + "\\builtin") },
            { "custom", Path.Combine(PathUtilities.Constants.AddonPath + "\\custom") },
        };

        public void StartAsync()
        {
            lock (_isRunningLock)
            {
                if (_isRunning)
                {
                    return;
                }

                _isRunning = true;
            }

            Thread addonThread = new Thread(Run);
            addonThread.Name = "Addon Manager";
            addonThread.Start();
        }

        public void Stop()
        {
            lock (_isRunningLock)
            {
                if (!_isRunning)
                {
                    return;
                }

                _isRunning = false;
            }

            SteamVR_WebKit.SteamVR_WebKit.Stop();
        }

        public void Init()
        {
            Instance = this;
            SteamVR_WebKit.SteamVR_WebKit.UseExperimentalOGL = true;
            SteamVR_WebKit.SteamVR_WebKit.DefaultFragmentShaderPath = Path.Combine(PathUtilities.Constants.GlobalShadersPath, "fragShader.frag");
            SteamVR_WebKit.SteamVR_WebKit.PrefixOverlayType = false;
            SteamVR_WebKit.SteamVR_WebKit.FPS = 30;
            SteamVR_WebKit.SteamVR_WebKit.LogEvent += SteamVR_WebKit_LogEvent;

            SteamVR_WebKit.SteamVR_WebKit.PreUpdateCallback += PreUpdate;
            SteamVR_WebKit.SteamVR_WebKit.PreDrawCallback += PreDraw;

            SteamVR_WebKit.SteamVR_WebKit.PostUpdateCallback += PostUpdate;
            SteamVR_WebKit.SteamVR_WebKit.PostDrawCallback += PostDraw;


            CefSettings cefSettings = new CefSettings()
            {
                CachePath = PathUtilities.Constants.GlobalCachePath,
                FocusedNodeChangedEnabled = true,
            };

            cefSettings.RegisterScheme(new CefCustomScheme()
            {
                SchemeName = "addon",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("addon", null),
                IsSecure = true,
                IsLocal = false,
                IsStandard = false,
                IsCorsEnabled = false,
                IsDisplayIsolated = false,
            });

            

            cefSettings.RegisterScheme(new CefCustomScheme()
            {
                SchemeName = "vrub",
                SchemeHandlerFactory = new RestrictedPathSchemeHandler("vrub", PathUtilities.Constants.GlobalStaticResourcesPath),
                IsSecure = true,
                IsLocal = false,
                IsStandard = false,
                IsCorsEnabled = false,
                IsDisplayIsolated = false,
            });

            cefSettings.RegisterScheme(new CefCustomScheme()
            {
                SchemeName = "plugin",
                SchemeHandlerFactory = new PluginSchemeHandler(),
                IsSecure = true,
                IsLocal = false,
                IsStandard = false,
                IsCorsEnabled = false,
                IsDisplayIsolated = false,
            });

            cefSettings.CefCommandLineArgs.Add("enable-widevine-cdm", "1");

            // Will experiment with this at some point.
            //cefSettings.CefCommandLineArgs.Add("touch-events", "1"); 

            SteamVR_WebKit.SteamVR_WebKit.Init(cefSettings);

            if(Directory.Exists(Path.Combine(Environment.CurrentDirectory, "WidevineCdm")))
                Cef.RegisterWidevineCdm(@".\WidevineCdm", new DRM.WidevineCallback());

            if(!Cef.IsInitialized)
            {
                SteamVR_WebKit.SteamVR_WebKit.Log("Failed to Init Cef!");
            }

            if (!_isRunning)
                return;

            CefSharp.Cef.GetGlobalCookieManager().SetStoragePath(PathUtilities.Constants.GlobalCookiePath, false);

            RegisterCallbacks();
            RegisterGlobalEventListeners();
            SetupAnchorOverlays();

            PopulateAddons();
            GetWorkshopAddons();
            SetupPluginsAndStart();
            Permissions.PermissionManager.Load();

            _displayMirrorManager = new DesktopMirrorManager();

            if (ConfigUtility.Get<bool>("desktop.enabled", false))
            {
                _displayMirrorManager.SetupMirrors();
            }

            HasInit = true;

            SteamVR_WebKit.SteamVR_WebKit.RunOverlays();
        }

        void WidevineCallback()
        {

        }

        void RegisterGlobalEventListeners()
        {
            string[] events = {
                "DashboardActivated",
                "DashboardDeactivated",
                "PrimaryDashboardDeviceChanged",
                "IpdChanged",
            };

            foreach(string ev in events)
                SteamVR_Event.Listen(ev, (args) => { GlobalEventHandler(ev, args); });
        }

        void GlobalEventHandler(string eventName, params object[] args)
        {
            foreach(Addon a in _addons.Values)
            {
                a.FireAtOverlays(eventName, args);
            }
        }

        private void SteamVR_WebKit_LogEvent(string line)
        {
            Logger.Info(line);
        }

        public void Run()
        {
            if (!_isRunning)
                return;

            if (!SteamVR_WebKit.SteamVR_WebKit.Initialised)
                Init();
        }

        public void PopulateAddons()
        {
            foreach(KeyValuePair<string, string> path in searchPaths)
            {
                if (Directory.Exists(path.Value))
                {
                    foreach (string folder in Directory.EnumerateDirectories(path.Value))
                    {
                        Addon newAddon = Addon.Parse(folder, path.Key);

                        if(_addons.ContainsKey(newAddon.DerivedKey))
                        {
                            newAddon.Key = newAddon.Key + Path.GetDirectoryName(path.Value);
                        }

                        _addons.Add(newAddon.DerivedKey, newAddon);
                    }
                } else
                {
                    Directory.CreateDirectory(path.Value);
                }
            }
        }

        public void GetWorkshopAddons()
        {
            Dictionary<PublishedFileId_t, string> paths = Steam.Workshop.GetSubscribedItems();

            foreach(KeyValuePair<PublishedFileId_t,string> path in paths)
            {
                if(Directory.Exists(path.Value))
                {
                    Addon newAddon = Addon.Parse(path.Value, path.Key.m_PublishedFileId.ToString());
                    Logger.Info("[WORKSHOP] Found Addon: " + newAddon.Name + ", Workshop ID " + path.Key.m_PublishedFileId.ToString());
                    _addons.Add(newAddon.DerivedKey, newAddon);
                }
            }


        }

        public void SetupPluginsAndStart()
        {
            foreach (Addon a in _addons.Values)
            {
                if (a.Enabled)
                {
                    a.RegisterPlugins();
                    a.Start();
                }
            }
        }

        public Addon GetAddon(string key)
        {
            return _addons.ContainsKey(key) ? _addons[key] : null;
        }

        void RegisterCallbacks()
        {
            Workshop.ItemInstalled += Workshop_ItemInstalled;
            Workshop.FileSubscribed += Workshop_FileSubscribed;
            Workshop.FileUnsubscribed += Workshop_FileUnsubscribed;
        }

        private void Workshop_FileUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t args)
        {
            // TODO: Handle unsubbed item
        }

        private void Workshop_FileSubscribed(RemoteStoragePublishedFileSubscribed_t args)
        {
            // TODO: Handle subbed item
        }

        private void Workshop_ItemInstalled(ItemInstalled_t args)
        {
            // TODO: Handle installed item
        }

        private void PreUpdate(object sender, EventArgs e)
        {
            if(SteamManager.Initialised)
                SteamManager.RunCallbacks();
        }

        private void PreDraw(object sender, EventArgs e)
        {
        }

        private void PostUpdate(object sender, EventArgs e)
        {
            _displayMirrorManager.Update();

            foreach(Addon a in _addons.Values)
            {
                a.Update();
            }

            foreach(PluginContainer plugin in PluginManager.Plugins.Values)
            {
                plugin.LoadedPlugin.GlobalUpdate();
            }
        }

        private void PostDraw(object sender, EventArgs e)
        {
            if(_displayMirrorManager != null)
                _displayMirrorManager.Draw();

            foreach (Addon a in _addons.Values)
            {
                a.Draw();
            }
        }

        void SetupAnchorOverlays()
        {
            Texture_t tex;
            int glId;

            OpenVRTools.OnePixelTexture(out tex, out glId, 1, 1);

            InternalOverlay dashboard = new InternalOverlay("vrub.anchor.dashboard", "VRUB Anchor - Dashboard", 1f, true);
            dashboard.SetAttachment(SteamVR_WebKit.AttachmentType.Overlay, new OpenTK.Vector3(0f, 1.3f, 0.6f), new OpenTK.Vector3(0, 0, -35f), "system.vrdashboard");

            dashboard.SetTexture(ref tex);

            dashboard.ToggleInput(false);
            dashboard.Show();
        }
    }
}
