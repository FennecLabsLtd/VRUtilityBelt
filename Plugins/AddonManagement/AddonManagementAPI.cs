using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.Addons.Plugins;

namespace AddonManagement
{
    class AddonManagementAPI
    {
        static AddonManagementAPI _instance;
        public static AddonManagementAPI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AddonManagementAPI();

                return _instance;
            }
        }

        public List<Dictionary<string,object>> GetAddons()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            foreach(Addon addon in AddonManager.Instance.Addons)
            {
                Dictionary<string, object> a = new Dictionary<string, object>();

                a.Add("key", addon.DerivedKey);
                a.Add("name", addon.Name);
                a.Add("description", addon.Description);
                a.Add("author", addon.Author);
                a.Add("enabled", addon.Enabled);
                a.Add("source", addon.Source.ToString());
                a.Add("sudo", addon.SudoAccess);

                List<Dictionary<string, object>> overlays = new List<Dictionary<string, object>>();

                foreach(Overlay o in addon.Overlays)
                {
                    overlays.Add(new Dictionary<string, object>()
                    {
                        { "key", o.Key },
                        { "name", o.Name },
                        { "description", o.Description },
                    });
                }

                a.Add("overlays", overlays);

                List<Dictionary<string, object>> plugins = new List<Dictionary<string, object>>();

                foreach(PluginContainer p in addon.Plugins)
                {
                    plugins.Add(new Dictionary<string, object>()
                    {
                        { "key", p.Key },
                        { "name", p.Name },
                        { "description", p.Description },
                    });
                }

                result.Add(a);
            }

            return result;
        }

        public void ToggleAddon(string addonKey, bool toggle)
        {
            Addon addon = AddonManager.Instance.GetAddon(addonKey);

            if (addon == null)
                throw new NullReferenceException("Invalid Addon Key.");
            else
                addon.Enabled = toggle;
        }
    }
}
