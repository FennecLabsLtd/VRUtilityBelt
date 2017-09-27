using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Plugins;
using VRUB.Utility;

namespace VRUB.Addons.Plugins
{
    public class PluginManager
    {
        static Dictionary<string, PluginContainer> _plugins = new Dictionary<string, PluginContainer>();

        public static Dictionary<string, PluginContainer> Plugins { get { return _plugins; } }

        public static void RegisterPlugin(string key, PluginContainer pluginContainer) {
            _plugins[key.ToLower()] = pluginContainer;
            Logger.Debug("[PLUGIN] Registered Plugin: " + key);
        }

        public static List<PluginContainer> FetchPlugins(string[] pluginKeys)
        {
            List<PluginContainer> fetched = new List<PluginContainer>();

            foreach (string key in pluginKeys)
            {
                if (_plugins.ContainsKey(key.ToLower()))
                    fetched.Add(_plugins[key.ToLower()]);
                else
                    Logger.Debug("[PLUGIN] Failed to locate plugin: " + key);
            }

            return fetched;
        }
    }
}
