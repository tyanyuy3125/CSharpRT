using RayTracing.Math;
using RayTracing.Scene;
using static RayTracing.Assets.Objects;
using static RayTracing.Math.Types;

namespace RayTracing.Assets;

internal class Ray : IDisposable
{
    public Vec3 direction;
    private double distance;
    public double maxdistance = 500;
    public Vec3 position;
    public double velocity;

    public Ray(Vec3 direction, Vec3 position, double velocity = Constants.STEPLENGTH)
    {
        this.direction = direction.Normalize();
        this.velocity = velocity;
        this.position = position;
    }

    public void Dispose()
    {
        //GC.SuppressFinalize(this);
    }

    public Color Emit(SceneManager sceneManager)
    {
        while (distance < maxdistance)
        {
            position += velocity * direction;
            distance += velocity;
            if (!sceneManager.HitTest(position)) continue;
            foreach (var lightBasis in sceneManager.lights)
            {
                using var testRay = new Ray(lightBasis.GetDirection(position), position);
                testRay.position += 1d * testRay.direction;
                if (testRay.CanHit(ref sceneManager, lightBasis.GetDistance(position))) return Color.Black;
                var brightness = lightBasis.GetBrightness(lightBasis.GetDistance(position));
                if (brightness > 255) brightness = 255;
                return Color.FromArgb((int)brightness, (int)brightness, (int)brightness);
            }
        }

        return Color.Black;
    }

    public double ProbeDepth(ref SceneManager sceneManager)
    {
        var depth = Constants.DEPTHBUFFER;
        while (distance < Constants.DEPTHBUFFER)
        {
            position += velocity * direction;
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
            position += velocity * direction;
            distance += velocity;
            var HitObject = sceneManager.GetHitObject(position);
            if (HitObject != null) return HitObject.GetNormal(position);
        }

        return new Vec3();
    }

    public Vec3 OptimizedProbeNormal(SceneManager sceneManager)
    {
        //find the closest.
        Vec3? IntersectVector = null;
        GeometryObject? IntersectObject = null;
        foreach (var geometryObject in sceneManager.objects)
        {
            var CurrentIntersectVector = geometryObject.Intersect(position, direction);
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
                }
            }
        }

        if (IntersectVector == null) return new Vec3();
        return IntersectObject.GetNormal(position + IntersectVector);
    }

    public bool CanHit(ref SceneManager sceneManager, double dist)
    {
        while (distance < dist)
        {
            position += velocity * direction;
            distance += velocity;
            if (sceneManager.HitTest(position)) return true;
        }

        return false;
    }
}

/// <summary>
///     Ray that supports Path Tracing
/// </summary>
internal class PathTracingRay : IDisposable
{
    public Vec3 Direction;

    private double Distance;

    // Indicates current position.
    public Vec3 Position;
    private readonly int RAYMAXBOUNCE;
    private readonly double RAYMAXDISTANCE;
    public double Velocity;

    /// <summary>
    ///     Initialize a ray that supports Path Tracing
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Direction"></param>
    /// <param name="Velocity"></param>
    /// <param name="RAYMAXDISTANCE">(Optional) The max probe distance of the ray.</param>
    public PathTracingRay(Vec3 Direction, Vec3 Position, double Velocity = Constants.STEPLENGTH,
        double RAYMAXDISTANCE = Constants.GLOBALRAYMAXDISTANCE, int RAYMAXBOUNCE = Constants.GLOBALRAYMAXBOUNCE)
    {
        this.Position = Position;
        this.Direction = Direction.Normalize();
        this.Velocity = Velocity;
        this.RAYMAXDISTANCE = RAYMAXDISTANCE;
        this.RAYMAXBOUNCE = RAYMAXBOUNCE;
    }

    public void Dispose()
    {
        // GC.SuppressFinalize(this);
    }

    public Vec3 OptimizedEmit(SceneManager sceneManager)
    {
        var RetVec = new Vec3();
        var WholeRay = new Vec3(1d, 1d, 1d);
        var Bounces = 0;
        GeometryObject? LastIntersectObject = null;
        while (Bounces < RAYMAXBOUNCE)
        {
            //find the closest.
            Vec3? IntersectVector = null;
            GeometryObject? IntersectObject = null;
            foreach (var geometryObject in sceneManager.objects)
            {
                if (geometryObject == LastIntersectObject) continue;
                var CurrentIntersectVector = geometryObject.Intersect(Position, Direction);
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
                    }
                }
            }

            if (IntersectVector == null) break;
            LastIntersectObject = IntersectObject;
            Position += IntersectVector;
            var NextDirection =
                IntersectObject.Material.GetOutRayDirection(Direction, IntersectObject.GetNormal(Position));
            //RetVec += IntersectObject.Material.Albedo ^ WholeRay;
            if (IntersectObject.Material.GetType() != typeof(Reflection))
            {
                if (IntersectObject.Material.Emission != null) RetVec += IntersectObject.Material.Emission ^ WholeRay;
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
        var RetVec = new Vec3();
        var WholeRay = new Vec3(1d, 1d, 1d);
        var Bounces = 0;
        while (Distance < RAYMAXDISTANCE && Bounces < RAYMAXBOUNCE)
        {
            Position += Velocity * Direction;
            Distance += Velocity;

            var HitObject = sceneManager.GetHitObject(Position);
            if (HitObject == null) continue;

            var NextDirection = HitObject.Material.GetOutRayDirection(Direction, HitObject.GetNormal(Position));
            if (NextDirection == new Vec3())
            {
                // 光源颜色是否应该用相乘来表示？
                // 是否应该考虑平方反比衰减？
                RetVec += HitObject.Material.Albedo ^ WholeRay;
                break;
            }

            WholeRay ^= HitObject.Material.Albedo;
            Direction = NextDirection;
            Position += Direction;
            Distance += Velocity;
            Bounces++;
        }

        return Operation.ReduceToMaxLimit(RetVec, new Vec3(1, 1, 1));
    }
}