using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using VRUtilityBelt.Steam;

namespace VRUtilityBelt.Addons
{
    public class AddonManager
    {
        private bool _isRunning;
        private readonly object _isRunningLock = new object();

        Dictionary<string, Addon> _addons = new Dictionary<string, Addon>();

        Dictionary<string, string> searchPaths = new Dictionary<string, string>() {
            { "builtin", Environment.CurrentDirectory + "\\addons\\builtin" },
            { "custom", Environment.CurrentDirectory + "\\addons\\custom" },
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

            new Thread(Run).Start();
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
            SteamVR_WebKit.SteamVR_WebKit.FPS = 30;
            SteamVR_WebKit.SteamVR_WebKit.LogEvent += SteamVR_WebKit_LogEvent;

            SteamVR_WebKit.SteamVR_WebKit.PreUpdateCallback += PreUpdate;
            SteamVR_WebKit.SteamVR_WebKit.PreDrawCallback += PreDraw;

            SteamVR_WebKit.SteamVR_WebKit.PostUpdateCallback += PostUpdate;
            SteamVR_WebKit.SteamVR_WebKit.PostDrawCallback += PostDraw;

            SteamVR_WebKit.SteamVR_WebKit.Init(new CefSharp.CefSettings()
            {
                CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VRUtilityBelt\\BrowserCache",
            });

            CefSharp.Cef.GetGlobalCookieManager().SetStoragePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VRUtilityBelt\\Cookies", false);

            PopulateAddons();
        }
        private void SteamVR_WebKit_LogEvent(string line)
        {
            Console.WriteLine("[WEBKIT]: " + line);
        }

        public void Run()
        {
            if (!SteamVR_WebKit.SteamVR_WebKit.Initialised)
                Init();

            SteamVR_WebKit.SteamVR_WebKit.RunOverlays();
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
                    }
                } else
                {
                    Directory.CreateDirectory(path.Value);
                }
            }
        }

        public void GetWorkshopAddons()
        {
            
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
        }

        private void PostDraw(object sender, EventArgs e)
        {
        }
    }
}
