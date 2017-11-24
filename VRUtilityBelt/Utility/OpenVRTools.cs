using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace VRUB.Utility
{
    public class OpenVRTools
    {
        public static async void ShowAsyncModal(string text, string caption, string button0Text, string button1Text, string button2Text, string button3Text, Action<VRMessageOverlayResponse> callback) {
            VRMessageOverlayResponse result = await Task.Run(() => OpenVR.Overlay.ShowMessageOverlay(text, caption, button0Text, button1Text, button2Text, button3Text));

            callback(result);
        }

        public static void OnePixelTexture(out Texture_t texture, out int glTextureId, int Width, int Height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            glTextureId = GL.GenTexture();
            texture = new Texture_t();
            texture.eColorSpace = EColorSpace.Linear;
            texture.eType = ETextureType.OpenGL;
            texture.handle = (IntPtr)glTextureId;

            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Transparent);
            bmp = new Bitmap(bmp, new Size(Width, Height)); // We do this to stretch it out.

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            GL.BindTexture(TextureTarget.Texture2D, glTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            bmp.UnlockBits(bmpData);
        }
    }
}
