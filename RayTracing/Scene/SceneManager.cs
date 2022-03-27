using static RayTracing.Assets.Light;
using static RayTracing.Assets.Objects;
using static RayTracing.Math.Types;

namespace RayTracing.Scene;

internal class SceneManager
{
    public List<LightBasis> lights;
    public List<GeometryObject> objects;

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
        foreach (var geometryObject in objects)
            if (geometryObject.GetPointRelation(coordinate) <= 0)
                return true;
        return false;
    }

    public GeometryObject GetHitObject(Vec3 Coordinate)
    {
        foreach (var geometryObject in objects)
            if (geometryObject.GetPointRelation(Coordinate) <= 0)
                return geometryObject;
        return null;
    }
}