using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRUtilityBelt.Addons.Data;
using VRUtilityBelt.Utility;

namespace VRUtilityBelt.Addons.Overlays
{
    class OverlaySchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "addon";

        Addon Addon { get; set; }
        BasicOverlay Overlay { get; set; }

        public OverlaySchemeHandlerFactory(Addon owner)
        {
            Addon = owner;
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            if(Addon == null)
            {
                return Error("This browser is not attached to an addon and therefore cannot process addon:// requests", HttpStatusCode.NotImplemented);
            }

            if (SchemeName != null && !schemeName.Equals(SchemeName, StringComparison.OrdinalIgnoreCase))
            {
                return Error(string.Format("SchemeName {0} does not match the expected SchemeName of {1}.", schemeName, SchemeName), HttpStatusCode.NotFound);
            }

            SteamVR_WebKit.SteamVR_WebKit.Log("Got request with addon:// scheme: " + request.Url); 

            Uri uri = new Uri(request.Url);

            string absolutePath = uri.Host + "/" + uri.AbsolutePath.Substring(1);

            if (string.IsNullOrEmpty(absolutePath))
                return Error("Empty Path", HttpStatusCode.NotFound);


            string filePath = PathUtilities.GetTruePath(Addon.BasePath, absolutePath);
             
            if(filePath.EndsWith("/") || filePath.EndsWith("\\"))
            {
                filePath = filePath.Substring(0, filePath.Length - 1);
            }

            if (PathUtilities.IsInFolder(Addon.BasePath, filePath))
            {
                string ext = Path.GetExtension(filePath);
                string mime = ResourceHandler.GetMimeType(ext);
                var stream = File.OpenRead(filePath);
                return ResourceHandler.FromStream(stream, mime);
            }

            return Error(String.Format("File not found: {0}", filePath), HttpStatusCode.NotFound);
        }

        IResourceHandler Error(string error, HttpStatusCode code)
        {
            var stream = ResourceHandler.GetMemoryStream(error, Encoding.UTF8);
            var resourceHandler = ResourceHandler.FromStream(stream);
            resourceHandler.StatusCode = (int)code;

            Console.WriteLine("[OVERLAY] Load Error for addon:// scheme: " + error);

            return resourceHandler;
        }
    }
}