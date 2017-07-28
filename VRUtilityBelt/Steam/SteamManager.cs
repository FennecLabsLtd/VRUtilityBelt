using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.Steam
{
    public class SteamManager
    {
        public static bool Initialised { get; set; }
        public static bool Init()
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary((AppId_t)645370))
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

            return Initialised;
        }

        private static void SteamAPIDebugTextHook(int severity, StringBuilder text)
        {
            Console.WriteLine("Steam API Error: " + text);
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
        }
    }
}
