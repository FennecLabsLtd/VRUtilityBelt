using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;

namespace Config
{
    public class ConfigAPI
    {
        Addon _addon;

        public ConfigAPI(Addon addon)
        {
            _addon = addon;
        }

        public Dictionary<string, object> FetchConfig()
        {
            return VRUB.Utility.ConfigUtility.GetModuleConfig("addon_" + _addon.DerivedKey);
        }

        public object FetchValue(string key, object defaultValue)
        {
            try
            {
                return VRUB.Utility.ConfigUtility.GetObject("addon_" + _addon.DerivedKey + "." + key, defaultValue);
            } catch(KeyNotFoundException e)
            {
                return null;
            }
        }

        public void Set(string key, object value)
        {
            VRUB.Utility.ConfigUtility.Set("addon_" + _addon.DerivedKey + "." + key, value);
        }
    }
}
