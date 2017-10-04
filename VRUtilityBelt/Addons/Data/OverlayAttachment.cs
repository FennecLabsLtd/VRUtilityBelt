using Newtonsoft.Json;
using OpenTK;
using SteamVR_WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace VRUB.Addons.Data
{
    public struct OverlayAttachment
    {
        [JsonProperty("type")]
        public AttachmentType Type;

        [JsonProperty("position")]
        public Vector3 Position;

        [JsonProperty("rotation")]
        public Vector3 Rotation;

        [JsonProperty("key")]
        public string AttachmentKey;
    }
}
