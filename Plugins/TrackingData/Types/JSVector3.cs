using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingData.Types
{
    class JSVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public JSVector3(Valve.VR.HmdVector3_t vec)
        {
            X = vec.v0;
            Y = vec.v1;
            Z = vec.v2;
        }

        public JSVector3(Vector3 vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }
    }
}
