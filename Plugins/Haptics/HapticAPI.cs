using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace Haptics
{
    class HapticAPI
    {
        static HapticAPI _instance;
        public static HapticAPI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HapticAPI();

                return _instance;
            }
        }

        public void Trigger()
        {
            TriggerOnPointingDevice(0, 200, 5, 100);
        }

        // We use long purely because C# reflection does implicitly cast...
        public void TriggerOnPointingDevice(long axisId, long strength, long msInterval, long msDuration)
        {
            TriggerOnDevice((long)OpenVR.Overlay.GetPrimaryDashboardDevice(), axisId, strength, msInterval, msDuration);
        }

        public void TriggerOnDevice(long controllerIndex, long axisId, long strength, long msInterval, long msDuration)
        {
            //OpenVR.System.TriggerHapticPulse((uint)controllerIndex, (uint)axisId, (char)strength);

            Plugin.AddPulse(new HapticPulse((uint)controllerIndex, (uint)axisId, strength / 3999f, (uint)msInterval, (uint)msDuration));
        }
    }
}
