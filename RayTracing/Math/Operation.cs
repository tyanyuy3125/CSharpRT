using static RayTracing.Math.Types;

namespace RayTracing.Math;

internal class Operation
{
    public static double Length(Vec3 a)
    {
        return a.length;
    }

    public static Vec3 Normalize(Vec3 a)
    {
        return a.Normalize();
    }

    public static Color AvgColor(Color a, Color b)
    {
        return Color.FromArgb((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);
    }

    public static double ReduceToMaxLimit(double Num, double Maximum)
    {
        if (Num > Maximum) return Maximum;
        return Num;
    }

    public static int ReduceToMaxLimit(int Num, int Maximum)
    {
        if (Num > Maximum) return Maximum;
        return Num;
    }

    public static Vec3 ReduceToMaxLimit(Vec3 Num, Vec3 Maximum)
    {
        var Ret = Num;
        for (var i = 0; i < 3; i++)
        {
            if (Num.x > Maximum.x) Ret.x = Maximum.x;
            if (Num.y > Maximum.y) Ret.y = Maximum.y;
            if (Num.z > Maximum.z) Ret.z = Maximum.z;
        }

        return Ret;
    }

    public static Vec3 Mix(Vec3 x, Vec3 y, double a)
    {
        return (1 - a) * x + a * y;
    }


    /// <summary>
    ///     This function does not check the vec3 inputed.
    /// </summary>
    /// <param name="vec3"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Vec3ToColor(Vec3 vec3)
    {
        vec3 = ReduceToMaxLimit(vec3, new Vec3(1, 1, 1));
        return Color.FromArgb((int)(vec3.x * 255), (int)(vec3.y * 255), (int)(vec3.z * 255));
    }

    public static Vec3 ColorToVec3(Color color)
    {
        return new Vec3(color.R / 255.0, color.G / 255.0, color.B / 255.0);
    }
}