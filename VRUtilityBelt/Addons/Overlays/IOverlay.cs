using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.Addons.Overlays
{
    public interface IOverlay
    {
        void Update();
        void Draw();
        void Destroy();
    }
}
