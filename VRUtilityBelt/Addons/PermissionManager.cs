using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Utility;

namespace VRUB.Addons
{
    public class PermissionManager
    {
        Dictionary<string, List<string>> grantedPermissions = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> declinedPermissions = new Dictionary<string, List<string>>();

        public void CheckPermissionAndPrompt(string appKey, string permissionKey, string reason, Action<bool> OnResult)
        {
            if(HasPermission(appKey, permissionKey))
            {
                OnResult(true);
            } else if(DeclinedPermission(appKey, permissionKey))
            {
                OpenVRTools.ShowAsyncModal(appKey + " is requesting permission to use " + permissionKey + ", provided reason: \n" + reason, "Permission Request", "Accept", "Decline", "Ask Later", null, (response) =>
                {
                    switch(response)
                    {
                        case Valve.VR.VRMessageOverlayResponse.ButtonPress_0:
                            GrantPermission(appKey, permissionKey, true);
                            OnResult(true);
                            break;

                        case Valve.VR.VRMessageOverlayResponse.ButtonPress_1:
                            GrantPermission(appKey, permissionKey, false);
                            OnResult(false);
                            break;

                        default:
                            return;
                    }
                });
            } else
            {
                OnResult(false);
            }
        }

        void GrantPermission(string appKey, string permissionKey, bool grantOrDecline)
        {
            Dictionary<string, List<string>> targetDictionary = grantOrDecline ? grantedPermissions : declinedPermissions;

            if (!targetDictionary.ContainsKey(appKey))
                targetDictionary.Add(appKey, new List<string>());

            if (targetDictionary[appKey] == null)
                targetDictionary[appKey] = new List<string>();

            if (targetDictionary[appKey].Contains(permissionKey))
                return;

            targetDictionary[appKey].Add(permissionKey);
        }

        bool HasPermission(string appKey, string permissionKey)
        {
            return grantedPermissions != null && grantedPermissions.ContainsKey(appKey) && grantedPermissions[appKey] != null && grantedPermissions[appKey].Contains(permissionKey);
        }

        bool DeclinedPermission(string appKey, string permissionKey)
        {
            return declinedPermissions != null && declinedPermissions.ContainsKey(appKey) && declinedPermissions[appKey] != null && declinedPermissions[appKey].Contains(permissionKey);
        } 
    }
}
