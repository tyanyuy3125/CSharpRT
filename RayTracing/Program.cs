// See https://aka.ms/new-console-template for more information

using RayTracing.Assets;
using RayTracing.Scene;
using RayTracing.Visualizer;
using System.Drawing.Imaging;
using static RayTracing.Math.Types;


var sceneManager = new SceneManager();
sceneManager.AddObject(new Objects.Sphere(new Vec3(0, 0, 47), 44, new Emission(new Vec3(1), new Vec3(1, 1, 1))));
sceneManager.AddObject(new Objects.Sphere(new Vec3(10, 70, 20), 20, new Diffusion(new Vec3(1, 1, 1))));
sceneManager.AddObject(new Objects.Plane(new Vec3(0, 0, 1), new Vec3(), new Diffusion(new Vec3(1, 1, 1))));
//sceneManager.AddObject(new Objects.Sphere(new Vec3(0, 0, -22 - 1000), 1000));
//sceneManager.AddLight(new Light.PointLight(new Vec3(125, 125, 44), 10,13000000.0d));
//sceneManager.AddLight(new Light.PlaneLight(new Vec3(0, -1, -1), 13000000.0d));

#region CornellBox

var CornellBox = new SceneManager();
CornellBox.AddObject(new Objects.Sphere(new Vec3(0, -180, 120), 120, new Diffusion(new Vec3(0.9, 1, 1))));
CornellBox.AddObject(new Objects.Sphere(new Vec3(0, 180, 120), 120, new Reflection(new Vec3(1, 1, 1))));
CornellBox.AddObject(new Objects.Plane(new Vec3(0, 0, 1), new Vec3(), new Diffusion(new Vec3(0.7, 0.7, 0.7))));
CornellBox.AddObject(new Objects.Plane(new Vec3(0, 1), new Vec3(0, -400), new Diffusion(new Vec3(0, 1))));
CornellBox.AddObject(new Objects.Plane(new Vec3(0, -1), new Vec3(0, 400), new Diffusion(new Vec3(1))));
CornellBox.AddObject(new Objects.XYLimitedPlane(new Vec3(0, 0, -1), new Vec3(-65, 0, 700), 300,
    new Emission(new Vec3(1, 0.9, 0.6), 10 * new Vec3(1, 0.9, 0.7))));
CornellBox.AddObject(new Objects.Plane(new Vec3(0, 0, -1), new Vec3(0, 0, 700),
    new Diffusion(new Vec3(0.7, 0.7, 0.7))));

CornellBox.AddObject(new Objects.Plane(new Vec3(1), new Vec3(-250), new Diffusion(new Vec3(0.7, 0.7, 0.7))));
//CornellBox.AddObject(new Objects.Plane(new Vec3(-1, 0, 0), new Vec3(-500, 0, 0), new Diffusion(new Vec3(1, 1, 1))));
Camera CornellBoxCamera = new PathTracingCamera(new Vec3(500, 0, 200), new Vec3(-1, 0, 0.15), 1.7, new Vec2(3.6, 2.4));

#endregion

/*
 * Old Code
Camera camera = new Camera(new Vec3(80, 0, 44), new Vec3(-1, 0, 0), 10);
Vec2 res = new Vec2(400, 300);
Bitmap bitmap = new Bitmap((int)res.x,(int)res.y);
camera.Render(new Vec2(20, 15),res, ref bitmap, ref sceneManager);
 *
 */
var res = new Vec2(800, 600);

Camera pathTracingCamera = new PathTracingCamera(new Vec3(150, 150, 44), new Vec3(-1, -1), 10, new Vec2(36, 24));
var bitmap = new Bitmap((int)res.x, (int)res.y);

var visualizer = new Visualizer(new Size(res.xi, res.yi));

var UIThread = new Task(() => { visualizer.ShowVisualizerWindow(); });

UIThread.Start();

var task = new Task(() =>
{
    while (true)
    {
        Thread.Sleep(200);
        lock (bitmap)
        {
            visualizer.TransmitData(bitmap.Clone() as Bitmap);
        }

        visualizer.UpdatePicture();
    }
});

task.Start();

var RenderTask = new Task(() =>
{
    CornellBoxCamera.QMCRender(res, bitmap, CornellBox);
    //(CornellBoxCamera as NormalCamera).OptimizedRender(res, bitmap, CornellBox);
});

RenderTask.Start();

//(CornellBoxCamera).QMCRender(res, bitmap, CornellBox);

var Path = string.Format("C:\\RenderTest\\{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss"));
if (!Directory.Exists("C:\\RenderTest\\"))
    Directory.CreateDirectory("C:\\RenderTest\\");
lock (bitmap)
{
    bitmap.Save(Path, ImageFormat.Jpeg);
}

Console.WriteLine("Render Completed");

/*
DepthBufferCamera camera = new DepthBufferCamera(new Vec3(80, 0, 44), new Vec3(-1, 0, 0), 10);
Vec2 res = new Vec2(400, 300);
Bitmap bitmap = new Bitmap((int)res.x, (int)res.y);
camera.Render(new Vec2(20, 15), res, ref bitmap, ref sceneManager);
*/


Console.ReadKey();