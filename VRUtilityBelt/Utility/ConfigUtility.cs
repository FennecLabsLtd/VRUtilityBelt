using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace VRUB.Utility
{
    class ConfigUtility
    {
        static Dictionary<string, Dictionary<string, string>> configValues = new Dictionary<string, Dictionary<string, string>>();

        public delegate void UpdateHandler(string key, string value);

        public static void Load() {
            if (!Directory.Exists(PathUtilities.Constants.ConfigPath))
                Directory.CreateDirectory(PathUtilities.Constants.ConfigPath);

            foreach(string file in Directory.EnumerateFiles(PathUtilities.Constants.ConfigPath)) {
                if (Path.GetFileName(file) == "permissions.json")
                    continue;

                string moduleKey = Path.GetFileNameWithoutExtension(file);

                try
                {
                    configValues.Add(moduleKey, JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file)));
                } catch(JsonReaderException e)
                {
                    Logger.Error("[CONFIG] Failed to read " + file + ": " + e.Message);
                }
            }
        }

        public Dictionary<string, string> GetModuleConfig(string key)
        {
            if (configValues.ContainsKey(key))
                return configValues[key];
            else
                return null;
        }

        public static string Get(string dotPath, string defaultValue = null)
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

        public static void Set(string dotPath, string value)
        {
            string[] splitPath = dotPath.Split(new char[] { '.' }, 2);

            if(splitPath.Length == 2)
            {
                if (!configValues.ContainsKey(splitPath[0]))
                    configValues.Add(splitPath[0], new Dictionary<string, string>());

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
            foreach(KeyValuePair<string, Dictionary<string,string>> keyVal in configValues) {
                File.WriteAllText(Path.Combine(PathUtilities.Constants.ConfigPath, keyVal.Key + ".json"), JsonConvert.SerializeObject(keyVal.Value));
            }
        }

        public static void SetDefaults(string key, Dictionary<string,string> values)
        {
            if (!configValues.ContainsKey(key)) {
                configValues.Add(key, values);
                Save();
                return;
            }

            foreach(KeyValuePair<string,string> cfgVal in configValues[key])
            {
                if (!configValues.ContainsKey(cfgVal.Key))
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

        public static void Send(string message, string key, string value)
        {
            if (listeners[message] is UpdateHandler actions)
            {
                actions(key, value);
            }
        }

        private static Hashtable listeners = new Hashtable();
    }
}
