using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Addons.Permissions
{
    public class RequiresPermissionAttribute : Attribute
    {
        string _verb;
        string _permission;

        public string PermissionKey { get { return _permission; } }
        public string Verb { get { return _verb; } }

        public RequiresPermissionAttribute(string perm, string verb)
        {
            _permission = perm;
            _verb = verb;
        }
    }
}
