using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Bridge.Attributes
{
    class MethodName : Attribute
    {
        public readonly string Name;

        public MethodName(string methodName)
        {
            Name = methodName;
        }
    }
}
