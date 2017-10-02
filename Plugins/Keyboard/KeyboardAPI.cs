using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Overlays;

namespace Keyboard
{
    class KeyboardAPI
    {
        Overlay _owner;

        public KeyboardAPI(Overlay owner)
        {
            _owner = owner;
        }

        public void Show(string value = "")
        {
            _owner.ShowKeyboard(true, value);
        }

        public void Hide()
        {
            _owner.ShowKeyboard(false);
        }
    }
}
