using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using System.Windows.Forms;
using VRUtilityBelt.Utility;

namespace VRUtilityBelt.JsInterop
{
    public class SteamAuth
    {
        Callback<GetAuthSessionTicketResponse_t> AuthSessionTicketCallback;
        HAuthTicket _ticketHandle;
        byte[] _ticketBytes;

        public SteamAuth()
        {
            AuthSessionTicketCallback = Callback<GetAuthSessionTicketResponse_t>.Create(OnAuthSessionTicket);
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if(_ticketHandle != null)
                SteamUser.CancelAuthTicket(_ticketHandle);
        }

        public string GetAuthSessionTicket()
        {
            if (_ticketHandle.m_HAuthTicket != 0)
                return BitConverter.ToString(_ticketBytes).Replace("-", string.Empty);

            _ticketBytes = new byte[1024];
            uint ticketSize = 0;
            _ticketHandle = SteamUser.GetAuthSessionTicket(_ticketBytes, 1024, out ticketSize);

            return BitConverter.ToString(_ticketBytes).Replace("-", string.Empty);
        }

        void OnAuthSessionTicket(GetAuthSessionTicketResponse_t response)
        {
            Logger.Trace("Got OnAuthSessionTicket: " + response.m_eResult.ToString());
        }
    }
}
