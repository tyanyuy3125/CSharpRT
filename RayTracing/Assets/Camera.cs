using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Math;
using RayTracing.Scene;
using static RayTracing.Math.Types;

namespace RayTracing.Assets
{
    internal class Camera
    {
        internal Vec3 position;
        internal Vec3 direction;
        internal double focus;
        internal Vec2 PortSize;
        public static object o = new object();
        public Camera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize)
        {
            this.position = _position;
            this.direction = _direction.Normalize();
            this.focus = _focus;
        }

        public virtual async void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            for (int x = 0; x < Resolution.x; x++)
            {
                for (int y = 0; y < Resolution.y; y++)
                {
                    double x_ = (((double)x) / Resolution.x) * PortSize.x;
                    double y_ = (((double)y) / Resolution.y) * PortSize.y;
                    Ray ray = new Ray(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                    Color pixelColor = ray.Emit(sceneManager);
                    output.SetPixel(x, y, pixelColor);
                }
            }
        }

        public virtual async void QMCRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            throw new NotImplementedException();
        }
    }

    internal class DepthBufferCamera : Camera
    {
        internal Vec3 position;
        internal Vec3 direction;
        internal double focus;
        public DepthBufferCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position, _direction, _focus, PortSize)
        {
            this.position = _position;
            this.direction = _direction.Normalize();
            this.focus = _focus;
            this.PortSize = PortSize;
        }

        public override async void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            int Progress = 0;
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
                    int colorval = Convert.ToInt32((ray.ProbeDepth(ref sceneManager) / Constants.DEPTHBUFFER) * 255.0);
                    Color pixelColor = Color.FromArgb(colorval, colorval, colorval);
                    lock (o)
                    {
                        Progress++;
                        output.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }
    }

    internal class NormalCamera : Camera
    {
        internal Vec3 position;
        internal Vec3 direction;
        internal double focus;
        public static object o = new object();
        public NormalCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position, _direction, _focus, PortSize)
        {
            this.position = _position;
            this.direction = _direction.Normalize();
            this.focus = _focus;
            this.PortSize = PortSize;
        }

        public override async void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            int Progress = 0;
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
                    Vec3 NormalVector = ray.ProbeNormal(sceneManager);
                    lock (output)
                    {
                        Progress++;
                        output.SetPixel(x, y, NormalVector.ToColor());
                    }
                });
            });
        }

        public async void OptimizedRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            int Progress = 0;
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
                    Vec3 NormalVector = ray.OptimizedProbeNormal(sceneManager);
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
        internal Vec3 position;
        internal Vec3 direction;
        internal double focus;
        public PathTracingCamera(Vec3 _position, Vec3 _direction, double _focus, Vec2 PortSize) : base(_position, _direction, _focus, PortSize)
        {
            this.position = _position;
            this.direction = _direction.Normalize();
            this.focus = _focus;
            this.PortSize = PortSize;
        }

        public override void QMCRender(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            int[,] iIter = new int[(int)Resolution.x, (int)Resolution.y];
            int Iter = 0;
            Vec3[,] ColorVecBuffer = new Vec3[(int)Resolution.x, (int)Resolution.y];
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            while (true)
            {
                Parallel.For(0, 32, i =>
                {
                    Random rand = new Random();
                    int x = Convert.ToInt32(rand.NextInt64(0, (int)Resolution.x));
                    int y = Convert.ToInt32(rand.NextInt64(0, (int)Resolution.y));
                    double x_ = (((double)x) / Resolution.x) * PortSize.x;
                    double y_ = (((double)y) / Resolution.y) * PortSize.y;

                    PathTracingRay ray = new PathTracingRay(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                    Vec3 pixelColorVec3 = ray.OptimizedEmit(sceneManager);


                    lock (output)
                    {
                        //pixelColorVec3 = (pixelColorVec3 + Operation.ColorToVec3(output.GetPixel(x, y))) / 2;
                        if (ColorVecBuffer[x, y] == null) ColorVecBuffer[x, y] = new Vec3(0, 0, 0);
                        //pixelColorVec3 = Operation.Mix(pixelColorVec3, ColorVecBuffer[x,y], 1.0 / (iIter*1.0 + 1.0));
                        pixelColorVec3 = (iIter[x,y] * ColorVecBuffer[x, y] + pixelColorVec3) / (iIter[x,y] + 1.0);
                        //pixelColorVec3 = (pixelColorVec3 + ColorVecBuffer[x, y] ) / 2;
                        Color pixelColor = Operation.Vec3ToColor(pixelColorVec3);

                        ColorVecBuffer[x, y] = pixelColorVec3;
                        output.SetPixel(x, y, pixelColor);
                    }
                    iIter[x,y]++;
                });
                Iter++;
            }
        }

        public static object o = new object();

        public override void Render(Vec2 Resolution, Bitmap output, SceneManager sceneManager)
        {
            Vec3 LT = focus * direction +
                (0.5 * PortSize.x) * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) +
                (0.5 * PortSize.y) * new Vec3(0, 0, 1);
            int Progress = 0;
            
            Parallel.For(0, (int)Resolution.x, x =>
             {
                 Parallel.For(0, (int)Resolution.y, y =>
                 {
                     lock (o)
                     {
                         Console.WriteLine("Rendering: {0},{1} \t [{2:F}%]", x, y, Progress*100.0/(Resolution.x*Resolution.y));
                     }
                     double x_ = (((double)x) / Resolution.x) * PortSize.x;
                     double y_ = (((double)y) / Resolution.y) * PortSize.y;
                     PathTracingRay ray = new PathTracingRay(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                     Vec3 pixelColorVec3 = ray.OptimizedEmit(sceneManager);
                     for (int i = 0; i < Constants.GLOBALMAXSAMPLINGCOUNT-1; i++)
                     {
                         ray = new PathTracingRay(LT - x_ * Operation.Normalize(Vec3.cross(new Vec3(0, 0, 1), direction)) - y_ * new Vec3(0, 0, 1), position);
                         pixelColorVec3 = (pixelColorVec3 + ray.OptimizedEmit(sceneManager))/2.0;
                     }
                     Color pixelColor = Operation.Vec3ToColor(pixelColorVec3);
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
}
