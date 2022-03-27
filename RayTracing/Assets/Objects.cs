using RayTracing.Math;
using static RayTracing.Math.Types;

namespace RayTracing.Assets;

internal class Objects
{
    public class GeometryObject
    {
        public MaterialBasis? Material;
        public Vec3? Position;

        public virtual int GetPointRelation(Vec3 coordinate)
        {
            throw new NotImplementedException();
        }

        public virtual Vec3 GetNormal(Vec3 coordinate)
        {
            throw new NotImplementedException();
        }

        public virtual Vec3? Intersect(Vec3 From, Vec3 Direction)
        {
            throw new NotImplementedException();
        }

        public virtual void SetMaterial(MaterialBasis material)
        {
            Material = material;
        }
    }

    public class Sphere : GeometryObject
    {
        public double radius;

        public Sphere(Vec3 coordinate, double radius, MaterialBasis? material = null)
        {
            Position = coordinate;
            this.radius = radius;
            Material = material;
        }

        public override int GetPointRelation(Vec3 coordinate)
        {
            if (System.Math.Abs(Operation.Length(coordinate - Position) - radius) <= Err) return 0;
            if (Operation.Length(coordinate - Position) > radius) return 1;
            return -1;
        }

        public override Vec3? Intersect(Vec3 From, Vec3 Direction)
        {
            var L = Position - From;
            var t_ca = L * Direction;
            if (t_ca < 0) return null;
            var dsquare = L * L - t_ca * t_ca;
            if (dsquare > radius * radius)
            {
                return null;
            }

            var d = System.Math.Sqrt(dsquare);
            var t_hc = System.Math.Sqrt(radius * radius - d * d);
            var t_0 = t_ca - t_hc;
            return t_0 * Direction;
        }

        public override Vec3 GetNormal(Vec3 coordinate)
        {
            return coordinate - Position;
        }
    }

    public class Plane : GeometryObject
    {
        public Vec3 normal;
        public Vec3 point;

        public Plane(Vec3 normal, Vec3 point, MaterialBasis? material = null)
        {
            this.normal = normal.Normalize();
            this.point = point;
            Material = material;
        }

        public override int GetPointRelation(Vec3 coordinate)
        {
            if ((coordinate - point) * normal < 0) return 0;
            return 1;
        }

        public override Vec3? Intersect(Vec3 From, Vec3 Direction)
        {
            var denom = normal * Direction;
            if (denom < 0)
            {
                var t = (point - From) * normal / denom;
                //if (t < 0) return null;
                return t * Direction;
            }

            return null;
        }

        public override Vec3 GetNormal(Vec3 coordinate)
        {
            return normal;
        }
    }

    public class XYLimitedPlane : GeometryObject
    {
        public Vec3 normal;
        public Vec3 point;
        public double side;

        public XYLimitedPlane(Vec3 normal, Vec3 point, double side, MaterialBasis? material = null)
        {
            this.normal = normal.Normalize();
            this.point = point;
            Material = material;
            this.side = side;
        }

        public override int GetPointRelation(Vec3 coordinate)
        {
            if ((coordinate - point) * normal < 0) return 0;
            return 1;
        }

        public override Vec3? Intersect(Vec3 From, Vec3 Direction)
        {
            var denom = normal * Direction;
            if (denom < 0)
            {
                var t = (point - From) * normal / denom;
                //if (t < 0) return null;
                if (System.Math.Abs((From + t * Direction - point).x) > side / 2 ||
                    System.Math.Abs((From + t * Direction - point).y) > side / 2) return null;
                return t * Direction;
            }

            return null;
        }

        public override Vec3 GetNormal(Vec3 coordinate)
        {
            return normal;
        }
    }

    public class Circle : GeometryObject
    {
        public Vec3 normal;
        public Vec3 point;
        public double Radius;

        public Circle(Vec3 normal, Vec3 point, double Side, MaterialBasis? material = null)
        {
            this.normal = normal;
            this.point = point;
            Material = material;
            Radius = Side;
        }

        public override int GetPointRelation(Vec3 coordinate)
        {
            var ProjectionToPlane = coordinate - point -
                                    (coordinate - point) * normal / (normal.length * normal.length) * normal;
            if ((coordinate - point) * normal < 0 && ProjectionToPlane.length < Radius) return 0;
            return 1;
        }

        public override Vec3? Intersect(Vec3 From, Vec3 Direction)
        {
            var denom = normal * Direction;
            if (denom < 0)
            {
                var t = (point - From) * normal / denom;
                if (t < 0) return null;
                var dist = Operation.Length(From + t * Direction - point);
                if (dist > Radius) return null;
                //重大bug：不应该返回From + t*Direction，这样会导致比较距离的时候相当于是比较原点到规定点的距离
                return t * Direction;
            }

            return null;
        }

        public override Vec3 GetNormal(Vec3 coordinate)
        {
            return normal;
        }
    }
}