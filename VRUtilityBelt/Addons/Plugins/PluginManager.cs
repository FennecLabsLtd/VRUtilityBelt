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
        public static Dictionary<string, PluginContainer> _plugins = new Dictionary<string, PluginContainer>();

        public static void RegisterPlugin(string key, PluginContainer pluginContainer) {
            _plugins[key] = pluginContainer;
            Logger.Debug("[PLUGIN] Registered Plugin: " + key);
        }

        public static List<PluginContainer> FetchPlugins(string[] pluginKeys)
        {
            List<PluginContainer> fetched = new List<PluginContainer>();

            foreach(KeyValuePair<string,PluginContainer> plugin in _plugins)
            {
                if (pluginKeys.Contains(plugin.Key))
                    fetched.Add(plugin.Value);
            }

            return fetched;
        }
    }
}
