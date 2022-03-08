﻿using System;
using System.Drawing;

namespace Raytracer
{
    class SceneContainer
    {
        public static Scene scene1(int resX, int resY) // 3Balls with poly
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
        public static Scene scene2(int resX, int resY) // 8Balls with hitbox
        {
            Camera theCamera = new Camera(new Vector(-200, -160, 70), new Vector(5, 4, -1.2), new Vector(0, 0, 3.5), Math.PI / 4.5, resX, resY);

            Entity[] theEntities = new Entity[11];
            Lightsource[] theLights = new Lightsource[3];
            theLights[1] = new PointLight(new Vector(-40, -90, 40), new RaytracerColor(Color.LightGoldenrodYellow));

            int b_s = 50; // background_size
            Material bgm = new Material(new RaytracerColor(Color.White), 0, 10, 0, 0.6);
            theEntities[0] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0), new Vector(-b_s, -b_s, 0), new Vector(b_s, -b_s, 0), new Material(new RaytracerColor(Color.White), 0, 70, 0.2, 0.8));
            
            double s_s = 6; // sphere_size natuerlich :)
            double dist = 8;
            int c = 0;
            Material materialS = Material.PolishedMetal;
            Entity[] group = new Entity[8];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
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
        public static Scene scene3(int resX, int resY) // Balls Mirrored Multiple times
        {
            Camera theCamera = new Camera(new Vector(-10,-10,10),new Vector(0.5,5,0), new Vector(0,1.2,2), Math.PI/1.45,resX,resY);

            Entity[] theEntities = new Entity[20];
            Lightsource[] theLights = new Lightsource[2];
            int lights = 15;
            int range = 50;
            theLights[0] = new CandleLight(new Vector(-lights-10,-lights,lights), range,new RaytracerColor(Color.Blue));
            theLights[1] = new CandleLight(new Vector(lights, -lights, lights), range, new RaytracerColor(Color.White));

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
        public static Scene scene4(int resX, int resY) // Torus and Disk test Scene
        {
            Camera cam = new Camera(new Vector(100, 0, 0), new Vector(-1, 0, 0), new Vector(0, 0, 1), Math.PI / 4, resX, resY);

            PointLight light = new PointLight(new Vector(30, 20, 40), RaytracerColor.White);
            Lightsource[] lights = new Lightsource[1] { light };

            Torus torus = new Torus(new Vector(0, 0, 0), new Vector(0.7, 0.3, 1), 10, 5, new Material(RaytracerColor.Red, 0.5, 100, 0.5, 0.8));
            Sphere sphere = new Sphere(new Vector(-200, 0, 0), 150, new Material(RaytracerColor.Blue, 0.3, 20, 0.5, 0.7));
            //Disk disk = new Disk(new Vector(0, -20, 0), new Vector(1, 0.5, 0.5), 15, new Material(RaytracerColor.Red, 0.7, 5000, 0.4, 0.7));
            Entity[] entities = new Entity[2] { torus, sphere };

            return new Scene(cam, entities, lights);
        }
        public static Scene scene5(int resX, int resY) // Portal scene
        {
            Scene baseScene = scene3(resX, resY);
            
            double psize = 4;
            double height = 4;
            double left_right = 7;
            double r = 4;
            Quadrilateral portalShape = new Quadrilateral(new Vector(psize - left_right, r,height), new Vector(-psize - left_right , -r, height), new Vector(-psize - left_right, -r, 2*psize + height), new Vector(psize - left_right, r, 2 * psize + height),Material.Matte);
            //Sphere portalShape = new Sphere(new Vector(-6, 7, 9), psize, Material.Iron);
            Portal portal = new Portal(portalShape, baseScene);


            bool portal_set = false;
            Entity[] entities = baseScene.giveEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] is Sphere|| entities[i] is EntityGroup || entities[i] is Portal)
                {
                   entities[i] = null;
                    if(portal_set == false)
                    {
                        entities[i] = portal;
                        portal_set = true;
                    }
                }
            }

            Camera cam= baseScene.giveCamera();
            Lightsource[] lights = baseScene.giveLights();
            Scene scene;
            scene = new Scene(cam, entities, lights);
            return scene;
        }
        public static Scene scene6(int resX, int resY) // Random scene
        {
            Lightsource[] l = standardlights(50, 2, 30);
            Entity[] e = new Entity[40];
            e[0] = standardfloor(10, RaytracerColor.White);
            int gesetzt = 0;
            Random r = new Random();
            while ( gesetzt < e.Length-1)
            {
                for(int i = 0; i<e.Length; i++)
                {
                    if(e[i] is null)
                    {
                        int size = 60;
                        int x = r.Next(-size, size);
                        int y = r.Next(-size, size);
                        int z = r.Next(-size, size);
                        double erg = x ^ 2 + y ^ 2 + z ^ 2;
                        if (Math.Sqrt(erg) < size)
                        {
                            e[i] = new Sphere(new Vector(x / 10, y / 10, z / 10 + size/(10*2)), r.NextDouble(), Material.Random);
                            gesetzt++;
                        }
                    }
                }
            }
            Camera c = standardCamera(Math.PI /12, resX, resY);
            return new Scene(c, e, l);
        }
        public static Scene scene7(int resX, int resY) // Smartphone Hintergrund
        {
            Scene baseScene  = scene3(resX,resY);
            Camera c = baseScene.giveCamera();
            Entity[] e = baseScene.giveEntities();
            Lightsource[] l = baseScene.giveLights();
            RaytracerColor r = baseScene.giveAmbientColor();
            c = new Camera(c.Position, c.Direction, c.Up, c.yAngle, resY, resX); // res tauschen fuer hochkantes Bild
            return new Scene(c, e, l, r); 
        }
        public static Scene scene8(int resX, int resY) // General Testing Scene
        {
            Entity[] e = new Entity[3];
            e[0] = standardfloor(10, RaytracerColor.White);
            e[1] = new Cylinder(new Vector(0, 0, 10), new Vector(3, 3, 2), 3,Material.MetalicRed);
            Lightsource[] l = standardlights(50, 2, 10);
            Camera c = standardCamera(Math.PI / 5, resX, resY);
            return new Scene(c, e, l);
        }

        public static Entity standardfloor(double size, RaytracerColor color)
        {
            Material bgm = new Material(color, 0, 100, 0.05, 0.7);
            return new Quadrilateral(new Vector(size, size, 0), new Vector(-size, size, 0), new Vector(-size, -size, 0), new Vector(size, -size, 0), bgm); // "Boden"
        }
        public static Lightsource[] standardlights(double lengthscale, int anz, double dist_of_ground)
        {
            Lightsource[] theLights = new Lightsource[anz];
            double lights = dist_of_ground;
            double range = lengthscale;
            for(int i = 0; i<anz; i++)
            {
                switch(i)
                {
                    case 0:
                        theLights[0] = new CandleLight(new Vector(-lights, -lights, lights), range, new RaytracerColor(Color.White));
                        break;
                    case 1:
                        theLights[1] = new CandleLight(new Vector(-lights, lights, lights), range, new RaytracerColor(Color.White));
                        break;
                    case 2:
                        theLights[2] = new CandleLight(new Vector(lights, -lights, lights), range, new RaytracerColor(Color.White));
                        break;
                    case 3:
                        theLights[3] = new CandleLight(new Vector(lights, lights, lights), range, new RaytracerColor(Color.White));
                        break;
                }
            }


            return theLights;
        }
        public static Camera standardCamera(double xAngle, int resX,int resY)
        {
            Camera c = new Camera(new Vector(-100, 0, 30), new Vector(100, 0, -27), new Vector(0, 0, 1), xAngle, resX, resY);
            return c;
        }
    }
}
