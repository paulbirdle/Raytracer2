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

            Entity[] theEntities = new Entity[4];
            theEntities[0] = new Sphere(new Vector(20, 0, 0), 1, MirrorRed);
            theEntities[1] = new Sphere(new Vector(19, -2, 1), 1, MattGreen);
            theEntities[2] = new Sphere(new Vector(18.7, 1.5, 1.3), 0.3, RoughYellow);
            theEntities[3] = new Cuboid(new Vector(30, -4, -2), new Vector(0, 0, 1), new Vector(-1, 0, 0), new double[3] { 1, 1.5, 2 }, 
                new Material[6]{
                new Material(RaytracerColor.Pink, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Yellow, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Orange, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Yellow, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Orange, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Pink, 0.7, 100, 0.5, 0.7) });

            Lightsource[] theLights = new Lightsource[1];
            theLights[0] = new PointLight(new Vector(15, 5, 7), RaytracerColor.White);

            return new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
        }

        public static Scene scene2(int resX, int resY)
        {
            Camera theCamera = new Camera(new Vector(-500, -400, 214), new Vector(5, 4, -2), new Vector(1, 1, 4.5), Math.PI / 2.9, resX, resY);

            Entity[] theEntities = new Entity[11];
            Lightsource[] theLights = new Lightsource[3];
            //theLights[0] = new ParallelLight(new Vector(-50, -15, 100)-new Vector(0,0,0), new RaytracerColor(Color.White));
            theLights[1] = new PointLight(new Vector(-40, -90, 40), RaytracerColor.White);
            //theLights[2] = new CandleLight(new Vector(0,-500,1000),300,new RaytracerColor(Color.White));

            int b_s = 50; // background_size
            Material bgm = new Material(new RaytracerColor(Color.White), 0.5, 10, 0, 0.6);
            theEntities[0] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, -b_s, 0), new Vector(b_s, -b_s, 0), new Material(new RaytracerColor(Color.White), 0.8, 10, 0, 0.6)); // "Boden"
            theEntities[1] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(b_s, b_s, b_s), new Vector(b_s, -b_s, b_s), new Vector(b_s, -b_s, 0), bgm); // "rechte Wand"
            theEntities[2] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, b_s, b_s), new Vector(b_s, b_s, b_s), bgm); // "linke Wand"

            double s_s = 6; // sphere_size natuerlich :)
            double dist = 8;
            //int c = 3;
            int c = 0;
            Material materialS = Material.Matte;
            Entity[] group = new Entity[8];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //theEntities[c] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s), s_s, materialS);
                    //theEntities[c + 1] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s + 2 * dist), s_s, materialS);
                    group[c] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s), s_s, materialS);
                    group[c + 1] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s + 2 * dist), s_s, materialS);
                    c += 2;
                }
            }

            double a = s_s + dist;
            Hitbox hitbox = new Hitbox(new Vector(0, 0, a), new Vector(0, 0, 1), new Vector(1, 0, 0), new double[3] { 2*a + 10, 2*a, 2*a });
            double radius = Math.Sqrt(3) * dist + s_s;
            Hitsphere hitsphere = new Hitsphere(new Vector(0, 0, a), radius);
            theEntities[3] = new EntityGroup(group, hitsphere);

            Scene theScene = new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
            return theScene;
        }

        public static Scene scene3(int resX, int resY) // Ball Mirrored Multiple times
        {
            Camera theCamera = new Camera(new Vector(-8,-49,10),new Vector(0,5,0), new Vector(0,0,1), Math.PI/1.5,resX,resY);

            Entity[] theEntities = new Entity[20];
            Lightsource[] theLights = new Lightsource[2];
            int lights = 15;
            int range = 50;
            theLights[0] = new CandleLight(new Vector(-lights-10,-lights,lights), range,new RaytracerColor(Color.FromArgb(255, 50, 50)));
            theLights[1] = new CandleLight(new Vector(lights, -lights, lights), range, new RaytracerColor(Color.FromArgb(50, 50, 255)));

            int b_s = 50; // background_size
            Material bgm = new Material(new RaytracerColor(Color.White), 0, 100, 0.05, 0.7);
            theEntities[0] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, -b_s, 0), new Vector(b_s, -b_s, 0), bgm); // "Boden"

           double mirrorS = 50;
           Material mirror = new Material(new RaytracerColor(Color.White),1,100,0,0.1);
           theEntities[1] = new Quadrilateral(new Vector(mirrorS,mirrorS,mirrorS),new Vector(-mirrorS,mirrorS-4,mirrorS),new Vector(-mirrorS,mirrorS-4,0),new Vector(mirrorS,mirrorS,0), mirror);
           theEntities[2] = new Quadrilateral(new Vector(mirrorS,-mirrorS,mirrorS),new Vector(-mirrorS,-mirrorS,mirrorS),new Vector(-mirrorS,-mirrorS,0),new Vector(mirrorS,-mirrorS,0), mirror);


           theEntities[3] = new Sphere(new Vector(0,0,10),2,new Material(new RaytracerColor(Color.Gold),0.1,20,0.2,0.8));
           theEntities[4] = new Sphere(new Vector(-5,-5,5),1,new Material(new RaytracerColor(Color.DarkRed),0.8,20,0.2,0.8));
           theEntities[5] = new Sphere(new Vector(-15,40,7),1,new Material(new RaytracerColor(Color.Green),0,20,0.2,0.8));
           theEntities[6] = new Sphere(new Vector(-12,7,12),1.2,new Material(new RaytracerColor(Color.Blue),0,100,0.2,0.8));
           theEntities[7] = new Sphere(new Vector(-14,-5,14), 0.3, new Material(new RaytracerColor(Color.Purple), 0.5, 20, 0.2, 0.8));
           theEntities[8] = new Sphere(new Vector(-8, 2, 7), 1, new Material(new RaytracerColor(Color.White), 0.8, 100, 0.1, 0.02));
           theEntities[9] = new Sphere(new Vector(-12, 7, 2), 3, new Material(new RaytracerColor(Color.Orange), 0.1, 100, 0.2, 0.8));
           theEntities[10] = new Sphere(new Vector(-8, 14, 2), 4, new Material(new RaytracerColor(Color.White), 0.1, 100, 0.2, 0.8));
           theEntities[11] = new Sphere(new Vector(2, 20, 2), 3, new Material(new RaytracerColor(Color.Turquoise), 0.1, 100, 0.2, 0.8));
           theEntities[12] = new Sphere(new Vector(-25, 35, 1), 2, new Material(new RaytracerColor(Color.DarkSlateGray), 0.1, 100, 0.2, 0.8));

            Scene theScene = new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
            return theScene;
        }
        public static Scene cylinderS(int resX, int resY)
        {
            Entity[] entities = new Entity[1];
            Lightsource[] l = new Lightsource[1];
            entities[0] = new Cylinder(new Vector(0, 0, -5), new Vector(0, 0, 5), 1);
            l[0] = new PointLight(new Vector(-5, -5, -5), RaytracerColor.White);
            Camera c = new Camera(new Vector(-20, 0, 0), new Vector(1, 0, 0), new Vector(0, 0, 1), Math.PI / 3, resX, resY);

            Scene scene = new Scene(c,entities,l);
            return scene;
        }

        public static Scene scene5(int resX, int resY)
        {
            Camera cam = new Camera(new Vector(100, 0, 0), new Vector(-1, 0, 0), new Vector(0, 0, 1), Math.PI / 4, resX, resY);

            PointLight light = new PointLight(new Vector(30, 20, 40), RaytracerColor.White);
            Lightsource[] lights = new Lightsource[1] { light };

            Torus torus = new Torus(new Vector(0, 0, 0), new Vector(0, 0, 1), 10, 3, new Material(RaytracerColor.Red, 0.5, 100, 0.5, 0.8));
            Sphere sphere = new Sphere(new Vector(-200, 0, 0), 150, new Material(RaytracerColor.Blue, 0.3, 20, 0.5, 0.7));
            Entity[] entities = new Entity[2] { torus, sphere };

            return new Scene(cam, entities, lights);
        }
    }
}
