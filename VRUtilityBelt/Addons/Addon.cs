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
using VRUB.Addons.Data;

namespace VRUB.Addons
{
    public class Addon
    {
        FileSystemWatcher _folderWatcher;

        bool _pluginsEnabled = false;

        public event EventHandler Disabled;

        #region Datums

        [JsonProperty("key")]
        public String Key { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("author")]
        public String Author { get; set; }

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

        [JsonProperty("permissions")]
        public Dictionary<string, string> Permissions { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [JsonProperty("default_to_disabled")]
        public bool DefaultToDisabled = false;

        [JsonIgnore]
        public AddonSource Source
        {
            get
            {
                if (_keyPrefix == "builtin")
                    return AddonSource.BuiltIn;
                else if (_keyPrefix == "custom")
                    return AddonSource.Custom;
                else
                    return AddonSource.Workshop;
            }
        }

        /// <summary>
        /// Not _real_ sudo, just access to everything without needing permissions granted. Only available to built-in overlays.
        /// </summary>
        [JsonProperty("sudo")]
        public bool SudoAccess { get; set; } = false;

        [JsonIgnore]
        public List<PluginContainer> Plugins { get; set; }

        private String ManifestPath { get { return BasePath + "\\manifest.json"; } }

        public bool Enabled
        {
            get
            {
                try
                {
                    return _enabled;
                }
                catch
                {
                    return false;
                }
            }

            set
            {
                if (SudoAccess && !value)
                    Enabled = true;

                ConfigUtility.Set("addons." + DerivedKey, value ? "1" : "0");

                if (!_enabled && value)
                    OnEnabled(_hasBeenEnabledAtSomePoint);

                if (!value && _enabled)
                {
                    OnDisabled();
                }

                _enabled = value;

                if (value)
                    _hasBeenEnabledAtSomePoint = true;
            }
        }

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

        bool _hasBeenEnabledAtSomePoint = false;
        bool _enabled = false;
        bool _pendingDestructionTick = false;

#endregion

        public static Addon Parse(string folder, string keyPrefix = "builtin")
        {
            Addon newAddon = new Addon();
            newAddon._keyPrefix = keyPrefix;
            newAddon.BasePath = folder;
            newAddon.ProcessManifest();

            if (keyPrefix != "builtin")
                newAddon.SudoAccess = false;

            Logger.Info("[ADDON] Found Addon: " + newAddon.Key + " - " + newAddon.Name);

            newAddon.Interops.Add("permissions", new Permissions.PermissionInterop(newAddon));
            newAddon.SetupConfig();


            if (keyPrefix == "builtin")
                newAddon._pluginsEnabled = true;

            if (newAddon.Enabled)
            {
                newAddon.OnEnabled(false);
            }

            return newAddon;
        }

        void OnEnabled(bool softEnable)
        {
            SetupOverlays();
            SetupThemes();

            _hasBeenEnabledAtSomePoint = true;

            if (!softEnable)
            {
                SetupPlugins();
                SetupFileWatchers();
            } else
            {
                Start(); // We do this here because a hard enable does it later on.
            }
        }

        void OnDisabled()
        {
            FireAtOverlays("Disabled", null);

            foreach(Overlay o in Overlays)
            {
                o.Stop();
            }

            Disabled?.Invoke(this, new EventArgs());

            _pendingDestructionTick = true;
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
                try
                {
                    Overlay newOverlay = new Overlay(this);
                    newOverlay.Setup(BasePath + "\\overlays\\" + key);
                    Overlays.Add(newOverlay);
                } catch(Exception e)
                {
                    Logger.Fatal("[OVERLAY] Failed to setup " + key + " due to: " + e.Message);
                }
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

        void SetupConfig()
        {
            try
            {
                _enabled = ConfigUtility.Get("addons." + DerivedKey) == "1";
            } catch(KeyNotFoundException)
            {
                ConfigUtility.Set("addons." + DerivedKey, DefaultToDisabled ? "0" : "1");
                _enabled = true;
            }
        }

        public string GetPermissionReasoning(string permissionKey)
        {
            if (Permissions.ContainsKey(permissionKey))
                return Permissions[permissionKey];
            else
                return null;
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

        public void Update()
        {
            if (!_enabled && !_pendingDestructionTick)
                return;

            foreach(Overlay o in Overlays)
            {
                o.Update();
            }

        }

        public void Draw()
        {
            if (!_enabled && !_pendingDestructionTick)
                return;

            foreach(Overlay o in Overlays)
            {
                o.Draw();
            }

            if (!_enabled)
                _pendingDestructionTick = false;
        }
    }
}
