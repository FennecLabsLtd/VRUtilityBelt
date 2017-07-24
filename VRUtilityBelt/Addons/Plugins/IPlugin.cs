using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.Addons.Plugins
{
    public interface IPlugin
    {
        void Update();
        void Draw();
    }
}
