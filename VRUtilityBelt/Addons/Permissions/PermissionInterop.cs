using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;

namespace VRUB.Addons.Permissions
{
    public class PermissionInterop
    {
        Addon _addon;
        public PermissionInterop(Addon addon)
        {
            _addon = addon;
        }
        public bool HasPermission(string key)
        {
            return PermissionManager.HasPermission(_addon.DerivedKey, key);
        }

        public void RequestPermission(string key, string reason)
        {
            PermissionManager.CheckPermissionAndPrompt(_addon.DerivedKey, key, reason, null);
        }
    }
}
