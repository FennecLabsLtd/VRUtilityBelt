using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCD.System.TouchInjection;
using VRUB.Utility;
using WindowsInput;

namespace VRUB.Desktop
{
    class DesktopMirrorManager
    {
        public List<DesktopMirror> _displayMirrors;

        public static DesktopMirror PrimaryDisplay;

        public static InputSimulator InputSimulator { get; private set; } = new InputSimulator();

        bool _redoSetup = false;
        bool _isActive = false;
        bool _doDestroy = false;

        public DesktopMirrorManager()
        {
            TouchInjector.InitializeTouchInjection();

            ConfigUtility.RegisterCustomConfigLayout("desktop", new List<ConfigUtility.ConfigLayout>() {
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Basic Settings", Key = "enabled", Title = "Enable Desktop Mirror?", Description = "Enables/Disables the improved desktop mirror", Type = "bool", Default = false },
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Screen Settings", Key = "distance", Title = "Display Distance", Description = "Distance of primary display from the middle of the play space", Type = "float", Default = 2f },
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Screen Settings", Key = "width", Title = "Display Overlay Width", Description = "Meter Width of each overlay", Type = "float", Default = 2.5f },
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Screen Settings", Key = "show_without_dashboard", Title = "Show without Dashboard", Description = "Show the desktop mirror even when the dashboard isn't visible", Type = "bool", Default = false },
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Screen Settings", Key = "position", Title = "Desktop Position", Default = "Opposite Dashboard", Description = "Where to place the desktop mirror: opposite the dashboard or at the rear of the play space", Type = "string", Options = new object[] {"Opposite Dashboard", "Rear of Playspace" } },
                new ConfigUtility.ConfigLayout() { NonAddonFileKey = "desktop", Category = "Screen Settings", Key = "use_touch", Title = "Use Touch Injection", Default = false, Description = "Use Touch Injection instead of Mouse, experimental but often provides a better experience.", Type = "bool" }
            });

            ConfigUtility.Listen("desktop.enabled", (k, v) =>
            {
                if (v is bool)
                {
                    if ((bool)v)
                    {
                        Enable();
                    }
                    else
                    {
                        Disable();
                    }
                }
            });

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        void Disable()
        {
            _redoSetup = false;

            if (_isActive)
            {
                _isActive = false;
                _doDestroy = true;
            }
        }

        void Enable()
        {
            _redoSetup = true;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            _redoSetup = true;
        }

        public void SetupMirrors()
        {
            _displayMirrors = new List<DesktopMirror>();

            for(int i = 0; i < Screen.AllScreens.Length; i++)
            {
                _displayMirrors.Add(new DesktopMirror(i, Screen.AllScreens[i]));

                if (i == 0)
                    PrimaryDisplay = _displayMirrors[0];
            }

            _isActive = true;
        }

        void RedoSetup()
        {
            DestroyMirrors();
            SetupMirrors();
            _redoSetup = false;
        }

        void DestroyMirrors()
        {
            if (_displayMirrors == null)
                return;

            foreach(DesktopMirror m in _displayMirrors)
            {
                m.Destroy();
            }

            _displayMirrors.Clear();
        }

        public void Update()
        {
            if (_doDestroy)
            {
                DestroyMirrors();
                _doDestroy = false;
            }

            if (_redoSetup)
                RedoSetup();

            if (!_isActive)
                return;

            foreach(DesktopMirror mirror in _displayMirrors)
            {
                mirror.Update();
            }
        }

        public void Draw()
        {
            if (!_isActive)
                return;

            foreach (DesktopMirror mirror in _displayMirrors)
            {
                mirror.Draw();
            }
        }
    }
}
