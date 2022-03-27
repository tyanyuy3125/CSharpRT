using RayTracing.Math;
using RayTracing.Scene;
using static RayTracing.Math.Types;

namespace RayTracing.Assets;

internal class Camera
{
    public static object o = new();
    internal Vec3 direction;
    internal double focus;
    internal Vec2 PortSize;
    internal Vec3 position;

    public Camera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize)
    {
        position = _position;
        direction = _direction.Normalize();
        focus = _focus;
    }

    public virtual void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        for (var x = 0; x < Resolution.x; x++)
            for (var y = 0; y < Resolution.y; y++)
            {
                var x_ = x / Resolution.x * PortSize.x;
                var y_ = y / Resolution.y * PortSize.y;
                var ray = new Ray(
                    LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                    position);
                var pixelColor = ray.Emit(sceneManager);
                output.SetPixel(x, y, pixelColor);
            }
    }

    public virtual void QMCRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        throw new NotImplementedException();
    }
}

internal class DepthBufferCamera : Camera
{
    internal Vec3 direction;
    internal double focus;
    internal Vec3 position;

    public DepthBufferCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position,
        _direction, _focus, PortSize)
    {
        position = _position;
        direction = _direction.Normalize();
        focus = _focus;
        this.PortSize = PortSize;
    }

    public override void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        var Progress = 0;
        /*
        Parallel.For(0, (int)Resolution.x, x =>
        {
            Parallel.For(0, (int)Resolution.y, y =>
            {
                lock (o)
                {
                    Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y, Progress * 100.0 / (Resolution.x * Resolution.y));
                }
                double x_ = (((double)x) / Resolution.x) * PortSize.x;
                double y_ = (((double)y) / Resolution.y) * PortSize.y;
                Ray ray = new Ray(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                int colorval = Convert.ToInt32((ray.ProbeDepth(ref sceneManager) / Constants.DEPTHBUFFER) * 255.0);
                Color pixelColor = Color.FromArgb(colorval, colorval, colorval);
                lock (o)
                {
                    Progress++;
                    output.SetPixel(x, y, pixelColor);
                }
            });
        });
        */
        for (var x = 0; x < (int)Resolution.x; x++)
            for (var y = 0; y < (int)Resolution.y; y++)
            {
                lock (o)
                {
                    Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y,
                        Progress * 100.0 / (Resolution.x * Resolution.y));
                }

                var x_ = x / Resolution.x * PortSize.x;
                var y_ = y / Resolution.y * PortSize.y;
                var ray = new Ray(
                    LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                    position);
                var colorval = Convert.ToInt32(ray.ProbeDepth(ref sceneManager) / Constants.DEPTHBUFFER * 255.0);
                var pixelColor = Color.FromArgb(colorval, colorval, colorval);
                lock (o)
                {
                    Progress++;
                    output.SetPixel(x, y, pixelColor);
                }
            }
    }
}

internal class NormalCamera : Camera
{
    public static object o = new();
    internal Vec3 direction;
    internal double focus;
    internal Vec3 position;

    public NormalCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position, _direction,
        _focus, PortSize)
    {
        position = _position;
        direction = _direction.Normalize();
        focus = _focus;
        this.PortSize = PortSize;
    }

    public override void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        var Progress = 0;
        Parallel.For(0, (int)Resolution.x, x =>
       {
           Parallel.For(0, (int)Resolution.y, y =>
           {
               lock (o)
               {
                   Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y,
                       Progress * 100.0 / (Resolution.x * Resolution.y));
               }

               var x_ = x / Resolution.x * PortSize.x;
               var y_ = y / Resolution.y * PortSize.y;
               var ray = new Ray(
                   LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                   position);
               var NormalVector = ray.ProbeNormal(sceneManager);
               lock (output)
               {
                   Progress++;
                   output.SetPixel(x, y, NormalVector.ToColor());
               }
           });
       });
    }

    public void OptimizedRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        var Progress = 0;
        Parallel.For(0, (int)Resolution.x, x =>
       {
           Parallel.For(0, (int)Resolution.y, y =>
           {
               lock (o)
               {
                   Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y,
                       Progress * 100.0 / (Resolution.x * Resolution.y));
               }

               var x_ = x / Resolution.x * PortSize.x;
               var y_ = y / Resolution.y * PortSize.y;
               var ray = new Ray(
                   LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                   position);
               var NormalVector = ray.OptimizedProbeNormal(sceneManager);
               lock (output)
               {
                   Progress++;
                   output.SetPixel(x, y, NormalVector.ToColor());
               }
           });
       });
        /*
        for (int x = 0; x < (int)Resolution.x; x++)
        {
            for (int y = 0; y < (int)Resolution.y; y++)
            {
                lock (o)
                {
                    Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y, Progress * 100.0 / (Resolution.x * Resolution.y));
                }
                double x_ = (((double)x) / Resolution.x) * PortSize.x;
                double y_ = (((double)y) / Resolution.y) * PortSize.y;
                Ray ray = new Ray(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                Vec3 NormalVector = ray.OptimizedProbeNormal(sceneManager);
                lock (output)
                {
                    Progress++;
                    output.SetPixel(x, y, NormalVector.ToColor());
                }
            }
        }
        */
    }
}

internal class PathTracingCamera : Camera
{
    public static object o = new();
    internal Vec3 direction;
    internal double focus;
    internal Vec3 position;

    public PathTracingCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position,
        _direction, _focus, PortSize)
    {
        position = _position;
        direction = _direction.Normalize();
        focus = _focus;
        this.PortSize = PortSize;
    }

    public override void QMCRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var iIter = new int[(int)Resolution.x, (int)Resolution.y];
        var Iter = 0;
        var ColorVecBuffer = new Vec3[(int)Resolution.x, (int)Resolution.y];
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        while (true)
        {
            Parallel.For(0, 32, i =>
            {
                var x = Convert.ToInt32(Random.Shared.NextInt64(0, (int)Resolution.x));
                var y = Convert.ToInt32(Random.Shared.NextInt64(0, (int)Resolution.y));
                var x_ = x / Resolution.x * PortSize.x;
                var y_ = y / Resolution.y * PortSize.y;

                var ray = new PathTracingRay(
                    LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                    position);
                var pixelColorVec3 = ray.OptimizedEmit(sceneManager);


                lock (output)
                {
                    //pixelColorVec3 = (pixelColorVec3 + Operation.ColorToVec3(output.GetPixel(x, y))) / 2;
                    if (ColorVecBuffer[x, y] == null) ColorVecBuffer[x, y] = new Vec3();
                    //pixelColorVec3 = Operation.Mix(pixelColorVec3, ColorVecBuffer[x,y], 1.0 / (iIter*1.0 + 1.0));
                    pixelColorVec3 = (iIter[x, y] * ColorVecBuffer[x, y] + pixelColorVec3) / (iIter[x, y] + 1.0);
                    //pixelColorVec3 = (pixelColorVec3 + ColorVecBuffer[x, y] ) / 2;
                    var pixelColor = Operation.Vec3ToColor(pixelColorVec3);

                    ColorVecBuffer[x, y] = pixelColorVec3;
                    output.SetPixel(x, y, pixelColor);
                }

                iIter[x, y]++;
            });
            Iter++;
        }
    }

    public override void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
    {
        var LT = focus * direction +
                 0.5 * PortSize.x * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                 0.5 * PortSize.y * new Vec3(0, 0, 1);
        var Progress = 0;

        Parallel.For(0, (int)Resolution.x, x =>
       {
           Parallel.For(0, (int)Resolution.y, y =>
           {
               lock (o)
               {
                   Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y,
                       Progress * 100.0 / (Resolution.x * Resolution.y));
               }

               var x_ = x / Resolution.x * PortSize.x;
               var y_ = y / Resolution.y * PortSize.y;
               var ray = new PathTracingRay(
                   LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1),
                   position);
               var pixelColorVec3 = ray.OptimizedEmit(sceneManager);
               for (var i = 0; i < Constants.GLOBALMAXSAMPLINGCOUNT - 1; i++)
               {
                   ray = new PathTracingRay(
                       LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) -
                       y_ * new Vec3(0, 0, 1), position);
                   pixelColorVec3 = (pixelColorVec3 + ray.OptimizedEmit(sceneManager)) / 2.0;
               }

               var pixelColor = Operation.Vec3ToColor(pixelColorVec3);
               lock (output)
               {
                   Progress++;
                   output.SetPixel(x, y, pixelColor);
               }
           });
       });
        /*
        for (int x = 0; x < (int)Resolution.x; x++)
        {
            for (int y = 0; y < (int)Resolution.y; y++)
            {
                lock (o)
                {
                    Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y, Progress * 100.0 / (Resolution.x * Resolution.y));
                }
                double x_ = (((double)x) / Resolution.x) * PortSize.x;
                double y_ = (((double)y) / Resolution.y) * PortSize.y;
                PathTracingRay ray = new PathTracingRay(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                Color pixelColor = Operation.Vec3ToColor(ray.OptimizedEmit(sceneManager));
                lock (output)
                {
                    Progress++;
                    output.SetPixel(x, y, pixelColor);
                }
            }
        }*/
        /*
        for (int x = 0; x < Resolution.x; x++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                Console.WriteLine("Rendering: {0},{1}", x, y);
                double x_ = (((double)x) / Resolution.x) * PortSize.x;
                double y_ = (((double)y) / Resolution.y) * PortSize.y;
                PathTracingRay ray = new PathTracingRay(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                Color pixelColor = ray.Emit(ref sceneManager);
                output.SetPixel(x, y, pixelColor);
            }
        }
        */
    }
}