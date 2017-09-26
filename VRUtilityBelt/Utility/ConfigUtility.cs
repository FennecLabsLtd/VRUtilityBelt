using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace VRUB.Utility
{
    class ConfigUtility
    {
        static Dictionary<string, Dictionary<string, string>> configValues = new Dictionary<string, Dictionary<string, string>>();

        public static void Load() {
            if (!Directory.Exists(PathUtilities.Constants.ConfigPath))
                Directory.CreateDirectory(PathUtilities.Constants.ConfigPath);

            foreach(string file in Directory.EnumerateFiles(PathUtilities.Constants.ConfigPath)) {
                string moduleKey = Path.GetFileNameWithoutExtension(file);

                configValues.Add(moduleKey, JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file)));
            }
        }

        public Dictionary<string, string> GetModuleConfig(string key)
        {
            if (configValues.ContainsKey(key))
                return configValues[key];
            else
                return null;
        }

        public static string Get(string dotPath)
        {
            string[] splitPath = dotPath.Split( new char[] { '.' }, 2);

            if(splitPath.Length == 2)
            {
                if (configValues.ContainsKey(splitPath[0])) {
                    if (configValues[splitPath[0]].ContainsKey(splitPath[1]))
                        return configValues[splitPath[0]][splitPath[1]];
                    else
                        throw new KeyNotFoundException("Invalid Config Key " + splitPath[1]);
                }
                else
                {
                    throw new KeyNotFoundException("Invalid File Key " + splitPath[0]);
                }
            } else
            {
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
    }
}
