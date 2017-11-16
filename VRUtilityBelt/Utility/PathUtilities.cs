using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Utility
{
    static class PathUtilities
    {
        internal class Constants {
            public static readonly string BaseRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VRUtilityBelt");
            public static readonly string BaseLocalAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VRUtilityBelt");
            public static readonly string GlobalCookiePath = Path.Combine(BaseRoamingPath, "Global\\Cookies");
            public static readonly string GlobalCachePath = Path.Combine(BaseLocalAppDataPath, "Global\\Cache");

            public static readonly string ConfigPath = Path.Combine(BaseRoamingPath, "Config");

            public static readonly string GlobalStaticResourcesPath = Path.Combine(Environment.CurrentDirectory, "static");

            public static readonly string GlobalShadersPath = Path.Combine(Environment.CurrentDirectory, "shaders");

            public static readonly string AddonPath = Path.Combine(Environment.CurrentDirectory, "addons");
        };

        public static bool IsInFolder(string basePath, string filePath)
        {
            return filePath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase) && File.Exists(filePath);
        }

        public static string GetTruePath(string basePath, string filePath)
        {
            return Path.GetFullPath(Path.Combine(basePath, filePath));
        }
    }
}
