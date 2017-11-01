using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Valve.VR;

namespace Haptics
{
    public class HapticPulse
    {
        Stopwatch intervalTimer;
        Stopwatch totalTimer;

        float _strength;
        uint _deviceId, _axis, _msInterval, _msDuration;

        public HapticPulse(uint deviceId, uint axis, float strength, uint msInterval, uint msDuration)
        {
            if (strength > 1f) strength = 1f;
            if (strength < 0f) strength = 0f;

            if (msInterval < 5)
                msInterval = 5;

            _deviceId = deviceId;
            _axis = axis;
            _strength = strength;
            _msInterval = msInterval;
            _msDuration = msDuration;

            intervalTimer = new Stopwatch();
            totalTimer = new Stopwatch();
            totalTimer.Start();

            Pulse();
        }

        /// <summary>
        /// Check if a pulse is necessary
        /// </summary>
        /// <returns>Returns false if the pulse has expired</returns>
        public bool CheckAndPulse()
        {
            if (totalTimer.ElapsedMilliseconds > _msDuration)
            {
                Stop();
                return false;
            }

            if(intervalTimer.ElapsedMilliseconds > _msInterval)
            {
                Pulse();
            }

            return true;
        }

        void Stop()
        {
            totalTimer.Stop();
            intervalTimer.Stop();
        }

        void Pulse()
        {
            VRUB.Utility.Logger.Trace("Pulse");
            OpenVR.System.TriggerHapticPulse(_deviceId, _axis, (char)Math.Floor((decimal)(3999 * _strength)));
            intervalTimer.Restart();
        }
    }
}
