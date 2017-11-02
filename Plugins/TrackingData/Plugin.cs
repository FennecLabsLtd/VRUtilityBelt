using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingData.Types;
using Valve.VR;
using VRUB.Addons;
using VRUB.Addons.Overlays;
using VRUB.API;
using VRUB.Bridge;

namespace TrackingData
{
    public class Plugin : IPlugin
    {
        List<Overlay> wantsTrackingData;

        public override void OnLoad(AddonManager manager, Addon owner)
        {
            wantsTrackingData = new List<Overlay>();
            SteamVR_Event.Listen("new_poses", OnNewPoses);
        }

        private void OnNewPoses(params object[] args)
        {
            TrackedDevicePose_t[] poses = (TrackedDevicePose_t[])args[0];

            for (int i = 0; i < poses.Length; i++)
            {
                TrackedDevicePose_t pose = poses[i];
                if(pose.bPoseIsValid)
                {
                    Matrix4 mat = SteamVR_WebKit.TransformUtils.OpenVRMatrixToOpenTKMatrix4(pose.mDeviceToAbsoluteTracking);

                    foreach(Overlay ov in wantsTrackingData)
                    {
                        ov.Bridge.FireEvent("new_pose", new { device_class = OpenVR.System.GetTrackedDeviceClass((uint)i).ToString(), device_index = i, position = new  JSVector3(mat.ExtractTranslation()), rotation = new JSQuaternion(mat.ExtractRotation()), velocity = new JSVector3(pose.vVelocity), angularVelocity = new JSVector3(pose.vAngularVelocity) });
                    }
                }
            }
        }

        public override void OnRegister(Addon parentAddon, Overlay overlay)
        {
            base.OnRegister(parentAddon, overlay);

            wantsTrackingData.Add(overlay);
        }
    }
}
