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
using System.Diagnostics;

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
        public float Meters { get; set; } = 1f;

        [JsonProperty("position")]
        public Vector3 Position { get; set; } = Vector3.Zero;

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [JsonProperty("opacity")]
        public float Opacity { get; set; } = 1;

        [JsonProperty("absolute")]
        public bool Absolute { get; set; } = false;

        [JsonProperty("parent")]
        public string ParentKey { get; set; }

        [JsonProperty("animations")]
        public List<RenderModelAnimation> Animations { get; set; }

        [JsonProperty("starting_animation")]
        public string StartingAnimation { get; set; }

        public RenderModelAnimation CurrentAnimation { get; private set; }

        Overlay _parent;
        public Overlay Parent { get { return _parent; } }

        InternalOverlay _internalOverlay;

        static bool _generatedOnePixelTexture = false;
        static Texture_t _onePixelTexture;
        static int _glOnePixelTextureId;

        Stopwatch _animationStopwatch = new Stopwatch();
        int _animationFrame = 0;
        bool _doReverse = false;


        public void Setup(Overlay parent)
        {
            _parent = parent;
            _internalOverlay = new InternalOverlay("vrub." + _parent.DerivedKey + ".models." + Key, "", Meters, true);

            if(Absolute)
                _internalOverlay.SetAttachment(AttachmentType.Absolute, Position, Rotation);
            else
                _internalOverlay.SetAttachment(AttachmentType.Overlay, Position, Rotation, ParentKey == null ? "vrub." + _parent.DerivedKey : ParentKey);

            _internalOverlay.SetTextureSize(1, 1);
            _internalOverlay.ToggleInput(false);

            if (!_generatedOnePixelTexture)
                GenerateOnePixelTexture(Width, Height);

            if(Animations != null && Animations.Count > 0)
            {
                foreach(RenderModelAnimation anim in Animations)
                {
                    anim.GetFrames(parent.BasePath);
                }
            }

            if (StartingAnimation != null)
                SetAnimation(StartingAnimation);
            else
                UpdateModel(ModelFile);

            _internalOverlay.Show();
        }

        public void SetAnimation(string animName)
        {
            if (Animations == null)
                return;

            _doReverse = false;

            foreach(RenderModelAnimation anim in Animations)
            {
                if (anim.Name == animName)
                {
                    CurrentAnimation = anim;
                    _animationStopwatch.Reset();
                    _animationFrame = 0;
                    UpdateAnimationFrame();
                    return;
                }
            }
        }

        public void SetTransform(AttachmentType type, Vector3 position, Vector3 rotation, string attachmentKey = null)
        {
            _internalOverlay.SetAttachment(type, position, rotation, attachmentKey);
            Position = position;
            Rotation = rotation;
            Absolute = type == AttachmentType.Absolute;
        }

        static void GenerateOnePixelTexture(int width, int height)
        {
            OpenVRTools.OnePixelTexture(out _onePixelTexture, out _glOnePixelTextureId, width, height);

            _generatedOnePixelTexture = true;
        }

        void UpdateAnimationFrame()
        {
            UpdateModel(CurrentAnimation.Frames[_animationFrame]);
        }

        void UpdateModel(string filePath)
        {
            HmdColor_t col = new HmdColor_t();
            col.a = Opacity;
            col.r = 1;
            col.g = 1;
            col.b = 1;

            string ModelFilePath = Path.IsPathRooted(filePath) ? filePath : PathUtilities.GetTruePath(_parent.BasePath, filePath);

            EVROverlayError err = OpenVR.Overlay.SetOverlayRenderModel(_internalOverlay.Handle, ModelFilePath, ref col);

            if(err != EVROverlayError.None)
            {
                Logger.Error("[RENDERMODEL] Failed to set overlay render model: " + err.ToString());
            } else
            {
                err = OpenVR.Overlay.SetOverlayTexture(_internalOverlay.Handle, ref _onePixelTexture);

                if (err != EVROverlayError.None)
                    Logger.Error("[RENDERMODEL] Failed to set overlay render model stub texture: " + err.ToString());
                else if(CurrentAnimation == null) // Don't log if we're animating.
                {
                    Logger.Info("[RENDERMODEL] Set render model " + ModelFilePath + " and stub texture for " + _parent.DerivedKey + ".models." + Key);
                }
            }

            /*for(uint i = 0; i < OpenVR.RenderModels.GetRenderModelCount(); i++)
            {
                StringBuilder renderModelName = new StringBuilder(255);
                StringBuilder renderModelPath = new StringBuilder(1024);
                OpenVR.RenderModels.GetRenderModelName(i, renderModelName, 255);
                EVRRenderModelError rerr = EVRRenderModelError.None;
                OpenVR.RenderModels.GetRenderModelOriginalPath(renderModelName.ToString().Trim(), renderModelPath, 1024, ref rerr);

                Logger.Trace("Render Model: " + renderModelName.ToString().Trim() + ", path: " + renderModelPath.ToString().Trim() + ", Error: " + rerr.ToString());
            }*/
        }

        public void Update()
        {
            if(CurrentAnimation != null)
            {
                if(_animationStopwatch.ElapsedMilliseconds > CurrentAnimation.FrameTime)
                {
                    _animationFrame += _doReverse ? -1 : 1;
                    if (_animationFrame >= CurrentAnimation.Frames.Count)
                    {
                        _animationFrame = CurrentAnimation.PingPong ? CurrentAnimation.Frames.Count - 2 : 0;

                        if (CurrentAnimation.PingPong)
                            _doReverse = true;
                    } else if(_animationFrame < 0)
                    {
                        if (CurrentAnimation.PingPong)
                            _doReverse = false;

                        _animationFrame = CurrentAnimation.PingPong ? 1 : 0;
                    }

                    UpdateAnimationFrame();

                    _animationStopwatch.Restart();
                }

                if (!_animationStopwatch.IsRunning)
                    _animationStopwatch.Start();
            }
        }

        public void Draw()
        {

        }

        public void Destroy()
        {
            _internalOverlay.Destroy();
        }
    }
}
