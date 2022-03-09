using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Raytracer
{
    class MovingCameraScene : VideoScene
    {
        //Scene verändert sich nicht
        private readonly Entity[] entities;
        private readonly Lightsource[] lights;
        private readonly RaytracerColor ambientColor;

        private readonly int n; //Anzahl frames

        //Camera verändert sich (bis auf Auflösung):
        private readonly Vector[] position;     //changing in time
        private readonly Vector[] direction;
        private readonly Vector[] up;
        private readonly double[] xangle;            
        private readonly int resx;              //constant in time
        private readonly int resy;

        public MovingCameraScene(Entity[] entities, Lightsource[] lights, RaytracerColor ambientColor, Vector[] position, Vector[] direction, Vector[] up, double[] xangle, int resx, int resy)
        {
            n = position.Length;
            this.entities = entities;
            this.lights = lights;
            this.ambientColor = ambientColor;
            this.position = position;
            this.direction = direction;
            this.up = up;
            this.xangle = xangle;
            this.resx = resx;
            this.resy = resy;
            if (!(direction.Length == n && n == up.Length && n == xangle.Length)) throw new Exception(" ");
        }

        public Scene GetFrame(int k)
        {
            return new Scene(new Camera(position[k], direction[k], up[k], xangle[k], resx, resy), entities, lights, ambientColor);
        }

        public RaytracerColor[,] RenderFrame(int k, int depth)
        {
            return GetFrame(k).render(depth);
        }

        public void SaveFrames(int depth, string foldername__)
        {
            string folderName = @"C:\RaytracerVideos";
            
            string globalPathString = System.IO.Path.Combine(folderName, foldername__);// DateTime.Now.Date.ToString());
            System.IO.Directory.CreateDirectory(globalPathString);
            string pathString;

            for(int k = 0; k < n; k++)
            {
                string filename = "img" + k.ToString("D6") + ".png";
                pathString = System.IO.Path.Combine(globalPathString, filename);
                Bitmap bm = RaytracerColor.convertToBitmap(RenderFrame(k, depth), resx, resy, 1);
                bm.Save(pathString, ImageFormat.Png);
            }
        }


        public static MovingCameraScene RotateCam(Scene scene, Vector axis, double angle, int frames)
        {
            Camera cam = scene.giveCamera();
            Vector campos = cam.Position;
            Vector camdir = cam.Direction;
            Vector camup = cam.Up;
            int resx = cam.resX;
            int resy = cam.resY;
            double xang = cam.xAngle;

            Vector[] position = new Vector[frames];
            Vector[] direction = new Vector[frames];
            Vector[] up = new Vector[frames];
            double[] xangle = new double[frames];

            Matrix Rotate = Matrix.Rotation(axis, angle / frames);
            for(int i = 0; i < frames; i++)
            {
                position[i] = campos;
                xangle[i] = xang;

                direction[i] = camdir;
                up[i] = camup;

                camdir = Rotate * camdir;
                camup = Rotate * camup;
            }

            return new MovingCameraScene(scene.giveEntities(), scene.giveLights(), scene.giveAmbientColor(), position, direction, up, xangle, resx, resy);
        }

        public static MovingCameraScene ZoomCam(Scene scene, double zoomFactor, int frames) //zoomFactor > 1 : reinzoomen, < 1: rauszoomen
        {
            Camera cam = scene.giveCamera();
            Vector campos = cam.Position;
            Vector camdir = cam.Direction;
            Vector camup = cam.Up;
            int resx = cam.resX;
            int resy = cam.resY;
            double xang = cam.xAngle;

            Vector[] position = new Vector[frames];
            Vector[] direction = new Vector[frames];
            Vector[] up = new Vector[frames];
            double[] xangle = new double[frames];

            double SingleZoomFactor = Math.Pow(zoomFactor, 1 / frames);

            for (int i = 0; i < frames; i++)
            {
                position[i] = campos;
                direction[i] = camdir;
                up[i] = camup;

                xangle[i] = xang;
                xang /= SingleZoomFactor;
            }

            return new MovingCameraScene(scene.giveEntities(), scene.giveLights(), scene.giveAmbientColor(), position, direction, up, xangle, resx, resy);
        }

        public static MovingCameraScene CircularTrackingShot(Scene scene, Vector rotationCenter, Vector axis, double angle, int frames)
        {
            Camera cam = scene.giveCamera();
            Vector campos = cam.Position;
            Vector camdir = cam.Direction;
            Vector camup = cam.Up;
            int resx = cam.resX;
            int resy = cam.resY;
            double xang = cam.xAngle;

            Vector[] position = new Vector[frames];
            Vector[] direction = new Vector[frames];
            Vector[] up = new Vector[frames];
            double[] xangle = new double[frames];

            Matrix Rotate = Matrix.Rotation(axis, angle / frames);

            for (int i = 0; i < frames; i++)
            {
                xangle[i] = xang;

                position[i] = campos;
                direction[i] = camdir;
                up[i] = camup;

                campos = Rotate * (campos - rotationCenter) + rotationCenter;
                camdir = Rotate * camdir;
                camup = Rotate * camup;
            }

            return new MovingCameraScene(scene.giveEntities(), scene.giveLights(), scene.giveAmbientColor(), position, direction, up, xangle, resx, resy);
        }

        public static MovingCameraScene LinearTrackingShot(Scene scene, Vector movingDirection, double length, int frames)
        {
            Camera cam = scene.giveCamera();
            Vector campos = cam.Position;
            Vector camdir = cam.Direction;
            Vector camup = cam.Up;
            int resx = cam.resX;
            int resy = cam.resY;
            double xang = cam.xAngle;

            Vector[] position = new Vector[frames];
            Vector[] direction = new Vector[frames];
            Vector[] up = new Vector[frames];
            double[] xangle = new double[frames];

            Vector translation = (length / frames) * movingDirection;

            for (int i = 0; i < frames; i++)
            {
                xangle[i] = xang;

                position[i] = campos;
                direction[i] = camdir;
                up[i] = camup;

                campos += translation;
            }

            return new MovingCameraScene(scene.giveEntities(), scene.giveLights(), scene.giveAmbientColor(), position, direction, up, xangle, resx, resy);
        }

        public static MovingCameraScene StaticShot(Scene scene, int frames)
        {
            Camera cam = scene.giveCamera();
            Vector campos = cam.Position;
            Vector camdir = cam.Direction;
            Vector camup = cam.Up;
            int resx = cam.resX;
            int resy = cam.resY;
            double xang = cam.xAngle;

            Vector[] position = new Vector[frames];
            Vector[] direction = new Vector[frames];
            Vector[] up = new Vector[frames];
            double[] xangle = new double[frames];

            for (int i = 0; i < frames; i++)
            {
                xangle[i] = xang;
                position[i] = campos;
                direction[i] = camdir;
                up[i] = camup;
            }

            return new MovingCameraScene(scene.giveEntities(), scene.giveLights(), scene.giveAmbientColor(), position, direction, up, xangle, resx, resy);
        }

    }
}
