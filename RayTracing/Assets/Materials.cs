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
    internal class MaterialBasis
    {
        internal Vec3 Albedo;
        internal Vec3 Emission;
        internal virtual Vec3 GetOutRayDirection(Vec3 InRayDirection, Vec3 MeshNormal)
        {
            throw new NotImplementedException();
        }
    }
    internal class ClassicReflection : MaterialBasis
    {
        public Vec3 GetReflectDirection(Vec3 InRay, Vec3 MeshNormal)
        {
            Vec3 OutRay = InRay - ((2 * InRay * MeshNormal) / (Operation.Length(MeshNormal) * Operation.Length(MeshNormal))) * MeshNormal;
            return OutRay;
        }
    }

    internal class ClassicDiffusion : MaterialBasis
    {

    }

    internal class Reflection : MaterialBasis
    {
        public Reflection(Vec3 vec3)
        {
            Albedo = vec3;
        }

        internal override Vec3 GetOutRayDirection(Vec3 InRayDirection, Vec3 MeshNormal)
        {
            Vec3 OutRay = InRayDirection - ((2 * InRayDirection * MeshNormal) / (Operation.Length(MeshNormal) * Operation.Length(MeshNormal))) * MeshNormal;
            return OutRay;
        }
    }

    internal class Diffusion : MaterialBasis
    {
        internal Diffusion(Vec3 Albedo)
        {
            base.Albedo = Albedo;
        }
        internal override Vec3 GetOutRayDirection(Vec3 InRayDirection, Vec3 MeshNormal)
        {
            Vec3 OutRayDirection;
            while (true)
            {
                OutRayDirection = Rand.RandVec3().Normalize();
                if(OutRayDirection * MeshNormal > 0)
                {
                    return OutRayDirection;
                }
            }
        }
    }

    internal class Emission : MaterialBasis
    {
        internal Emission(Vec3 color, Vec3 Emission)
        {
            Albedo = color;
            base.Emission = Emission;
        }
        // 否决：我们假设这是一个理想黑体。
        internal override Vec3 GetOutRayDirection(Vec3 InRayDirection, Vec3 MeshNormal)
        {
            //return new Vec3(0, 0, 0);
            Vec3 OutRayDirection;
            while (true)
            {
                OutRayDirection = Rand.RandVec3().Normalize();
                if (OutRayDirection * MeshNormal > 0)
                {
                    return OutRayDirection;
                }
            }
        }
    }

}
