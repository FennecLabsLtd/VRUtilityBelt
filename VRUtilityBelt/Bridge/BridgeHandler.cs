using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRUB.Addons.Overlays;
using VRUB.Utility;

namespace VRUtilityBelt.Bridge
{
    public class BridgeHandler
    {
        Overlay _overlay;

        static Dictionary<string, object> GlobalBridgeLinks = new Dictionary<string, object>();

        Dictionary<string, object> BridgeLinks = new Dictionary<string, object>();

        public static void RegisterGlobalLink(string key, object instance)
        {
            GlobalBridgeLinks[key] = instance;
        }

        public BridgeHandler(Overlay overlay)
        {
            _overlay = overlay;
        }

        public void RegisterLink(string key, object instance)
        {
            BridgeLinks[key] = instance;
        }

        public void FireEvent(string eventName, object payload)
        {
            _overlay.InternalOverlay.TryExecAsyncJS("VRUB.Events.Fire('" + eventName + "', " + JsonConvert.SerializeObject(payload) + ")");
        }

        public bool CallSync(string objectName, string methodName, string promiseUUID, string arguments)
        {
            object[] args = JsonConvert.DeserializeObject<object[]>(arguments);

            object link = null;

            if (BridgeLinks.ContainsKey(objectName))
                link = BridgeLinks[objectName];
            else if (GlobalBridgeLinks.ContainsKey(objectName))
                link = GlobalBridgeLinks[objectName];
            else
            {
                ReturnPromise(promiseUUID, false);
                return false;
            }

            if (link == null)
            {
                Logger.Error("[BRIDGE] Tried to call unknown method " + methodName + " on " + objectName);
                Error(objectName + "::" + methodName + ": Unknown Method");
                ReturnPromise(promiseUUID, false);
                return false;
            }

            Type type = link.GetType();

            try
            {
                MethodInfo method = type.GetMethod(methodName, System.Reflection.BindingFlags.Instance |  BindingFlags.Public);



                try
                {
                    ReturnPromise(promiseUUID, method.Invoke(link, args));
                    return true;
                } catch(Exception e)
                {
                    Logger.Error("[BRIDGE] Failed to call method " + methodName + " on " + objectName + ": " + e.Message);
                    Error(objectName + "::" + methodName + ": " + e.Message);
                    ReturnPromise(promiseUUID, false);
                    return false;
                }
            } catch(Exception e)
            {
                Logger.Error("[BRIDGE] Failed to find method " + methodName + " on " + objectName + ": " + e.Message);
                Error(objectName + "::" + methodName + ": " + e.Message);
                ReturnPromise(promiseUUID, false);
                return false;
            }
        }

        public async void Call (string objectName, string methodName, string promiseUUID, string arguments)
        {
            await Task.Run(() =>
            {
                CallSync(objectName, methodName, promiseUUID, arguments);
            });
        }

        void ReturnPromise(string UUID, object response)
        {
            FireEvent("promise-" + UUID, response);
        }

        void Error(string message)
        {
            FireEvent("BridgeError", message);
        }
    }
}
