using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Bridge.Attributes
{
    class BridgeMethod : Attribute
    {
        string _methodName;
        public string MethodName { get { return _methodName; } }

        public BridgeMethod(string methodName)
        {
            _methodName = methodName;
        }
    }
}
