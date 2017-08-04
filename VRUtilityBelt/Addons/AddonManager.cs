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
using Steamworks;

namespace VRUtilityBelt.Addons
{
    public class AddonManager
    {
        private bool _isRunning;
        private readonly object _isRunningLock = new object();

        private bool _stopping = false;

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

            if (!_isRunning)
                return;

            CefSharp.Cef.GetGlobalCookieManager().SetStoragePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VRUtilityBelt\\Cookies", false);

            RegisterCallbacks();

            PopulateAddons();
        }
        private void SteamVR_WebKit_LogEvent(string line)
        {
            Console.WriteLine("[WEBKIT]: " + line);
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

                        if(_addons.ContainsKey(newAddon.Key))
                        {
                            newAddon.Key = newAddon.Key + Path.GetDirectoryName(path.Value);
                        }

                        _addons.Add(newAddon.Key, newAddon);
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
                    _addons.Add(path.Key.m_PublishedFileId.ToString() + "_" + newAddon.Key, newAddon);
                }
            }
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
        }

        private void PostDraw(object sender, EventArgs e)
        {
        }
    }
}
