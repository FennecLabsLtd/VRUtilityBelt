using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;
using VRUB.Addons.Overlays;

namespace VRUB.API
{
    public class IPlugin
    {
        /// <summary>
        /// OnLoad is called once when the DLL is loaded, not per-addon that uses it.
        /// </summary>
        /// <param name="manager">Instance of AddonManager used to manage all addons</param>
        /// <param name="owner">Owner of this plugin.</param>
        public virtual void OnLoad(AddonManager manager, Addon owner)
        {

        }

        /// <summary>
        /// Called for each overlay that uses this plugin
        /// </summary>
        /// <param name="parentAddon">The addon that that this plugin is being registered to (may happen multiple times, as plugins register to overlays directly)</param>
        /// <param name="overlay">The overlay that that this plugin is being registered to</param>
        public virtual void OnRegister(Addon parentAddon, Overlay overlay)
        {

        }

        /// <summary>
        /// Called after Browser object creation, but before it starts. Registration of interopation JS objects is usually done here.
        /// </summary>
        /// <param name="parentAddon"></param>
        /// <param name="overlay"></param>
        /// <param name="browser"></param>
        public virtual void OnBrowserPreInit(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {
            
        }

        /// <summary>
        /// Called after Browser has initialised, but it may not have loaded the page yet.
        /// </summary>
        /// <param name="parentAddon"></param>
        /// <param name="overlay"></param>
        /// <param name="browser"></param>
        public virtual void OnBrowserReady(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {

        }

        /// <summary>
        /// Called when the Browser navigates to a new page.
        /// </summary>
        /// <param name="parentAddon"></param>
        /// <param name="overlay"></param>
        /// <param name="browser"></param>
        public virtual void OnBrowserNavigation(Addon parentAddon, Overlay overlay, ChromiumWebBrowser browser)
        {

        }

        /// <summary>
        /// Update is called every frame for every overlay, use it sparingly - it won't block the browsers but will block rendering to OpenVR
        /// </summary>
        /// <param name="parentAddon"></param>
        /// <param name="overlay"></param>
        public virtual void Update(Addon parentAddon, Overlay overlay) { }

        /// <summary>
        /// Draw is called every frame for every overlay, use it sparingly - it won't block the browser but will block rendering to OpenVR
        /// </summary>
        /// <param name="parentAddon"></param>
        /// <param name="overlay"></param>
        public virtual void Draw(Addon parentAddon, Overlay overlay) { }

        /// <summary>
        /// Global update called once per frame, non-overlay specific.
        /// </summary>
        public virtual void GlobalUpdate() { }
    }
}
