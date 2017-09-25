using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.Utility
{
    static class PathUtilities
    {
        internal class PathConstants {
            public readonly string BaseCookiePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VRUtilityBelt\\Cookies";
        };

        public static PathConstants Constants;

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
