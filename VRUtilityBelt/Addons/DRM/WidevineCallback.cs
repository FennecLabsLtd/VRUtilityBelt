using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using VRUB.Utility;

namespace VRUB.Addons.DRM
{
    class WidevineCallback : IRegisterCdmCallback
    {
        public void Dispose()
        {
            Logger.Trace("[WIDEVINE] Tried to dispose of callback.");
        }

        public void OnRegistrationComplete(CdmRegistration registration)
        {
            if(registration.ErrorCode == CdmRegistrationErrorCode.None)
            {
                Logger.Info("[WIDEVINE] Widevine CDM Registered successfully.");
            } else
            {
                Logger.Error("[WIDEVINE] Failed to register Widevine CDM with error code " + registration.ErrorCode + " - " + registration.ErrorMessage);
            }
        }
    }
}
