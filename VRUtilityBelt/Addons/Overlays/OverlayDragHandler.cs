using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Addons.Overlays
{
    class OverlayDragHandler : IDragHandler
    {
        Overlay _owner;
        public OverlayDragHandler(Overlay owner)
        {
            _owner = owner;
        }

        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return true;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
            
        }
    }
}
