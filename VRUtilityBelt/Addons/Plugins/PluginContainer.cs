using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Data;
using VRUB.API;
using VRUB.Utility;

namespace VRUB.Addons.Plugins
{
    public class PluginContainer
    {
        Addon _addon;
        string _key;

        public String Key { get { return _key; } }

        public string BasePath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dll")]
        public string Dll { get; set; }

        IPlugin _loadedPlugin;

        public IPlugin LoadedPlugin { get { return _loadedPlugin; } }

        public PluginContainer(Addon addon, string key)
        {
            _addon = addon;
            _key = key;
        }

        public void Setup(string path)
        {
            Logger.Trace("[PLUGIN] Loading Plugin at " + path);
            BasePath = path;
            ParseManifest(path);
            LoadDll();

            RegisterPlugin();
        }

        void RegisterPlugin()
        {
            PluginManager.RegisterPlugin(_addon.Key + "_" + _key, this);
        }

        void DeregisterPlugin()
        {
            PluginManager.DeregisterPlugin(_addon.Key + "_" + _key);
        }

        void ParseManifest(string path)
        {
            if (!File.Exists(path + "\\manifest.json"))
            {
                throw new FileNotFoundException("Could not locate plugin manifest.json in " + path);
            }

            JsonConvert.PopulateObject(File.ReadAllText(path + "\\manifest.json"), this);
        }

        void LoadDll()
        {
            if(!File.Exists(Path.Combine(BasePath, Dll)))
            {
                Logger.Error("[PLUGIN] Failed to locate " + Path.Combine(BasePath, Dll));
                DeregisterPlugin();
                return;
            }

            Assembly asm = Assembly.LoadFrom(Path.Combine(BasePath, Dll));

            try
            {
                Type[] types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Error(e.Message);
            }

            foreach (Type t in asm.GetTypes())
            {
                if (t.IsInterface)
                    continue;

                if (t.Name == "Plugin")
                {
                    _loadedPlugin = (IPlugin)Activator.CreateInstance(t);
                    break;
                }
            }

            if (_loadedPlugin != null)
            {
                _loadedPlugin.OnLoad(AddonManager.Instance, _addon);
            }
        }
    }
}
