using RayTracing.Math;
using static RayTracing.Math.Types;

namespace RayTracing.Assets;

internal static class Light
{
    public class LightBasis
    {
        public double Illuminance;
        public Vec3 position;

        public virtual int GetPointRelation(Vec3 coordinate)
        {
            throw new NotImplementedException();
        }

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
        private readonly double hitradius;

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
            return Operation.Length(position - objectPosition);
        }

        internal override double GetBrightness(double distance)
        {
            return Illuminance / (distance * distance);
        }

        public override int GetPointRelation(Vec3 coordinate)
        {
            if (System.Math.Abs(Operation.Length(coordinate - position) - hitradius) <= Err) return 0;
            if (Operation.Length(coordinate - position) > hitradius) return 1;
            return -1;
        }
    }

    public class PlaneLight : LightBasis
    {
        public Vec3 direction;

        public PlaneLight(Vec3 direction, double Illuminance = 1000.0d)
        {
            this.direction = direction.Normalize();
            this.Illuminance = Illuminance;
        }

        internal override Vec3 GetDirection(Vec3 objectPosition)
        {
            return -1.0 * direction;
        }

        internal override double GetDistance(Vec3 objectPosition)
        {
            return RAYDETECTMAXDIST;
        }

        internal override double GetBrightness(double distance)
        {
            return Illuminance;
        }
    }
}