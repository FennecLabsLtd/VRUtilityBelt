using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCD.System.TouchInjection;
using VRUB.Utility;

namespace VRUB.Desktop
{
    class DesktopMirrorManager
    {
        public List<DesktopMirror> _displayMirrors;

        public static DesktopMirror PrimaryDisplay;

        bool _redoSetup = false;

        public DesktopMirrorManager()
        {
            TouchInjector.InitializeTouchInjection();

            ConfigUtility.SetDefaults("desktop", new Dictionary<string, object>()
            {
                { "distance", "2" },
                { "width", "1.5" },
            });

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
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
        }

        void RedoSetup()
        {
            DestroyMirrors();
            SetupMirrors();
            _redoSetup = false;
        }

        void DestroyMirrors()
        {
            foreach(DesktopMirror m in _displayMirrors)
            {
                m.Destroy();
            }

            _displayMirrors.Clear();
        }

        public void Update()
        {
            if (_redoSetup)
                RedoSetup();

            foreach(DesktopMirror mirror in _displayMirrors)
            {
                mirror.Update();
            }
        }

        public void Draw()
        {
            foreach (DesktopMirror mirror in _displayMirrors)
            {
                mirror.Draw();
            }
        }
    }
}
