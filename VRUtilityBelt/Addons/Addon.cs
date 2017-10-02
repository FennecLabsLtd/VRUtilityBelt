using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Overlays;
using VRUB.Addons.Thermes;
using VRUB.API;
using VRUB.Utility;
using VRUB.Addons.Plugins;

namespace VRUB.Addons
{
    public class Addon
    {
        FileSystemWatcher _folderWatcher;

        bool _pluginsEnabled = false;

        #region Datums

        [JsonProperty("key")]
        public String Key { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("overlays")]
        public List<string> OverlayKeys { get; set; } = new List<string>();

        [JsonIgnore]
        public List<Overlay> Overlays { get; set; }

        [JsonProperty("themes")]
        public List<string> ThemeKeys { get; set; } = new List<string>();

        [JsonIgnore]
        public List<ITheme> Themes { get; set; }

        [JsonProperty("plugins")]
        public List<string> PluginKeys { get; set; } = new List<string>();

        [JsonIgnore]
        public List<PluginContainer> Plugins { get; set; }

        private String ManifestPath { get { return BasePath + "\\manifest.json"; } }

        public string DerivedKey
        {
            get
            {
                return _keyPrefix + "_" + Key;
            }
        }

        public Dictionary<string, object> Interops { get; set; } = new Dictionary<string, object>();

        public string BasePath { get; set; }
        string _keyPrefix = "builtin";

#endregion

        public static Addon Parse(string folder, string keyPrefix = "builtin")
        {
            Addon newAddon = new Addon();
            newAddon._keyPrefix = keyPrefix;
            newAddon.BasePath = folder;
            newAddon.ProcessManifest();

            Logger.Info("[ADDON] Found Addon: " + newAddon.Key + " - " + newAddon.Name);

            newAddon.Interops.Add("permissions", new Permissions.PermissionInterop(newAddon));

            if (keyPrefix == "builtin")
                newAddon._pluginsEnabled = true;

            newAddon.SetupOverlays();
            newAddon.SetupThemes();
            newAddon.SetupPlugins();

            newAddon.SetupFileWatchers();

            return newAddon;
        }

        public void Start()
        {
            foreach(Overlay o in Overlays)
            {
                o.Start();
            }
        }

        public void RegisterPlugins()
        {
            foreach(Overlay o in Overlays)
            {
                o.RegisterPlugins();
            }
        }

        void ProcessManifest()
        {
            if(!File.Exists(ManifestPath))
            {
                throw new FileNotFoundException("Cannot locate manifest.json in " + BasePath);
            }

            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(ManifestPath), this);
            } catch(Exception e)
            {
                Logger.Warning("[JSON] Failed to parse Addon Manifest at " + ManifestPath + ": " + e.Message);
            }
        }

        void SetupFileWatchers()
        {
            _folderWatcher = new FileSystemWatcher(BasePath);
            _folderWatcher.IncludeSubdirectories = true;

            _folderWatcher.Changed += _folderWatcher_Updated;
            _folderWatcher.Deleted += _folderWatcher_Updated;
            _folderWatcher.Renamed += _folderWatcher_Updated;
        }

        void SetupOverlays()
        {
            Overlays = new List<Overlay>();

            foreach(string key in OverlayKeys)
            {
                Overlay newOverlay = new Overlay(this);
                newOverlay.Setup(BasePath + "\\overlays\\" + key);

                Overlays.Add(newOverlay);
            }
        }

        void SetupThemes()
        {
            // Coming sooner than plugins.
        }

        void SetupPlugins()
        {
            Plugins = new List<PluginContainer>();

            if (!_pluginsEnabled)
                return;

            foreach(string key in PluginKeys)
            {
                PluginContainer newPlugin = new PluginContainer(this, key);
                newPlugin.Setup(BasePath + "\\plugins\\" + key);

                Plugins.Add(newPlugin);
            }
        }

        private void _folderWatcher_Updated(object sender, FileSystemEventArgs e)
        {
            if(Directory.Exists(BasePath))
                Refresh();
        }

        public void Refresh()
        {
            // TODO: Make it refresh
        }

        void Dispose()
        {
            if(_folderWatcher != null)
                _folderWatcher.Dispose();
        }

        public void FireAtOverlays(string eventName, object value)
        {
            foreach(Overlay o in Overlays)
            {
                if (o.Bridge != null)
                    o.Bridge.FireEvent(eventName, value);
            }
        }
    }
}
