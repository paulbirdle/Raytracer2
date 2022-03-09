using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class MovingCameraScene : VideoScene
    {
        private readonly Entity[] entities;
        private readonly Lightsource[] lights;
        private readonly RaytracerColor ambientColor;

        private readonly int n; //Anzahl frames
        //Camera:
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
            if (!(direction.Length == 1 && n == up.Length && n == xangle.Length)) throw new Exception(" ");
        }

        public Scene GetFrame(int k)
        {
            return new Scene(new Camera(position[k], direction[k], up[k], xangle[k], resx, resy), entities, lights);
        }

        public RaytracerColor[,] renderFrame(int k, int depth)
        {
            return GetFrame(k).render(depth);
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

        //public static MovingCameraScene ZoomCam(Scene scene, double zoomFactor, int frames)

    }
}
