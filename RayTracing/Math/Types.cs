using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing.Math
{
    internal class Types
    {
        public const double Err = 0.000001;

        public const double RAYDETECTMAXDIST = 5000;

        internal class Vec3 {

            public double x;
            public double y;
            public double z;

            public int xi { get { return (int)x; } }
            public int yi { get { return (int)y; } }

            public int zi { get { return (int)z; } }

            public double length { get { return System.Math.Sqrt(x*x+y*y+z*z); } }
            public Vec3(double _x = 0, double _y = 0, double _z = 0)
            {
                x = _x; y = _y; z = _z;
            }

            public static Vec3 operator +(Vec3 a, Vec3 b)
            {
                Vec3 vec3 = new Vec3();
                vec3.x = a.x + b.x;
                vec3.y = a.y + b.y;
                vec3.z = a.z + b.z;
                return vec3;
            }
            public static Vec3 operator -(Vec3 a, Vec3 b)
            {
                Vec3 vec3 = new Vec3();
                vec3.x = a.x - b.x;
                vec3.y = a.y - b.y;
                vec3.z = a.z - b.z;
                return vec3;
            }

            public static Vec3 operator /(Vec3 a,double b)
            {
                return new Vec3(a.x / b,a.y / b,a.z / b);
            }

            public static bool operator ==(Vec3? a, Vec3? b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                if (ReferenceEquals(a,null) || ReferenceEquals(b, null))
                {
                    return false;
                }

                return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
            }

            public static bool operator !=(Vec3? a, Vec3? b)
            {
                return !(a == b);
            }
            public static double operator *(Vec3 a, Vec3 b)
            {
                return a.x * b.x + a.y * b.y + a.z * b.z;
            }

            public static Vec3 operator ^(Vec3 a, Vec3 b)
            {
                return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
            }

            public static Vec3 operator *(double lambda,Vec3 a)
            {
                return new Vec3(lambda*a.x,lambda*a.y,lambda*a.z);
            }

            public static Vec3 cross(Vec3 a,Vec3 b)
            {
                Vec3 vec3 = new Vec3();
                vec3.x = a.y * b.z - a.z * b.y;
                vec3.y = -(a.x * b.z - a.z * b.x);
                vec3.z = a.x * b.y - a.y * b.x;
                return vec3;
            }

            public Vec3 Normalize()
            {
                if(this.length==0) return new Vec3(0,0,0);
                return (1.0 / this.length) * this;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (ReferenceEquals(obj, null)||ReferenceEquals(this,null))
                {
                    return false;
                }

                return false;
            }

            public Color ToColor()
            {
                Vec3 ConvertedVector = 0.5*(this.Normalize() + new Vec3(1, 1, 1));
                return Color.FromArgb((int)(255*ConvertedVector.x), (int)(255*ConvertedVector.y), (int)(255*ConvertedVector.z));
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.x, this.y, this.z);
            }
        }

        internal class Vec2
        {

            public double x, y;

            public int xi { get { return (int)x; } }
            public int yi { get { return (int)y; } }
            public Vec2(double _x = 0, double _y = 0)
            {
                x = _x; y = _y;
            }

            public double length()
            {
                return System.Math.Sqrt(x * x + y * y);
            }

            public static Vec2 operator +(Vec2 a, Vec2 b)
            {
                Vec2 Vec2 = new Vec2();
                Vec2.x = a.x + b.x;
                Vec2.y = a.y + b.y;
                return Vec2;
            }
            public static Vec2 operator -(Vec2 a, Vec2 b)
            {
                Vec2 Vec2 = new Vec2();
                Vec2.x = a.x - b.x;
                Vec2.y = a.y - b.y;
                return Vec2;
            }

            public static bool operator ==(Vec2 a, Vec2 b)
            {
                return (a.x == b.x) && (a.y == b.y);
            }

            public static bool operator !=(Vec2 a, Vec2 b)
            {
                return !(a == b);
            }
            public static double operator *(Vec2 a, Vec2 b)
            {
                return a.x * b.x + a.y * b.y;
            }
            public static Vec2 operator *(double lambda, Vec2 a)
            {
                a.x *= lambda;
                a.y *= lambda;
                return a;
            }

            public static double cross(Vec2 a, Vec2 b)
            {
                return a.x*b.y-a.y*b.x;
            }

            public Vec2 Normalize()
            {
                return (1.0 / this.length()) * this;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (ReferenceEquals(obj, null))
                {
                    return false;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y);
            }
        }

    }
}
