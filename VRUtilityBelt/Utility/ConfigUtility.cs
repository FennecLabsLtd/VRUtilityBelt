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
    class ConfigUtility
    {
        static Dictionary<string, Dictionary<string, object>> configValues = new Dictionary<string, Dictionary<string, object>>();
        static Dictionary<Addon, List<ConfigLayout>> addonConfigLayouts = new Dictionary<Addon, List<ConfigLayout>>();

        public delegate void UpdateHandler(string key, object value);

        public static void Load() {
            if (!Directory.Exists(PathUtilities.Constants.ConfigPath))
                Directory.CreateDirectory(PathUtilities.Constants.ConfigPath);

            foreach(string file in Directory.EnumerateFiles(PathUtilities.Constants.ConfigPath)) {
                if (Path.GetFileName(file) == "permissions.json")
                    continue;

                string moduleKey = Path.GetFileNameWithoutExtension(file);

                try
                {
                    configValues.Add(moduleKey, JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file)));
                } catch(JsonReaderException e)
                {
                    Logger.Error("[CONFIG] Failed to read " + file + ": " + e.Message);
                }
            }
        }

        public Dictionary<string, object> GetModuleConfig(string key)
        {
            if (configValues.ContainsKey(key))
                return configValues[key];
            else
                return null;
        }

        public static object GetObject(string dotPath, object defaultValue = null)
        {
            string[] splitPath = dotPath.Split( new char[] { '.' }, 2);

            if(splitPath.Length == 2)
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
            } else if(returnType == typeof(bool))
            {
                bool output;
                if(bool.TryParse(value.ToString(), out output))
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

            if(splitPath.Length == 2)
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
            foreach(KeyValuePair<string, Dictionary<string,object>> keyVal in configValues) {
                File.WriteAllText(Path.Combine(PathUtilities.Constants.ConfigPath, keyVal.Key + ".json"), JsonConvert.SerializeObject(keyVal.Value));
            }
        }

        public static void SetDefaults(string key, Dictionary<string,object> values)
        {
            if (!configValues.ContainsKey(key)) {
                configValues.Add(key, values);
                Save();
                return;
            }

            foreach(KeyValuePair<string,object> cfgVal in configValues[key])
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
            if (addonConfigLayouts.ContainsKey(addon))
                addonConfigLayouts.Remove(addon);

            if(File.Exists(Path.Combine(addon.BasePath, "config.json")))
            {
                List<ConfigLayout> configSettings = JsonConvert.DeserializeObject<List<ConfigLayout>>(File.ReadAllText(Path.Combine(addon.BasePath, "config.json")));

                foreach(ConfigLayout layout in configSettings)
                {
                    layout.Addon = addon;
                }

                addonConfigLayouts.Add(addon, configSettings);

                Dictionary<string, object> defaults = new Dictionary<string, object>();

                foreach(ConfigLayout layout in configSettings)
                {
                    defaults[layout.Key] = layout.Default;
                }

                SetDefaults("addon." + addon.DerivedKey, defaults);
            }
        }

        public static Dictionary<Addon, List<ConfigLayout>> GetLayouts()
        {
            return addonConfigLayouts;
        }

        private static Hashtable listeners = new Hashtable();

        public class ConfigLayout
        {
            public Addon Addon { get; set; }
            public string Key { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public object Default { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
         
            public object GetValue()
            {
                return GetObject("addon." + Addon.DerivedKey + "." + Key, Default);
            }

            public void SetValue(object value)
            {
                Set("addon." + Addon.DerivedKey + "." + Key, value);
            }
        }
    }
}
