using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUB.Utility;
using OpenTK.Graphics.OpenGL;
using Valve.VR;
using DesktopDuplication;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using TCD.System.TouchInjection;

namespace VRUtilityBelt.Desktop
{
    class DesktopMirror
    {
        Overlay InternalOverlay;

        int _screenIndex;
        Screen _screenObject;
        int _glTextureId;

        Texture_t _textureData;

        DesktopDuplicator _desktopDuplicator;
        DesktopFrame _latestFrame;
        VREvent_t eventData;
        uint eventSize = (uint)Marshal.SizeOf(typeof(VREvent_t));
        uint cStateSize = (uint)Marshal.SizeOf(typeof(VRControllerState_t));

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        PointerTouchInfo touch;
        bool isTouching = false;

        public DesktopMirror(int screenIndex, Screen screenObject)
        {
            _screenIndex = screenIndex;
            _screenObject = screenObject;

            Logger.Debug("[DESKTOP] Found Screen: " + screenObject.DeviceName + " with bounds of " + screenObject.Bounds.ToString() + " and working area " + screenObject.WorkingArea.ToString());

            Setup();
        }

        void Setup()
        {
            InternalOverlay = new Overlay("vrub.desktop." + _screenIndex, "Desktop", 3.5f, _screenIndex != 0);
            InternalOverlay.ToggleInput(true);
            InternalOverlay.SetTextureSize(_screenObject.WorkingArea.Width, _screenObject.WorkingArea.Height);

            if(_screenIndex > 0)
            {

            }

            _glTextureId = GL.GenTexture();
            _textureData = new Texture_t();
            _textureData.eColorSpace = EColorSpace.Linear;
            _textureData.eType = ETextureType.OpenGL;
            _textureData.handle = (IntPtr)_glTextureId;

            _desktopDuplicator = new DesktopDuplicator(_screenIndex);

            Logger.Debug("[DESKTOP] Display " + _screenIndex + " setup complete");
        }

        public void Update()
        {
            while (OpenVR.Overlay.PollNextOverlayEvent(InternalOverlay.Handle, ref eventData, eventSize))
            {
                EVREventType type = (EVREventType)eventData.eventType;

                switch(type)
                {
                    case EVREventType.VREvent_MouseButtonDown:
                        HandleMouseDown();
                        break;

                    case EVREventType.VREvent_InputFocusReleased:
                        HandleInputLost();
                        break;

                    case EVREventType.VREvent_MouseButtonUp:
                        HandleMouseUp();
                        break;

                    case EVREventType.VREvent_MouseMove:
                        HandleMouseMove();
                        break;
                }
            }
        }

        public void Draw()
        {
            UpdateTexture();
            ApplyTexture();
        }

        void UpdateTexture()
        {
            try
            {
                _latestFrame = _desktopDuplicator.GetLatestFrame();
            } catch
            {
                return; // Do nothing, it'll probably just spam the console.
            }

            if(_latestFrame != null && _latestFrame.DesktopImage != null)
            {
                lock (_latestFrame.DesktopImage) {
                    BitmapData bmpData = _latestFrame.DesktopImage.LockBits(
                        new Rectangle(0, 0, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb
                        );

                    GL.BindTexture(TextureTarget.Texture2D, _glTextureId);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    _latestFrame.DesktopImage.UnlockBits(bmpData);

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
            }
        }

        void ApplyTexture()
        {
            if(_latestFrame != null)
            {
                InternalOverlay.SetTexture(ref _textureData);
            }
        }

        void HandleInputLost()
        {
            ReleaseTouch();
        }

        void HandleMouseMove()
        {
            if (!isTouching)
                return;

            touch.PointerInfo.PtPixelLocation.X = _screenObject.Bounds.Left + (int)eventData.data.mouse.x;
            touch.PointerInfo.PtPixelLocation.Y = _screenObject.Bounds.Bottom - (int)eventData.data.mouse.y;
            touch.PointerInfo.PointerFlags = PointerFlags.UPDATE | PointerFlags.INCONTACT | PointerFlags.INRANGE;

            VRControllerState_t cState = new VRControllerState_t();
            OpenVR.System.GetControllerState(OpenVR.Overlay.GetPrimaryDashboardDevice(), ref cState, cStateSize);

            if (!TouchInjector.InjectTouchInput(1, new PointerTouchInfo[] { touch }))
            {
                Logger.Warning("[DESKTOP] Failed to inject touch move: " + GetLastError());
            }
        }

        void HandleMouseDown()
        {
            if(isTouching)
            {
                ReleaseTouch();
            }

            VRControllerState_t cState = new VRControllerState_t();
            OpenVR.System.GetControllerState(OpenVR.Overlay.GetPrimaryDashboardDevice(), ref cState, cStateSize);

            touch = new PointerTouchInfo();
            touch.PointerInfo.pointerType = PointerInputType.TOUCH;
            touch.TouchFlags = TouchFlags.NONE;
            touch.Orientation = 0;
            touch.Pressure = 32000;//(uint)(cState.rAxis3.x * 2048);
            touch.PointerInfo.PointerFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;
            touch.TouchMasks = TouchMask.PRESSURE | TouchMask.CONTACTAREA;

            touch.PointerInfo.PtPixelLocation.X = _screenObject.Bounds.Left + (int)eventData.data.mouse.x;
            touch.PointerInfo.PtPixelLocation.Y = _screenObject.Bounds.Bottom - (int)eventData.data.mouse.y;

            touch.ContactArea.top = touch.PointerInfo.PtPixelLocation.Y - 5;
            touch.ContactArea.bottom = touch.PointerInfo.PtPixelLocation.Y + 5;

            touch.ContactArea.left = touch.PointerInfo.PtPixelLocation.X - 5;
            touch.ContactArea.right = touch.PointerInfo.PtPixelLocation.X + 5;

            touch.PointerInfo.PointerId = 10;

            if(!TouchInjector.InjectTouchInput(1, new PointerTouchInfo[] { touch }))
            {
                Logger.Warning("[DESKTOP] Failed to inject touch down: " + GetLastError());
            } else
            {
                isTouching = true;
            }
        }

        void HandleMouseUp()
        {
            if (isTouching)
                ReleaseTouch();
        }

        void ReleaseTouch()
        {
            touch.PointerInfo.PointerFlags = PointerFlags.UP;
            if(!TouchInjector.InjectTouchInput(1, new PointerTouchInfo[] { touch }))
            {
                Logger.Warning("[DESKTOP] Failed to inject touch up: " + GetLastError());
            } else
                isTouching = false;
        }
    }
}
