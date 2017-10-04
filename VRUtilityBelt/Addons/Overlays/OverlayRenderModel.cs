using Newtonsoft.Json;
using OpenTK;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using VRUB.Utility;
using InternalOverlay = SteamVR_WebKit.Overlay;

namespace VRUB.Addons.Overlays
{
    public class OverlayRenderModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("model")]
        public string ModelFile { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; } = 1;

        [JsonProperty("height")]
        public int Height { get; set; } = 1;

        [JsonProperty("meters")]
        public float Meters { get; set; } = 2f;

        [JsonProperty("position")]
        public Vector3 Position { get; set; } = Vector3.Zero;

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [JsonProperty("opacity")]
        public float Opacity { get; set; } = 1;

        Overlay _parent;
        public Overlay Parent { get { return _parent; } }

        InternalOverlay _internalOverlay;

        public void Setup(Overlay parent)
        {
            _parent = parent;
            _internalOverlay = new InternalOverlay("vrub." + _parent.DerivedKey + ".models." + Key, "", Meters, true);
            _internalOverlay.SetAttachment(AttachmentType.Overlay, Position, Rotation, "vrub." + _parent.DerivedKey);
            _internalOverlay.SetTextureSize(Width, Height);

            UpdateModel();
            _internalOverlay.Show();
            Logger.Info("[RENDERMODEL] Setup Render Model " + _parent.DerivedKey + ".models." + Key);
        }

        void UpdateModel()
        {
            HmdColor_t col = new HmdColor_t();
            col.a = Opacity;
            col.r = 1;
            col.g = 1;
            col.b = 1;

            string ModelFilepath = Path.IsPathRooted(ModelFile) ? ModelFile : PathUtilities.GetTruePath(_parent.BasePath, ModelFile);
            OpenVR.Overlay.SetOverlayFromFile(_internalOverlay.Handle, ModelFile.Replace(".obj", ".png"));

            EVROverlayError err = OpenVR.Overlay.SetOverlayRenderModel(_internalOverlay.Handle, ModelFilepath, ref col);

            if(err != EVROverlayError.None)
            {
                Logger.Error("[RENDERMODEL] Failed to set overlay render model: " + err.ToString());
            }
        }

        public void Destroy()
        {
            _internalOverlay.Destroy();
        }
    }
}
