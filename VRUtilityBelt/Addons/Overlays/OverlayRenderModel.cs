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
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

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

        static bool _generatedOnePixelTexture = false;
        static Texture_t _onePixelTexture;
        static int _glOnePixelTextureId;

        public void Setup(Overlay parent)
        {
            _parent = parent;
            _internalOverlay = new InternalOverlay("vrub." + _parent.DerivedKey + ".models." + Key, "", Meters, true);
            _internalOverlay.SetAttachment(AttachmentType.Overlay, Position, Rotation, "vrub." + _parent.DerivedKey);
            _internalOverlay.SetTextureSize(Width, Height);

            if (!_generatedOnePixelTexture)
                GenerateOnePixelTexture();

            UpdateModel();
            _internalOverlay.Show();
        }

        static void GenerateOnePixelTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            _glOnePixelTextureId = GL.GenTexture();
            _onePixelTexture = new Texture_t();
            _onePixelTexture.eColorSpace = EColorSpace.Linear;
            _onePixelTexture.eType = ETextureType.OpenGL;
            _onePixelTexture.handle = (IntPtr)_glOnePixelTextureId;

            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Transparent);

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, 1, 1),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            GL.BindTexture(TextureTarget.Texture2D, _glOnePixelTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            bmp.UnlockBits(bmpData);

            _generatedOnePixelTexture = true;
        }

        void UpdateModel()
        {
            HmdColor_t col = new HmdColor_t();
            col.a = Opacity;
            col.r = 1;
            col.g = 1;
            col.b = 1;

            string ModelFilePath = Path.IsPathRooted(ModelFile) ? ModelFile : PathUtilities.GetTruePath(_parent.BasePath, ModelFile);

            EVROverlayError err = OpenVR.Overlay.SetOverlayRenderModel(_internalOverlay.Handle, ModelFilePath, ref col);

            if(err != EVROverlayError.None)
            {
                Logger.Error("[RENDERMODEL] Failed to set overlay render model: " + err.ToString());
            } else
            {
                err = OpenVR.Overlay.SetOverlayTexture(_internalOverlay.Handle, ref _onePixelTexture);

                if (err != EVROverlayError.None)
                    Logger.Error("[RENDERMODEL] Failed to set overlay render model stub texture: " + err.ToString());
                else
                    Logger.Info("[RENDERMODEL] Set render model " + ModelFilePath + " and stub texture for " + _parent.DerivedKey + ".models." + Key);
            }
        }

        public void Destroy()
        {
            _internalOverlay.Destroy();
        }
    }
}
