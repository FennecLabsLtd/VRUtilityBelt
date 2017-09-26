using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons;

namespace VRUB.API
{
    public class IPlugin
    {
        /// <summary>
        /// OnLoad is called once when the DLL is loaded, not per-addon that uses it.
        /// </summary>
        /// <param name="manager">Instance of AddonManager used to manage all addons</param>
        public virtual void OnLoad(AddonManager manager)
        {

        }

        /// <summary>
        /// Called for each addon that uses it
        /// </summary>
        /// <param name="parentAddon">The calling addon that this plugin is being registered to</param>
        public virtual void OnRegister(Addon parentAddon)
        {

        }

        /// <summary>
        /// Called after Browser object creation, but before it starts. Registration of interopation JS objects is usually done here.
        /// </summary>
        /// <param name="browser"></param>
        public virtual void OnBrowserPreInit(ChromiumWebBrowser browser)
        {
            
        }

        /// <summary>
        /// Called after Browser has initialised, but it may not have loaded the page yet.
        /// </summary>
        /// <param name="browser"></param>
        public virtual void OnBrowserReady(ChromiumWebBrowser browser)
        {

        }

        /// <summary>
        /// Called when the Browser navigates to a new page.
        /// </summary>
        /// <param name="browser"></param>
        public virtual void OnBrowserNavigation(ChromiumWebBrowser browser)
        {

        }

        /// <summary>
        /// Update is called every frame, use it sparingly - it won't block the browsers but will block rendering to OpenVR
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Draw is called every frame, use it sparingly - it won't block the browser but will block rendering to OpenVR
        /// </summary>
        public virtual void Draw() { }
    }
}
