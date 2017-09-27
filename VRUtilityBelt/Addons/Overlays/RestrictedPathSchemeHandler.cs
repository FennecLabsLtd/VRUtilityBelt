using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Data;
using VRUB.Utility;

namespace VRUB.Addons.Overlays
{
    class RestrictedPathSchemeHandler : ISchemeHandlerFactory
    {
        string _schemeName;

        Overlay InternalOverlay { get; set; }

        string _basePath;

        public RestrictedPathSchemeHandler(string scheme, string basePath)
        {
            _basePath = basePath;
            _schemeName = scheme;
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            if(_basePath == null)
            {
                return Error("This scheme handler has a base path of null and cannot continue processing " + _schemeName + ":// requests", HttpStatusCode.NotImplemented);
            }

            if (_schemeName != null && !schemeName.Equals(_schemeName, StringComparison.OrdinalIgnoreCase))
            {
                return Error(string.Format("SchemeName {0} does not match the expected SchemeName of {1}.", schemeName, _schemeName), HttpStatusCode.NotFound);
            }

            Logger.Trace("Got request with " + _schemeName + ":// scheme: " + request.Url); 

            Uri uri = new Uri(request.Url);

            string absolutePath = uri.Host + "/" + uri.AbsolutePath.Substring(1);

            if (string.IsNullOrEmpty(absolutePath))
                return Error("Empty Path", HttpStatusCode.NotFound);


            string filePath = PathUtilities.GetTruePath(_basePath, absolutePath);
             
            if(filePath.EndsWith("/") || filePath.EndsWith("\\"))
            {
                filePath = filePath.Substring(0, filePath.Length - 1);
            }

            if (PathUtilities.IsInFolder(_basePath, filePath))
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

            Logger.Error("[OVERLAY] Load Error for addon:// scheme: " + error);

            return resourceHandler;
        }
    }
}