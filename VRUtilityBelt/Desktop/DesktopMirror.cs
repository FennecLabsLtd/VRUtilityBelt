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
using System.Threading;

namespace VRUB.Desktop
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

        static bool useTouch = false;

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        PointerTouchInfo touch;
        bool isTouching = false;

        float _width = 2.5f;
        float _distance = 2f;

        bool _didFirst = false;
        private readonly object _frameLock = new object();

        bool _requestNew = true;

        int touchYOffset = 0;

        Thread frameGetter;

        public DesktopMirror(int screenIndex, Screen screenObject)
        {
            _width = ConfigUtility.Get<float>("desktop.width");
            _distance = ConfigUtility.Get<float>("desktop.distance", 2f);

            ConfigUtility.Listen("desktop.width", DesktopWidthChanged);
            ConfigUtility.Listen("desktop.distance", DesktopDistanceChanged);
            ConfigUtility.Listen("desktop.position", DesktopPositionChanged);
            ConfigUtility.Listen("desktop.show_without_dashboard", DesktopAppearanceChanged);
            ConfigUtility.Listen("desktop.use_touch", (key, v) =>
            {
                useTouch = (bool)v;
            });

            useTouch = ConfigUtility.Get<bool>("desktop.use_touch", true);

            SteamVR_Event.Listen("DashboardActivated", (d) =>
            {
                if (_screenIndex > 0)
                    return;

                SetRootPosition();
                InternalOverlay.Show();
            });

            SteamVR_Event.Listen("DashboardDeactivated", (d) =>
            {
                if (_screenIndex > 0)
                    return;

                if (!ConfigUtility.Get<bool>("desktop.show_without_dashboard", false))
                    InternalOverlay.Hide();
            });

            _screenIndex = screenIndex;
            _screenObject = screenObject;

            Logger.Debug("[DESKTOP] Found Screen: " + screenObject.DeviceName + " with bounds of " + screenObject.Bounds.ToString() + " and working area " + screenObject.WorkingArea.ToString());

            Setup();
        }

        private void DesktopPositionChanged(string key, object value)
        {
            if (_screenIndex > 0)
            {
                return;
            }

            SetRootPosition();
        }

        private void DesktopAppearanceChanged(string key, object value)
        {
            if (_screenIndex > 0)
            {
                return;
            }

            bool showWithoutDashboard = (bool)value;

            UpdateScreen();

            if (showWithoutDashboard)
                InternalOverlay.Show();
            else if (!OpenVR.Overlay.IsDashboardVisible())
                InternalOverlay.Hide();
        }

        void DesktopWidthChanged(string key, object value)
        {
            _width = float.Parse(value.ToString());
            UpdateScreen();
        }

        void DesktopDistanceChanged(string key, object value)
        {
            _distance = float.Parse(value.ToString());
            UpdateScreen();
        }

        void Setup()
        {
            InternalOverlay = new Overlay("vrub.desktop." + _screenIndex, "Desktop " + _screenIndex, 2.5f, true, false);
            InternalOverlay.ToggleInput(true);
            InternalOverlay.SetTextureSize(_screenObject.Bounds.Width, _screenObject.Bounds.Height);

            UpdateScreen();

            _glTextureId = GL.GenTexture();
            ConfigureTexture();

            _textureData = new Texture_t();
            _textureData.eColorSpace = EColorSpace.Linear;
            _textureData.eType = ETextureType.OpenGL;
            _textureData.handle = (IntPtr)_glTextureId;

            _desktopDuplicator = new DesktopDuplicator(_screenIndex);

            InternalOverlay.Show();

            frameGetter = new Thread(new ThreadStart(GetFrame));
            frameGetter.Name = "Desktop " + _screenIndex + " Frame Getter";
            frameGetter.Start();

            Logger.Debug("[DESKTOP] Display " + _screenIndex + " setup complete");
        }

        void GetFrame()
        {
            while (true)
            {
                lock (_frameLock)
                {
                    if (!_requestNew)
                        continue;
                    try
                    {
                        _latestFrame = _desktopDuplicator.GetLatestFrame();
                        _requestNew = false;
                    }
                    catch
                    {
                        return; // Do nothing, it'll probably just spam the console.
                    }
                }

                Thread.Sleep(5);
            }
        }

        void ConfigureTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, _glTextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        void UpdateScreen()
        {
            if (_screenIndex == 0)
            {
                SetRootPosition();
            }
            else
            {
                SetPositionRelativeToPrimary();
            }
        }

        void SetRootPosition()
        {
            /*double angleOfDashboard = 35f;
            double distanceFromDashboard = _distance + 1f;

            double yOpp = Math.Tan(angleOfDashboard) * distanceFromDashboard;
            double zHyp = yOpp / Math.Sin(angleOfDashboard);*/

            //InternalOverlay.SetAttachment(AttachmentType.Absolute, new OpenTK.Vector3(0f, 1f, _distance), new OpenTK.Vector3(0, 180, 0));

            if (_screenIndex > 0)
                return;

            if (ConfigUtility.Get<string>("desktop.position", "Opposite Dashboard") == "Opposite Dashboard")
                InternalOverlay.SetAttachment(AttachmentType.AbsoluteRelative, new OpenTK.Vector3(0f, 0.5f, 2f + _distance), new OpenTK.Vector3(0, 180, 0), "vrub.anchor.dashboard");
            else
                InternalOverlay.SetAttachment(AttachmentType.Absolute, new OpenTK.Vector3(0, 1f, _distance), new OpenTK.Vector3(0, 180, 0));

            InternalOverlay.Width = _width;
            InternalOverlay.Alpha = 0.95f;
        }

        void SetPositionRelativeToPrimary()
        {
            float metersInAPixel = _width / DesktopMirrorManager.PrimaryDisplay._screenObject.Bounds.Width;

            OpenTK.Vector2 scaler = new OpenTK.Vector2(_screenObject.Bounds.Width / DesktopMirrorManager.PrimaryDisplay._screenObject.Bounds.Width, _screenObject.Bounds.Height / DesktopMirrorManager.PrimaryDisplay._screenObject.Bounds.Height);
            Logger.Debug("[DESKTOP] Meters in a Pixel: " + metersInAPixel);

            OpenTK.Vector3 newPos = new OpenTK.Vector3(_screenObject.Bounds.X * metersInAPixel, -_screenObject.Bounds.Y * metersInAPixel, 0);
            OpenTK.Vector3 rotation = new OpenTK.Vector3(0, 0, 0);

            float scaledWidth = _width * scaler.X;

            InternalOverlay.SetAttachment(AttachmentType.Overlay, newPos, rotation, "vrub.desktop.0");
            InternalOverlay.Width = scaledWidth;
        }



        public void Update()
        {
            while (OpenVR.Overlay.PollNextOverlayEvent(InternalOverlay.Handle, ref eventData, eventSize))
            {
                EVREventType type = (EVREventType)eventData.eventType;

                switch(type)
                {
                    case EVREventType.VREvent_MouseButtonDown:
                        if (useTouch)
                            HandleTouchDown();
                        else
                            HandleMouseDown();
                        break;

                    case EVREventType.VREvent_InputFocusReleased:
                        if (useTouch)
                            HandleTouchLost();
                        else
                            HandleMouseLost();
                        break;

                    case EVREventType.VREvent_MouseButtonUp:
                        if (useTouch)
                            HandleTouchUp();
                        else
                            HandleMouseUp();
                        break;

                    case EVREventType.VREvent_MouseMove:
                        if (useTouch)
                            HandleTouchMove();
                        else
                            HandleMouseMove();
                        break;
                }
            }
        }

        public void Draw()
        {
            UpdateTexture();
            ApplyTexture();

            if (_latestFrame != null)
                _latestFrame = null;
        }

        void UpdateTexture()
        {
            lock (_frameLock)
            {
                if (_latestFrame != null && _latestFrame.DesktopImage != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, _glTextureId);

                    if (_didFirst)
                    {
                        foreach (Rectangle rec in _latestFrame.UpdatedRegions)
                        {
                            BitmapData bmpData = _latestFrame.DesktopImage.LockBits(
                                new Rectangle(rec.X, rec.Y, rec.Size.Width, rec.Size.Height),
                                ImageLockMode.ReadOnly,
                                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                            );

                            GL.TexSubImage2D(TextureTarget.Texture2D, 0, rec.X, rec.Y, rec.Size.Width, rec.Size.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                            _latestFrame.DesktopImage.UnlockBits(bmpData);
                        }

                        /*BitmapData bmpData = _latestFrame.DesktopImage.LockBits(
                            new Rectangle(0, 0, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb
                            );

                        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                        _latestFrame.DesktopImage.UnlockBits(bmpData);*/
                    }
                    else
                    {
                        BitmapData bmpData = _latestFrame.DesktopImage.LockBits(
                            new Rectangle(0, 0, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb
                            );

                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _latestFrame.DesktopImage.Width, _latestFrame.DesktopImage.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
                        _didFirst = true;
                        _latestFrame.DesktopImage.UnlockBits(bmpData);
                    }

                    _latestFrame.DesktopImage.Dispose();

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }

                _requestNew = true;
            }
        }

        void ApplyTexture()
        {
            if(_latestFrame != null)
            {
                InternalOverlay.SetTexture(ref _textureData);
            }
        }

        public void Destroy()
        {
            InternalOverlay.Destroy();
            _desktopDuplicator.Destroy();

            if(frameGetter.IsAlive)
                frameGetter.Abort();
        }

        void HandleMouseUp()
        {
            DesktopMirrorManager.InputSimulator.Mouse.LeftButtonUp();
        }

        void HandleMouseDown()
        {
            DesktopMirrorManager.InputSimulator.Mouse.LeftButtonDown();
        }

        void HandleMouseLost()
        {
            DesktopMirrorManager.InputSimulator.Mouse.LeftButtonUp();
        }

        void HandleMouseMove()
        {
            int xPos = (_screenObject.Bounds.Left + (int)eventData.data.mouse.x) - SystemInformation.VirtualScreen.X;
            int yPos = (_screenObject.Bounds.Bottom - (int)eventData.data.mouse.y) - SystemInformation.VirtualScreen.Y;
            DesktopMirrorManager.InputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(
                Math.Round((double)((float)65535 * ((float)xPos / (float)SystemInformation.VirtualScreen.Width))),
                Math.Round((double)((float)65535 * ((float)yPos / (float)SystemInformation.VirtualScreen.Height)))
                );
        }

        #region Touch Handlers

        void HandleTouchLost()
        {
            ReleaseTouch();
        }

        void HandleTouchMove()
        {
            if (!isTouching)
                return;

            touch.PointerInfo.PtPixelLocation.X = _screenObject.Bounds.Left + (int)Math.Round((double)eventData.data.mouse.x);
            touch.PointerInfo.PtPixelLocation.Y = _screenObject.Bounds.Bottom - ((int)Math.Round((double)eventData.data.mouse.y) + touchYOffset);
            touch.PointerInfo.PointerFlags = PointerFlags.UPDATE | PointerFlags.INCONTACT | PointerFlags.INRANGE;

            VRControllerState_t cState = new VRControllerState_t();
            OpenVR.System.GetControllerState(OpenVR.Overlay.GetPrimaryDashboardDevice(), ref cState, cStateSize);

            if (!TouchInjector.InjectTouchInput(1, new PointerTouchInfo[] { touch }))
            {
                Logger.Warning("[DESKTOP] Failed to inject touch move: " + GetLastError());
            }
        }

        void HandleTouchDown()
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

            touch.PointerInfo.PtPixelLocation.X = _screenObject.Bounds.Left + (int)Math.Round((double)eventData.data.mouse.x);
            touch.PointerInfo.PtPixelLocation.Y = _screenObject.Bounds.Bottom - ((int)Math.Round((double)eventData.data.mouse.y) + touchYOffset);

            touch.ContactArea.top = touch.PointerInfo.PtPixelLocation.Y - 3;
            touch.ContactArea.bottom = touch.PointerInfo.PtPixelLocation.Y + 3;

            touch.ContactArea.left = touch.PointerInfo.PtPixelLocation.X - 6;
            touch.ContactArea.right = touch.PointerInfo.PtPixelLocation.X + 3;

            touch.PointerInfo.PointerId = 10;

            if(!TouchInjector.InjectTouchInput(1, new PointerTouchInfo[] { touch }))
            {
                Logger.Warning("[DESKTOP] Failed to inject touch down: " + GetLastError());
            } else
            {
                isTouching = true;
            }
        }

        void HandleTouchUp()
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
        #endregion
    }
}
