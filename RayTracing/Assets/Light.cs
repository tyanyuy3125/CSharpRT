using RayTracing.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RayTracing.Math.Types;

namespace RayTracing.Assets
{
    internal static class Light
    {
        public class LightBasis
        {
            public Vec3 position;
            public double Illuminance;
            public virtual int GetPointRelation(Vec3 coordinate) { throw new NotImplementedException(); }

            internal virtual Vec3 GetDirection(Vec3 objectPosition)
            {
                throw new NotImplementedException();
            }

            internal virtual double GetDistance(Vec3 position)
            {
                throw new NotImplementedException();
            }

            internal virtual double GetBrightness(double distance)
            {
                throw new NotImplementedException();
            }
        }
        public class PointLight : LightBasis
        {
            double hitradius;

            public PointLight(Vec3 position, double hitradius, double Illuminance = 1000.0d)
            {
                this.position = position;
                this.hitradius = hitradius;
                this.Illuminance = Illuminance;
            }

            internal override Vec3 GetDirection(Vec3 objectPosition)
            {
                return position - objectPosition;
            }

            internal override double GetDistance(Vec3 objectPosition)
            {
                return Operation.Length(position- objectPosition);
            }

            internal override double GetBrightness(double distance)
            {
                return Illuminance / (distance*distance);
            }

            public override int GetPointRelation(Vec3 coordinate)
            {
                if (System.Math.Abs(Operation.Length(coordinate - this.position) - hitradius) <= Types.Err)
                {
                    return 0;
                }
                if (Operation.Length(coordinate - this.position) > hitradius)
                {
                    return 1;
                }
                return -1;
            }
        }

        public class PlaneLight : LightBasis
        {
            public Vec3 direction;
            internal override Vec3 GetDirection(Vec3 objectPosition)
            {
                return (-1.0)*direction;
            }
            internal override double GetDistance(Vec3 objectPosition)
            {
                return Types.RAYDETECTMAXDIST;
            }
            internal override double GetBrightness(double distance)
            {
                return Illuminance;
            }
            public PlaneLight(Vec3 direction, double Illuminance = 1000.0d)
            {
                this.direction = direction.Normalize();
                this.Illuminance = Illuminance;
            }
        }
    }
}
