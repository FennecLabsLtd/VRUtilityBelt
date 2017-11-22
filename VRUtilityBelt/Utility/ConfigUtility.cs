using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using VRUB.Addons;

namespace VRUB.Utility
{
    public class ConfigUtility
    {
        static Dictionary<string, Dictionary<string, object>> configValues = new Dictionary<string, Dictionary<string, object>>();
        static Dictionary<string, List<ConfigLayout>> addonConfigLayouts = new Dictionary<string, List<ConfigLayout>>();

        public delegate void UpdateHandler(string key, object value);

        public static void Load() {
            if (!Directory.Exists(PathUtilities.Constants.ConfigPath))
                Directory.CreateDirectory(PathUtilities.Constants.ConfigPath);

            foreach (string file in Directory.EnumerateFiles(PathUtilities.Constants.ConfigPath)) {
                if (Path.GetFileName(file) == "permissions.json")
                    continue;

                string moduleKey = Path.GetFileNameWithoutExtension(file);

                try
                {
                    configValues.Add(moduleKey, JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file)));
                } catch (JsonReaderException e)
                {
                    Logger.Error("[CONFIG] Failed to read " + file + ": " + e.Message);
                }
            }
        }

        public static Dictionary<string, object> GetModuleConfig(string key)
        {
            if (configValues.ContainsKey(key))
                return configValues[key];
            else
                return null;
        }

        public static object GetObject(string dotPath, object defaultValue = null)
        {
            string[] splitPath = dotPath.Split(new char[] { '.' }, 2);

            if (splitPath.Length == 2)
            {
                if (configValues.ContainsKey(splitPath[0])) {
                    if (configValues[splitPath[0]].ContainsKey(splitPath[1]))
                        return configValues[splitPath[0]][splitPath[1]];
                    else
                    {
                        if (defaultValue != null)
                            return defaultValue;
                        else
                            throw new KeyNotFoundException("Invalid Config Key " + splitPath[1]);
                    }
                }
                else
                {
                    if (defaultValue != null)
                        return defaultValue;
                    else
                        throw new KeyNotFoundException("Invalid File Key " + splitPath[0]);
                }
            } else
            {
                if (defaultValue != null)
                    return defaultValue;
                else
                    throw new KeyNotFoundException("Please provide a full path to a config value.");
            }
        }

        public static dynamic Get<T>(string dotPath, T defaultValue = default(T))
        {
            object value = GetObject(dotPath, defaultValue);

            Type returnType = typeof(T);

            if (returnType == typeof(float))
            {
                float output;
                if (float.TryParse(value.ToString(), out output))
                    return output;
                else
                    return defaultValue;
            }
            else if (returnType == typeof(double))
            {
                double output;
                if (double.TryParse(value.ToString(), out output))
                {
                    return output;
                }
                else
                {
                    return defaultValue;
                }
            }
            else if (returnType == typeof(int))
            {
                int output;
                if (int.TryParse(value.ToString(), out output))
                {
                    return output;
                }
                else
                {
                    return defaultValue;
                }
            } else if (returnType == typeof(bool))
            {
                bool output;
                if (value.ToString() == "1")
                    return true;
                else if (value.ToString() == "0")
                    return false;
                else if (bool.TryParse(value.ToString(), out output))
                {
                    return output;
                } else
                {
                    return defaultValue;
                }
            }

            return (T)value;
        }

        public static void Set(string dotPath, object value)
        {
            string[] splitPath = dotPath.Split(new char[] { '.' }, 2);

            if (splitPath.Length == 2)
            {
                if (!configValues.ContainsKey(splitPath[0]))
                    configValues.Add(splitPath[0], new Dictionary<string, object>());

                if (configValues[splitPath[0]].ContainsKey(splitPath[1]))
                    configValues[splitPath[0]][splitPath[1]] = value;
                else
                    configValues[splitPath[0]].Add(splitPath[1], value);

                Save();
            } else
            {
                throw new KeyNotFoundException("Please provide a full path to a config value.");
            }

            Send(dotPath, dotPath, value);
        }

        static void Save()
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> keyVal in configValues) {
                File.WriteAllText(Path.Combine(PathUtilities.Constants.ConfigPath, keyVal.Key + ".json"), JsonConvert.SerializeObject(keyVal.Value));
            }
        }

        public static void SetDefaults(string key, Dictionary<string, object> values)
        {
            if (!configValues.ContainsKey(key)) {
                configValues.Add(key, values);
                Save();
                return;
            }

            foreach (KeyValuePair<string, object> cfgVal in values)
            {
                if (!configValues[key].ContainsKey(cfgVal.Key))
                    configValues[key].Add(cfgVal.Key, cfgVal.Value);
            }

            Save();
        }

        public static void Listen(string message, UpdateHandler action)
        {
            var actions = listeners[message] as UpdateHandler;
            if (actions != null)
            {
                listeners[message] = actions + action;
            }
            else
            {
                listeners[message] = action;
            }
        }

        public static void Remove(string message, UpdateHandler action)
        {
            var actions = listeners[message] as UpdateHandler;
            if (actions != null)
            {
                listeners[message] = actions - action;
            }
        }

        public static void Send(string message, string key, object value)
        {
            if (listeners[message] is UpdateHandler actions)
            {
                actions(key, value);
            }
        }

        public static void RegisterAddonConfig(Addon addon)
        {
            if (addonConfigLayouts.ContainsKey("addon_" + addon.DerivedKey))
                addonConfigLayouts.Remove("addon_" + addon.DerivedKey);

            if (File.Exists(Path.Combine(addon.BasePath, "config.json")))
            {
                List<ConfigLayout> configSettings = JsonConvert.DeserializeObject<List<ConfigLayout>>(File.ReadAllText(Path.Combine(addon.BasePath, "config.json"))).Where(l => l.Key != "enabled").ToList();

                configSettings.Add(new ConfigLayout() { Key = "enabled", Title = "Enabled", Type = "bool", Category = "Addon Settings", Description = "Enable this addon?", Addon = addon });

                foreach (ConfigLayout layout in configSettings)
                {
                    layout.Addon = addon;
                }

                addonConfigLayouts.Add("addon_" + addon.DerivedKey, configSettings);

                Dictionary<string, object> defaults = new Dictionary<string, object>();

                foreach (ConfigLayout layout in configSettings)
                {
                    if (layout.Key == "enabled")
                        continue;

                    defaults[layout.Key] = layout.Default;
                }

                SetDefaults("addon_" + addon.DerivedKey, defaults);
            }
            else
            {
                List<ConfigLayout> configSettings = new List<ConfigLayout>() { new ConfigLayout() { Key = "enabled", Title = "Enabled", Type = "bool", Category = "Addon Settings", Description = "Enable this addon?", Addon = addon } };

                addonConfigLayouts.Add("addon_" + addon.DerivedKey, configSettings);
            }
        }

        public static void RegisterCustomConfigLayout(string key, List<ConfigLayout> layouts)
        {
            addonConfigLayouts.Add(key, layouts);

            Dictionary<string, object> defaults = new Dictionary<string, object>();

            foreach (ConfigLayout layout in layouts)
            {
                if (layout.Key == "enabled")
                    continue;

                defaults[layout.Key] = layout.Default;
            }

            SetDefaults(key, defaults);
        }

        public static Dictionary<string, List<ConfigLayout>> GetLayouts()
        {
            return addonConfigLayouts;
        }

        private static Hashtable listeners = new Hashtable();

        public class ConfigLayout
        {
            public Addon Addon { get; set; }

            /// <summary>
            /// Used for config layouts that don't have an addon key (desktop, main settings, etc)
            /// </summary>
            public string NonAddonFileKey { get; set; }

            public string Key { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public object Default { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }

            public object[] Options { get; set; }

            public string DerivedModuleKey {
                get {
                    if (Addon != null)
                        return "addon_" + Addon.DerivedKey;
                    else
                        return NonAddonFileKey;
                }
            }

            public object GetValue()
            {
                if (Addon != null && Key == "enabled")
                    return Addon.Enabled;
                else
                    return CastValue(GetObject(DerivedModuleKey + "." + Key, Default));
            }

            public void SetValue(object value)
            {
                if (Addon != null && Key == "enabled")
                    Addon.Enabled = bool.Parse(value.ToString());
                else
                    Set(DerivedModuleKey + "." + Key, CastValue(value));
            }

            public object CastValue(object value)
            {
                switch(Type.ToLower())
                {
                    case "string":
                        return value.ToString();

                    case "double":
                    case "float":
                        return double.Parse(value.ToString());

                    case "int":
                    case "integer":
                    case "number":
                        return int.Parse(value.ToString());
                }

                return value;
            }
        }
    }
}
