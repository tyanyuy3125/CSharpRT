using RayTracing.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RayTracing.Math.Types;

namespace RayTracing.Assets
{
    internal class Objects
    {
        public class GeometryObject
        {
            public Vec3? Position;
            public MaterialBasis? Material;
            public virtual int GetPointRelation(Vec3 coordinate) { throw new NotImplementedException(); }
            public virtual Vec3 GetNormal(Vec3 coordinate) { throw new NotImplementedException(); }
            public virtual Vec3? Intersect(Vec3 From, Vec3 Direction) { throw new NotImplementedException(); }
            public virtual void SetMaterial(MaterialBasis material)
            {
                Material = material;
            }
        }

        public class Sphere : GeometryObject
        {
            public double radius;

            public Sphere(Vec3 coordinate, double radius, MaterialBasis? material =null)
            {
                this.Position = coordinate;
                this.radius = radius;
                this.Material = material;
            }

            public override int GetPointRelation(Vec3 coordinate)
            {
                if(System.Math.Abs(Operation.Length(coordinate - this.Position) - radius) <= Types.Err)
                {
                    return 0;
                }
                if(Operation.Length(coordinate-this.Position) > radius)
                {
                    return 1;
                }
                return -1;
            }

            public override Vec3? Intersect(Vec3 From, Vec3 Direction)
            {
                Vec3 L = Position - From;
                double t_ca = L * Direction;
                if (t_ca < 0) return null;
                double dsquare = L * L - t_ca * t_ca;
                if (dsquare > radius*radius) return null;
                else
                {
                    double d = System.Math.Sqrt(dsquare);
                    double t_hc = System.Math.Sqrt(radius * radius - d*d);
                    double t_0 = t_ca - t_hc;
                    return t_0 * Direction;
                }
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
                this.Material = material;
            }

            public override int GetPointRelation(Vec3 coordinate)
            {
                if ((coordinate - point) * normal < 0)
                {
                    return 0;
                }
                return 1;
            }

            public override Vec3? Intersect(Vec3 From, Vec3 Direction)
            {
                double denom = normal * Direction;
                if(denom < 0)
                {
                    double t = ((point - From) * normal)/denom;
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
                this.Material = material;
                this.side = side;
            }

            public override int GetPointRelation(Vec3 coordinate)
            {
                if ((coordinate - point) * normal < 0)
                {
                    return 0;
                }
                return 1;
            }

            public override Vec3? Intersect(Vec3 From, Vec3 Direction)
            {
                double denom = normal * Direction;
                if (denom < 0)
                {
                    double t = ((point - From) * normal) / denom;
                    //if (t < 0) return null;
                    if (System.Math.Abs((From + t * Direction - point).x) > side / 2 || System.Math.Abs((From + t * Direction - point).y) > side / 2) return null;
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
                this.Material = material;
                this.Radius = Side;
            }

            public override int GetPointRelation(Vec3 coordinate)
            {
                Vec3 ProjectionToPlane = (coordinate - point) - (((coordinate - point) * normal) / (normal.length * normal.length)) * normal;
                if ((coordinate - point) * normal < 0&& ProjectionToPlane.length<Radius)
                {
                    return 0;
                }
                return 1;
            }

            public override Vec3? Intersect(Vec3 From, Vec3 Direction)
            {
                double denom = normal * Direction;
                if (denom < 0)
                {
                    double t = ((point - From) * normal)/denom;
                    if (t < 0) return null;
                    double dist = Operation.Length((From + t * Direction) - point);
                    if(dist > Radius)
                    {
                        return null;
                    }
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
}
