using CefSharp;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Plugins;
using VRUB.Utility;

namespace VRUB.Addons.Overlays
{
    class PluginSchemeHandler : ISchemeHandlerFactory
    {
        Overlay InternalOverlay { get; set; }

        public PluginSchemeHandler()
        {
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            Uri uri = new Uri(request.Url);

            if (!PluginManager.Plugins.ContainsKey(uri.Host.ToLower()))
                return Error("Could not find plugin matching " + uri.Host, HttpStatusCode.NotFound);

            string basePath = PluginManager.Plugins[uri.Host].BasePath;

            if (basePath == null)
            {
                return Error("This scheme handler has a base path of null and cannot continue processing plugin:// requests", HttpStatusCode.NotImplemented);
            }

            if (!schemeName.Equals("plugin", StringComparison.OrdinalIgnoreCase))
            {
                return Error(string.Format("SchemeName {0} does not match the expected SchemeName of {1}.", schemeName, "plugin"), HttpStatusCode.NotFound);
            }

            Logger.Trace("Got request with plugin:// scheme: " + request.Url);

            string absolutePath = uri.AbsolutePath.Substring(1);

            if (string.IsNullOrEmpty(absolutePath))
                return Error("Empty Path", HttpStatusCode.NotFound);


            string filePath = PathUtilities.GetTruePath(basePath, absolutePath);

            if (filePath.EndsWith("/") || filePath.EndsWith("\\"))
            {
                filePath = filePath.Substring(0, filePath.Length - 1);
            }

            if (PathUtilities.IsInFolder(basePath, filePath))
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

            Logger.Error("[OVERLAY] Load Error for plugin:// scheme: " + error);

            return resourceHandler;
        }
    }
}
