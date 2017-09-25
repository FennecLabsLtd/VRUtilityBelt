using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace VRUtilityBelt.Utility
{
    public class OpenVRTools
    {
        public static async void ShowAsyncModal(string text, string caption, string button0Text, string button1Text, string button2Text, string button3Text, Action<VRMessageOverlayResponse> callback) {
            VRMessageOverlayResponse result = await Task.Run(() => OpenVR.Overlay.ShowMessageOverlay(text, caption, button0Text, button1Text, button2Text, button3Text));

            callback(result);
        }
    }
}
