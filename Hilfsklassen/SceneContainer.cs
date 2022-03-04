using System;
using System.Drawing;

namespace Raytracer
{
    class SceneContainer
    {

        public static Scene scene1(int resX, int resY)
        {
            Camera theCamera = new Camera(new Vector(0, 0, 0), new Vector(1, 0, 0), new Vector(0, 0, 1), Math.PI / 8, resX, resY);

            Material MirrorRed = new Material(new RaytracerColor(Color.Red), 0.8, 100, 0.5, 0.8);
            Material MattGreen = new Material(new RaytracerColor(Color.Green), 0.3, 30, 0.7, 0.6);
            Material RoughYellow = new Material(new RaytracerColor(Color.Yellow), 0.5, 10, 0.7, 0.3);

            Entity[] theEntities = new Entity[3];
            theEntities[0] = new Sphere(new Vector(20, 0, 0), 1, MirrorRed);
            theEntities[1] = new Sphere(new Vector(19, -2, 1), 1, MattGreen);
            theEntities[2] = new Sphere(new Vector(18.7, 1.5, 1.3), 0.3, RoughYellow);

            Lightsource[] theLights = new Lightsource[1];
            theLights[0] = new PointLight(new Vector(15, 5, 7), RaytracerColor.White);

            return new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
        }

        public static Scene scene2(int resX, int resY)
        {
            Camera theCamera = new Camera(new Vector(-500, -400, 200), new Vector(5, 4, -1.85), new Vector(1, 1, 4.5), Math.PI / 2.9, resX, resY);

            Entity[] theEntities = new Entity[20];
            Lightsource[] theLights = new Lightsource[4];
            //theLights[0] = new ParallelLight(new Vector(-50, -15, 100)-new Vector(0,0,0), new RaytracerColor(Color.White));
            theLights[1] = new PointLight(new Vector(-40, -90, 40), RaytracerColor.White);
            //theLights[2] = new CandleLight(new Vector(0,-500,1000),300,new RaytracerColor(Color.White));

            int b_s = 50; // background_size
            Material bgm = new Material(new RaytracerColor(Color.White), 0, 10, 0, 0);
            theEntities[2] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, -b_s, 0), new Vector(b_s, -b_s, 0), new Material(new RaytracerColor(Color.White), 0.8, 10, 0, 0.6)); // "Boden"
            theEntities[3] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(b_s, b_s, b_s), new Vector(b_s, -b_s, b_s), new Vector(b_s, -b_s, 0), bgm); // "rechte Wand"
            theEntities[4] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, b_s, b_s), new Vector(b_s, b_s, b_s), bgm); // "linke Wand"

            int s_s = 6; // sphere_size natuerlich :)
            int dist = 8;
            int c = 5;
            Material materialS = Material.Matte;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    theEntities[c] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s), s_s, materialS);
                    theEntities[c + 1] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s + 2 * dist), s_s, materialS);
                    c += 2;
                }
            }

            Scene theScene = new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
            return theScene;
        }
    }
}
