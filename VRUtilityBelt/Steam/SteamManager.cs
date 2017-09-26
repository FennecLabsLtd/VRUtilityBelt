using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Utility;

namespace VRUB.Steam
{
    public class SteamManager
    {
        public static bool Initialised { get; set; }
        public static readonly AppId_t AppID = (AppId_t)645370;
        public static bool Init()
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary(AppID))
                {
                    Environment.Exit(0);
                }
            } catch(Exception)
            {
                return false;
            }

            Initialised = SteamAPI.Init();

            if(Initialised)
            {
                SteamClient.SetWarningMessageHook(SteamAPIDebugTextHook);
            }

            Workshop.RegisterCallbacks();

            return Initialised;
        }

        private static void SteamAPIDebugTextHook(int severity, StringBuilder text)
        {
            Logger.Error("Steam API Error: " + text);
        }

        public static void RunCallbacks()
        {
            if(Initialised)
                SteamAPI.RunCallbacks();
        }

        public static void Shutdown()
        {
            if (Initialised)
                SteamAPI.Shutdown();

            Initialised = false;
        }
    }
}
