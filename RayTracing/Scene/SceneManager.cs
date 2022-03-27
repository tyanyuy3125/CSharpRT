using RayTracing.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RayTracing.Assets.Light;
using static RayTracing.Assets.Objects;
using static RayTracing.Math.Types;

namespace RayTracing.Scene
{
    internal class SceneManager
    {
        public List<GeometryObject> objects;
        public List<LightBasis> lights;

        public SceneManager()
        {
            objects = new List<GeometryObject>();
            lights = new List<LightBasis>();
        }

        public void AddObject(GeometryObject geometryObject)
        {
            objects.Add(geometryObject);
        }

        public void AddLight(LightBasis lightBasis)
        {
            lights.Add(lightBasis);
        }

        public bool HitTest(Vec3 coordinate)
        {
            foreach(GeometryObject geometryObject in objects)
            {
                if (geometryObject.GetPointRelation(coordinate) <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public GeometryObject GetHitObject(Vec3 Coordinate)
        {
            foreach (GeometryObject geometryObject in objects)
            {
                if (geometryObject.GetPointRelation(Coordinate) <= 0)
                {
                    return geometryObject;
                }
            }
            return null;
        }
    }
}
