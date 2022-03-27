using RayTracing.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RayTracing.Math.Types;
using static RayTracing.Assets.Light;
using System.Drawing;
using RayTracing.Math;
using static RayTracing.Assets.Objects;

namespace RayTracing.Assets
{
    internal class Ray:IDisposable
    {
        public Vec3 direction;
        public Vec3 position;
        public double velocity;
        private double distance;
        public double maxdistance = 500;

        public Ray(Vec3 direction,Vec3 position,double velocity=Constants.STEPLENGTH)
        {
            this.direction = direction.Normalize();
            this.velocity = velocity;
            this.position = position;
        }

        public Color Emit(SceneManager sceneManager)
        {
            while(distance < maxdistance)
            {
                position = position + (velocity*direction);
                distance += velocity;
                if (sceneManager.HitTest(position))
                {
                    foreach(LightBasis lightBasis in sceneManager.lights)
                    {
                        using(Ray testRay = new Ray(lightBasis.GetDirection(position), position))
                        {
                            testRay.position += 1.0d * testRay.direction;
                            if (testRay.CanHit(ref sceneManager, lightBasis.GetDistance(position)))
                            {
                                return Color.Black;
                            }
                            double brightness = lightBasis.GetBrightness(lightBasis.GetDistance(position));
                            if(brightness>255)brightness = 255;
                            return Color.FromArgb((int)brightness, (int)brightness, (int)brightness);
                        }
                    }
                }
            }
            return Color.Black;
        }

        public double ProbeDepth(ref SceneManager sceneManager)
        {
            double depth = Constants.DEPTHBUFFER;
            while (distance < Constants.DEPTHBUFFER)
            {
                position = position + (velocity * direction);
                distance += velocity;
                if (sceneManager.HitTest(position))
                {
                    depth = distance;
                    break;
                }
            }
            return depth;
        }

        public Vec3 ProbeNormal(SceneManager sceneManager)
        {
            while (distance < Constants.GLOBALRAYMAXDISTANCE)
            {
                position = position + (velocity * direction);
                distance += velocity;
                GeometryObject HitObject = sceneManager.GetHitObject(position);
                if (HitObject!=null)
                {
                    return HitObject.GetNormal(position);
                }
            }
            return new Vec3(0, 0, 0);
        }

        public Vec3 OptimizedProbeNormal(SceneManager sceneManager)
        {
                //find the closest.
                Vec3? IntersectVector = null;
                GeometryObject? IntersectObject = null;
                foreach (GeometryObject geometryObject in sceneManager.objects)
                {
                    Vec3? CurrentIntersectVector = geometryObject.Intersect(position, direction);
                    if (CurrentIntersectVector != null)
                    {
                        if (IntersectVector == null)
                        {
                            IntersectVector = CurrentIntersectVector;
                            IntersectObject = geometryObject;
                            continue;
                        }
                        if (Operation.Length(CurrentIntersectVector) < Operation.Length(IntersectVector))
                        {
                            IntersectVector = CurrentIntersectVector;
                            IntersectObject = geometryObject;
                            continue;
                        }
                    }
                }
            if (IntersectVector == null) return new Vec3(0, 0, 0);
            return IntersectObject.GetNormal(position+IntersectVector);
        }

        public bool CanHit(ref SceneManager sceneManager, double dist)
        {
            while (distance < dist)
            {
                position += (velocity * direction);
                distance += velocity;
                if (sceneManager.HitTest(position))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Ray that supports Path Tracing
    /// </summary>
    internal class PathTracingRay : IDisposable
    {
        // Indicates current position.
        public Vec3 Position;
        public Vec3 Direction;
        public double Velocity;
        private double RAYMAXDISTANCE;
        private int RAYMAXBOUNCE;
        private double Distance;

        /// <summary>
        /// Initialize a ray that supports Path Tracing
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Direction"></param>
        /// <param name="Velocity"></param>
        /// <param name="RAYMAXDISTANCE">(Optional) The max probe distance of the ray.</param>
        public PathTracingRay(Vec3 Direction, Vec3 Position, double Velocity = Constants.STEPLENGTH, double RAYMAXDISTANCE = Constants.GLOBALRAYMAXDISTANCE, int RAYMAXBOUNCE = Constants.GLOBALRAYMAXBOUNCE)
        {
            this.Position = Position;
            this.Direction = Direction.Normalize();
            this.Velocity = Velocity;
            this.RAYMAXDISTANCE = RAYMAXDISTANCE;
            this.RAYMAXBOUNCE = RAYMAXBOUNCE;
        }

        public Vec3 OptimizedEmit(SceneManager sceneManager)
        {
            Vec3 RetVec = new Vec3();
            Vec3 WholeRay = new Vec3(1.0d, 1.0d, 1.0d);
            int Bounces = 0;
            GeometryObject? LastIntersectObject = null;
            while (Bounces < RAYMAXBOUNCE)
            {
                //find the closest.
                Vec3? IntersectVector = null;
                GeometryObject? IntersectObject = null;
                foreach(GeometryObject geometryObject in sceneManager.objects)
                {
                    if(geometryObject == LastIntersectObject) { continue; }
                    Vec3? CurrentIntersectVector = geometryObject.Intersect(Position, Direction);
                    if(CurrentIntersectVector != null)
                    {
                        if(IntersectVector == null)
                        {
                            IntersectVector = CurrentIntersectVector;
                            IntersectObject = geometryObject;
                            continue;
                        }
                        if (Operation.Length(CurrentIntersectVector) < Operation.Length(IntersectVector))
                        {
                            IntersectVector = CurrentIntersectVector;
                            IntersectObject = geometryObject;
                            continue;
                        }
                    }
                }
                if (IntersectVector == null) break;
                LastIntersectObject = IntersectObject;
                this.Position = Position + IntersectVector;
                Vec3 NextDirection = IntersectObject.Material.GetOutRayDirection(Direction, IntersectObject.GetNormal(Position));
                //RetVec += IntersectObject.Material.Albedo ^ WholeRay;
                if (IntersectObject.Material.GetType()!=typeof(Reflection))
                {
                    
                    if(IntersectObject.Material.Emission!=null)RetVec += IntersectObject.Material.Emission ^ WholeRay;
                    WholeRay ^= IntersectObject.Material.Albedo;
                    //RetVec = RetVec / (IntersectVector.length * IntersectVector.length);
                    //break;
                }
                
                Direction = NextDirection;
                Bounces++;
            }
            //return Operation.ReduceToMaxLimit(RetVec, new Vec3(1, 1, 1));
            return RetVec;
        }

        public Vec3 Emit(SceneManager sceneManager)
        {
            Vec3 RetVec = new Vec3();
            Vec3 WholeRay = new Vec3(1.0d, 1.0d, 1.0d);
            int Bounces = 0;
            while (Distance < RAYMAXDISTANCE && Bounces < RAYMAXBOUNCE)
            {
                Position += Velocity * Direction;
                Distance += Velocity;

                GeometryObject HitObject = sceneManager.GetHitObject(Position);
                if(HitObject == null)
                {
                    continue;
                }
                else
                {
                    Vec3 NextDirection = HitObject.Material.GetOutRayDirection(Direction, HitObject.GetNormal(Position));
                    if(NextDirection == new Vec3(0,0,0))
                    {
                        // 光源颜色是否应该用相乘来表示？
                        // 是否应该考虑平方反比衰减？
                        RetVec += HitObject.Material.Albedo ^ WholeRay;
                        break;
                    }
                    else
                    {
                        WholeRay ^= HitObject.Material.Albedo;
                        Direction = NextDirection;
                        Position += Direction;
                        Distance += Velocity;
                    }
                    Bounces++;
                }
            }
            return Operation.ReduceToMaxLimit(RetVec, new Vec3(1, 1, 1));
        }

        public void Dispose()
        {
            GC.SuppressFinalize (this);
        }
    }
}
