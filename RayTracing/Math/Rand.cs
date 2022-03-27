using static RayTracing.Math.Types;

namespace RayTracing.Math;

internal static class Rand
{
    internal static Vec3 RandVec3()
    {
        var ret = new Vec3(Random.Shared.NextDouble() - 0.5d, Random.Shared.NextDouble() - 0.5d, Random.Shared.NextDouble() - 0.5d);
        return ret;
    }
}