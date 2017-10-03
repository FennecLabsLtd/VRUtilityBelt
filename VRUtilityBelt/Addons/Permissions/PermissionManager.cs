using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Utility;

namespace VRUB.Addons.Permissions
{
    public class PermissionManager
    {
        static Dictionary<string, List<string>> grantedPermissions = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> declinedPermissions = new Dictionary<string, List<string>>();

        class PermissionJSON
        {
            public Dictionary<string, List<string>> GrantedPermissions { get; set; }
            public Dictionary<string, List<string>> DeclinedPermissions { get; set; }
        }

        public static void Load()
        {
            if (!File.Exists(Path.Combine(PathUtilities.Constants.ConfigPath, "permissions.json")))
                return;

            string json = File.ReadAllText(Path.Combine(PathUtilities.Constants.ConfigPath, "permissions.json"));

            PermissionJSON decoded = JsonConvert.DeserializeObject<PermissionJSON>(json);

            grantedPermissions = decoded.GrantedPermissions;
            declinedPermissions = decoded.DeclinedPermissions;
        }

        public static void CheckPermissionAndPrompt(Addon addon, string permissionKey, string verb, Action<bool> OnResult = null)
        {
            string addonKey = addon.DerivedKey;

            if(addon.SudoAccess || HasPermission(addonKey, permissionKey))
            {
                OnResult?.Invoke(true);
            } else if(!DeclinedPermission(addonKey, permissionKey))
            {
                OpenVRTools.ShowAsyncModal(addon.Name + " is requesting permission to " + verb  + "\n\nProvided reason: \"" +  addon.GetPermissionReasoning(permissionKey) + "\"", "Permission Request", "Accept", "Decline", "Ask Later", null, (response) =>
                {
                    switch(response)
                    {
                        case Valve.VR.VRMessageOverlayResponse.ButtonPress_0:
                            GrantPermission(addonKey, permissionKey, true);

                            OnResult?.Invoke(true);

                            break;

                        case Valve.VR.VRMessageOverlayResponse.ButtonPress_1:
                            GrantPermission(addonKey, permissionKey, false);

                            OnResult?.Invoke(false);

                            break;

                        default:
                            return;
                    }
                });
            } else
            {
                OnResult?.Invoke(false);
            }
        }

        static void GrantPermission(string addonKey, string permissionKey, bool grantOrDecline)
        {
            Dictionary<string, List<string>> targetDictionary = grantOrDecline ? grantedPermissions : declinedPermissions;

            if (!targetDictionary.ContainsKey(addonKey))
                targetDictionary.Add(addonKey, new List<string>());

            if (targetDictionary[addonKey] == null)
                targetDictionary[addonKey] = new List<string>();

            if (targetDictionary[addonKey].Contains(permissionKey))
                return;

            targetDictionary[addonKey].Add(permissionKey);

            Addon addon = AddonManager.Instance.GetAddon(addonKey);

            if(addon != null)
            {
                addon.FireAtOverlays("permission", new { granted = grantOrDecline, key = permissionKey });
            }

            Save();
        }

        static void Save()
        {
            if (!Directory.Exists(PathUtilities.Constants.ConfigPath))
                Directory.CreateDirectory(PathUtilities.Constants.ConfigPath);

            File.WriteAllText(Path.Combine(PathUtilities.Constants.ConfigPath, "permissions.json"), JsonConvert.SerializeObject(new PermissionJSON() { GrantedPermissions = grantedPermissions, DeclinedPermissions = declinedPermissions }));
        }

        public static bool HasPermission(string addonKey, string permissionKey)
        {
            return grantedPermissions != null && grantedPermissions.ContainsKey(addonKey) && grantedPermissions[addonKey] != null && grantedPermissions[addonKey].Contains(permissionKey);
        }

        public static bool DeclinedPermission(string addonKey, string permissionKey)
        {
            return declinedPermissions != null && declinedPermissions.ContainsKey(addonKey) && declinedPermissions[addonKey] != null && declinedPermissions[addonKey].Contains(permissionKey);
        } 
    }
}
