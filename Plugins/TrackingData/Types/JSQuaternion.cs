using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TrackingData.Types
{
    class JSQuaternion
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public JSQuaternion(Quaternion quaternion)
        {
            X = quaternion.X;
            Y = quaternion.Y;
            Z = quaternion.Z;
            W = quaternion.W;
        }
    }
}