using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RayTracing.Math.Types;

namespace RayTracing.Math
{
    internal static class Rand
    {
        internal static Vec3 RandVec3()
        {
            Random random = new Random();
            Vec3 ret = new Vec3(random.NextDouble()-0.5d,random.NextDouble()-0.5d,random.NextDouble()-0.5d);
            return ret;
        }
    }
}
