using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Utility
{
    public class MathUtilities
    {
        public static float Lerp(float start, float finish, float blend)
        {
            return blend * (finish - start) + start;
        }

        public static float InverseLerp(float start, float finish, float value)
        {
            return (value - start) / (finish - start);
        }
    }
}
