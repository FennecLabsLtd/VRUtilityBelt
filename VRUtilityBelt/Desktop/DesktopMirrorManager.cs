using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCD.System.TouchInjection;

namespace VRUB.Desktop
{
    class DesktopMirrorManager
    {
        public List<DesktopMirror> _displayMirrors;

        public DesktopMirrorManager()
        {
            TouchInjector.InitializeTouchInjection();
        }

        public void SetupMirrors()
        {
            _displayMirrors = new List<DesktopMirror>();

            for(int i = 0; i < Screen.AllScreens.Length; i++)
            {
                _displayMirrors.Add(new DesktopMirror(i, Screen.AllScreens[i]));
            }
        }

        public void Update()
        {
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
