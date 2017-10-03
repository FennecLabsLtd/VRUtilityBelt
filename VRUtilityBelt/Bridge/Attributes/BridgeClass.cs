using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Bridge.Attributes
{
    class BridgeClass
    {
        string _className;
        public string ClassName { get { return _className; } }

        public BridgeClass(string className)
        {
            _className = className;
        }
    }
}
