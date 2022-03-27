using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing.Math
{
    internal static class Constants
    {
        public const double PI = System.Math.PI;
        public const double STEPLENGTH = 0.05;
        public const double DEPTHBUFFER = 255;
        public const double GLOBALRAYMAXDISTANCE = 400;
        public const int GLOBALRAYMAXBOUNCE = 4;
        public const int GLOBALMAXSAMPLINGCOUNT = 16;
    }
}
