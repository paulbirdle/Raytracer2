using System;
using System.Drawing;
using System.Collections.Generic;


namespace Raytracer
{
    static class SceneContainer
    {
        public static Scene scene1(int resX, int resY) // 3Balls with poly
        {
            Camera theCamera = new Camera(new Vector(0, 0, 0), new Vector(1, 0, 0), new Vector(0, 0, 1), Math.PI / 8, resX, resY);

            Material MirrorRed = new Material(new RaytracerColor(Color.Red), 0.8, 100, 0.5, 0.8);
            Material MattGreen = new Material(new RaytracerColor(Color.Green), 0.3, 30, 0.7, 0.6);
            Material RoughYellow = new Material(new RaytracerColor(Color.Yellow), 0.5, 10, 0.7, 0.3);

            Entity[] theEntities = new Entity[4];
            //theEntities[0] = new Sphere(new Vector(20, 0, 0), 1, MirrorRed);
            //theEntities[0] = new Ellipsoid(new Vector(20, 0, 0), new double[3] { 1, 0.6, 0.4 }, new Vector(0, 1, 1), new Vector(1, 1, -1), MirrorRed);
            theEntities[0] = new Tetrahedron(new Vector[4] { new Vector(20, -1, -1), new Vector(20, 1.5, -0.7), new Vector(20, -0.3, 0.5), new Vector(18, 0.1, -0.3) }, MirrorRed);
            //theEntities[0] = new Cylinder(new Vector(20, 0, 0), new Vector(1, -1, 2), 1.5, 0.5, MirrorRed);
            theEntities[1] = new Sphere(new Vector(19, -2, 1), 1, MattGreen);
            theEntities[2] = new Sphere(new Vector(18.7, 1.5, 1.3), 0.3, RoughYellow);
            /*theEntities[3] = new Cuboid(new Vector(30, -4, -2), new Vector(0, 0, 1), new Vector(-1, 0, 0), new double[3] { 1, 1.5, 2 }, 
                new Material[6]{
                new Material(RaytracerColor.Pink, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Yellow, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Orange, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Yellow, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Orange, 0.7, 100, 0.5, 0.7),
                new Material(RaytracerColor.Pink, 0.7, 100, 0.5, 0.7) });*/

            Lightsource[] theLights = new Lightsource[3];
            theLights[0] = new PointLight(new Vector(15, 5, 7), RaytracerColor.White);
            theLights[1] = new PointLight(new Vector(0, 0, 1), RaytracerColor.White, 0.4);
            theLights[2] = new CandleLight(new Vector(19, 4, -4), 3, RaytracerColor.White);

            return new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
        }
        public static Scene scene2(int resX, int resY) // vertical Bars
        {
            Camera theCamera = new Camera(new Vector(-600, -660, 400), new Vector(6, 6.6, -3.95), new Vector(0, 0, 1), Math.PI/7, resX, resY);
            Entity[] theEntities = new Entity[64];

            Lightsource[] theLights = new Lightsource[2];
           
            double intensity = 28;
            int range = 80;
            double lights = 150;
            theLights[0] = new CandleLight(new Vector(-lights - 70, -lights + 70, lights + 40), range, new RaytracerColor(Color.White), intensity + 5);
            //theLights[1] = new CandleLight(new Vector(-lights + 70, -lights - 70, lights + 40), range, new RaytracerColor(Color.DarkGreen), intensity );
           
            
            //Random r = new Random();
            int amount = 4; 
            if (amount % 2 != 0) throw new Exception("Amount muss gerade sein");
            double size = 100;
             Material floorMaterial = new Material(new RaytracerColor(Color.FromArgb(38, 255, 168)), 0, 20, 0.5, 0.6);
            int maxHeight = 20;
            double s = 21; // spacing: is  subtractive not additive
            int baseSize = 370;
            int offsetUpDown = -400;
            Entity[,] cuboids = new Entity[amount, amount]; // greating Cuboids
            double height = maxHeight;
            for (int i = 0; i<amount; i++)
            {
                for(int j = 0; j<amount; j++)
                {
                    //height = r.Next(1, maxHeight*10);
                    cuboids[i,j] = new Cuboid(new Vector(i * (size/amount),j * (size / amount), (baseSize / 2) + (height / 20)) - new Vector((size - (size/amount))/2, (size - (size / amount)) / 2, -offsetUpDown), new Vector(1, 0, 0), new Vector(0, 0, 1), new double[3] { size / (amount) - s, size/ (amount) -s, (height/10) + baseSize },floorMaterial);
                    //theEntities[i * amount + j] = cuboids[i, j];
                }
            }

            Entity[] groups = new Entity[amount * amount / 4];
            for (int i = 0; i < amount / 2; i++)  //into groups of 4 
            {
                for (int j = 0; j < amount / 2; j++)
                {
                    Entity[] var = new Cuboid[4];
                    var[0] = cuboids[i * 2, j * 2];
                    var[1] = cuboids[i * 2 + 1, j * 2];
                    var[2] = cuboids[i * 2, j * 2 + 1];
                    var[3] = cuboids[i * 2 + 1, j * 2 + 1];
                    Hitentity e = new Hitbox(new Vector(i * (2 * size / amount), j * (2 * size / amount), (baseSize / 2) + (maxHeight / 2)) - new Vector((size - (2 * size / amount)) / 2, (size - (2 * size / amount)) / 2, -offsetUpDown), new Vector(1, 0, 0), new Vector(0, 0, 1), new double[3] { 2 * size / (amount), 2 * size / (amount), maxHeight + baseSize });
                    groups[i * (amount / 2) + j] = new EntityGroup(var, e);
                    theEntities[i * (amount / 2) + j] = groups[i * (amount / 2) + j];
                }
            }

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
            Camera cam = new Camera(new Vector(49, 0, 0), new Vector(-1, 0, 0), new Vector(0, 0, 1), Math.PI / 4, resX, resY);

            PointLight light = new PointLight(new Vector(30, 20, 40), RaytracerColor.White);
            Lightsource[] lights = new Lightsource[1] { light };

            Torus torus = new Torus(new Vector(0, 0, 0), new Vector(0.7, 0.3, 1), 5, 2.5, new Material(RaytracerColor.Red, 0.5, 100, 0.5, 0.8));
            Sphere sphere = new Sphere(new Vector(-200, 0, 0), 150, new Material(RaytracerColor.Blue, 0.3, 20, 0.5, 0.7));
            Cone cone = new Cone(new Vector(0, 0, 7), new Vector(0.2, -0.3, -1), 5, 10, new Material(RaytracerColor.Red, 0.5, 100, 0.5, 0.8));
            Disk disk = new Disk(new Vector(0, -20, 0), new Vector(1, 0.5, 0.5), 15, new Material(RaytracerColor.Red, 0.7, 5000, 0.4, 0.7));
            Triangle triangle = new Triangle(new Vector(0, 0, 7), new Vector(1, 1, -7), new Vector(-2, 10, 0), new Material(RaytracerColor.Red, 0.8, 200, 0.7, 0.4));
            Entity[] entities = new Entity[2] { triangle, sphere };
            

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
            Lightsource[] l = standardlights(50, 2, 30,1);
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
            Entity[] e = new Entity[7];
            Lightsource[] l = new Lightsource[3];
            l[0] = new ParallelLight(new Vector(-1, 0, 0), new RaytracerColor(Color.FromArgb(255, 255, 255)), 0.8);
            //l[1] = new CandleLight(new Vector(-30, -20, -20), 5, new RaytracerColor(Color.FromArgb(255,255,255)), 2.1);

            int size = 10;
            e[0] = new Quadrilateral(new Vector(0, size, size), new Vector(0, size, -size), new Vector(0, -size, -size), new Vector(0, -size, size), new Material(new RaytracerColor(Color.FromArgb(255, 255, 255)), 0, 40, 0.2, 0.8));
            //e[1] = new Triangle(new Vector(0, size, size), new Vector(0, size, -size), new Vector(0, -size, -size), new Material(new RaytracerColor(Color.FromArgb(255, 255, 255)), 0, 40, 0.2, 0.8)); 

            Camera c = new Camera(new Vector(-500, 0, 0), new Vector(100, 0, 0), new Vector(0, 0, 1), Math.PI / 8, resX, resY);
            return new Scene(c, e, l);
        }
        public static Scene scene9(int resX, int resY) // Minimalist flat background
        {
            Entity[] e = new Entity[7];
            Lightsource[] l = new Lightsource[4];
            //l[3] = new ParallelLight(new Vector(-50, 1, 1), new RaytracerColor(Color.FromArgb(255, 255, 255)), 0.05);

            int size = 500;
            RaytracerColor bcol = new RaytracerColor(Color.FromArgb(255,255,255)); 
            RaytracerColor col = new RaytracerColor(Color.FromArgb(255,255,255));

            e[0] = new Quadrilateral(new Vector(0, size, size), new Vector(0, size, -size), new Vector(0, -size, -size), new Vector(0, -size, size), new Material(bcol, 0, 1000, 0, 1));  //Background
           
            int s = 5;


            double stretch = 43;

            //e[1] = new Sphere(new Vector(-0.1, 0, 0), s, new Material(col, 0, 100, 0.4, 1));
            //e[2] = new Sphere(new Vector(-0.1, -stretch, 0), s, new Material(col, 0, 100, 0.4, 1));
            //e[3] = new Sphere(new Vector(-0.1, stretch, 0), s, new Material(col, 0, 100, 0.4, 1));

            //e[1] = new Quadrilateral(new Vector(-0.1, s, s), new Vector(-0.1, s, -s), new Vector(-0.1, -s,-s), new Vector(-0.1, -s, s), new Material(col, 0, 100, 0, 1));
            //e[2] = new Triangle(new Vector(-0.1, 0 + stretch, s - s / 2), new Vector(-0.1, s / 2 + stretch, - s / 2), new Vector(-0.1, -s / 2 + stretch, - s / 2), new Material(col, 0, 100, 0, 1));
            //e[3] = new Disk(new Vector(-0.1, s - stretch, 0),new Vector(-1,0,0), 2, new Material(col, 0, 100, 0, 1));

            l[0] = new Spotlight(new Vector(-80,        0, 0), new Vector(1, 0, 0), Math.PI / 9, new RaytracerColor(Color.FromArgb(0, 255, 0)), 0.9);
            l[1] = new Spotlight(new Vector(-80,  stretch, 0), new Vector(1, 0, 0), Math.PI / 9, new RaytracerColor(Color.FromArgb(255, 0, 0)), 0.9);
            l[2] = new Spotlight(new Vector(-80, -stretch, 0), new Vector(1, 0, 0), Math.PI / 9, new RaytracerColor(Color.FromArgb(0, 0, 255)), 0.9);


            Camera c = new Camera(new Vector(-1000, -1000, -500), new Vector(100, 100, 50), new Vector(0, 0, 1), Math.PI /10, resX, resY);
            return new Scene(c, e, l);



            //opti[1] = new Quadrilateral(new Vector(-20, s, s), new Vector(-20, s, gap), new Vector(-20, 0 + gap, gap), new Vector(-20, gap, s), new Material(col, 0, 100, 0, 1)); // WindowsLogo like shape
            //opti[2] = new Quadrilateral(new Vector(-20, -s, s), new Vector(-20, -s, gap), new Vector(-20, -gap, gap), new Vector(-20, -gap, s), new Material(col, 0, 100, 0, 1));
            //opti[3] = new Quadrilateral(new Vector(-20, -s, -s), new Vector(-20, -s, -gap), new Vector(-20, -gap, -gap), new Vector(-20, -gap, -s), new Material(col, 0, 100, 0, 1));
            //opti[0] = new Quadrilateral(new Vector(-20, s, -s), new Vector(-20, s, -gap), new Vector(-20, gap, -gap), new Vector(-20, gap, -s), new Material(col, 0, 100, 0, 1));
        }
        public static Scene scene10(int resx, int resy) // Smooth shadow scene
        {
            Camera cam = new Camera(new Vector(100, 0, 50), new Vector(-1, 0, -0.5), new Vector(0, 0, 1), Math.PI / 4, resx, resy);
            
            Sphere floor = new Sphere(new Vector(0, 0, -500), 500, new Material(RaytracerColor.White, 0, 100, 0, 1));
            Sphere sphere = new Sphere(new Vector(0, 0, 10), 5, new Material(RaytracerColor.Green, 0.5, 200, 0.5, 0.8));

            Entity[] ents = new Entity[2] { floor, sphere };

            double width = 0;
            int n = 10;
            double h = width / n;
            Lightsource[] lights = new Lightsource[n*n];

            for (int i = 0; i < n; i++)
            { 
                for(int j = 0; j < n; j++)
                {
                    lights[i*n + j] = new PointLight(new Vector(i*h, j*h, 30), RaytracerColor.White, 0.5/Math.Pow(n, 2));
                }
            }

            return new Scene(cam, ents, lights);
        }
        public static Scene scene11(int resX,int resY) // Floor again :)
        {
            Camera theCamera = new Camera(new Vector(-600, -680, 420), new Vector(6, 6.8, -4.15), new Vector(0, 0, 1), Math.PI / 7, resX, resY);
            Entity[] theEntities = new Entity[32*32];

            Lightsource[] theLights = new Lightsource[6];

            double intensity = 5;
            int range = 130;
            double lights = 150;
            theLights[0] = new CandleLight(new Vector(-lights - 200, -lights + 70, lights + 80), range, new RaytracerColor(Color.White), intensity );
            theLights[1] = new CandleLight(new Vector(-lights + 70, -lights - 200, lights + 40), range, new RaytracerColor(Color.DarkGreen), intensity );
            theLights[2] = new CandleLight(new Vector(0, +40, 100), 17, new RaytracerColor(Color.White), intensity +5);
            theLights[3] = new CandleLight(new Vector(0, +300, 100), 17, new RaytracerColor(Color.White), intensity + 5);
            theLights[4] = new CandleLight(new Vector(+220, +80, 100), 17, new RaytracerColor(Color.White), intensity + 5);
            theLights[5] = new CandleLight(new Vector(lights + 70, lights - 200, lights), 45, new RaytracerColor(Color.DarkGreen), intensity +6);

            Random r = new Random();
            int amount = 32;
            if (amount % 2 != 0) throw new Exception("Amount muss gerade sein");
            double size = 800;
            Material floorMaterial = new Material(new RaytracerColor(Color.FromArgb(38, 255, 168)), 0, 20, 0.5, 0.6);
            int maxHeight = 80;
            double s = 0; // spacing: is  subtractive not additive
            int baseSize = 18;
            int offsetUpDown = -40;
            Entity[,] cuboids = new Entity[amount, amount]; // greating Cuboids
            double height = maxHeight;
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    height = r.Next(1, maxHeight*10);
                    cuboids[i, j] = new Cuboid(new Vector(i * (size / amount), j * (size / amount), (baseSize / 2) + (height / 20)) - new Vector((size - (size / amount)) / 2, (size - (size / amount)) / 2, -offsetUpDown), new Vector(1, 0, 0), new Vector(0, 0, 1), new double[3] { size / (amount) - s, size / (amount) - s, (height / 10) + baseSize }, floorMaterial);
                }
            }

            Entity[] groups = new Entity[amount * amount / 4];
            for (int i = 0; i < amount / 2; i++)  //into groups of 4 
            {
                for (int j = 0; j < amount / 2; j++)
                {
                    Entity[] var = new Cuboid[4];
                    var[0] = cuboids[i * 2, j * 2];
                    var[1] = cuboids[i * 2 + 1, j * 2];
                    var[2] = cuboids[i * 2, j * 2 + 1];
                    var[3] = cuboids[i * 2 + 1, j * 2 + 1];
                    Hitentity e = new Hitbox(new Vector(i * (2 * size / amount), j * (2 * size / amount), (baseSize / 2) + (maxHeight / 2)) - new Vector((size - (2 * size / amount)) / 2, (size - (2 * size / amount)) / 2, -offsetUpDown), new Vector(1, 0, 0), new Vector(0, 0, 1), new double[3] { 2 * size / (amount), 2 * size / (amount), maxHeight + baseSize });
                    groups[i * (amount / 2) + j] = new EntityGroup(var, e);
                    theEntities[i * (amount / 2) + j] = groups[i * (amount / 2) + j];
                }
            }

            Scene theScene = new Scene(theCamera, theEntities, theLights, RaytracerColor.Black);
            return theScene;
        }
        public static Entity standardfloor(double size, RaytracerColor color)
        {
            Material bgm = new Material(color, 0, 100, 0.05, 0.7);
            return new Quadrilateral(new Vector(size, size, 0), new Vector(-size, size, 0), new Vector(-size, -size, 0), new Vector(size, -size, 0), bgm); // "Boden"
        }
        public static Lightsource[] standardlights(double lengthscale, int anz, double dist_of_ground, double intensity)
        {
            Lightsource[] theLights = new Lightsource[anz];
            double lights = dist_of_ground;
            double range = lengthscale;
            for(int i = 0; i<anz; i++)
            {
                switch(i)
                {
                    case 0:
                        theLights[0] = new CandleLight(new Vector(-lights, -lights, lights), range, new RaytracerColor(Color.White), intensity);
                        break;
                    case 1:
                        theLights[1] = new CandleLight(new Vector(-lights, lights, lights), range, new RaytracerColor(Color.White), intensity);
                        break;
                    case 2:
                        theLights[2] = new CandleLight(new Vector(lights, -lights, lights), range, new RaytracerColor(Color.White), intensity);
                        break;
                    case 3:
                        theLights[3] = new CandleLight(new Vector(lights, lights, lights), range, new RaytracerColor(Color.White), intensity);
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

        public static Dictionary<int, string> sceneDictionary = new Dictionary<int, string>
        {
            {1, "3 Ball Scene"},
            {2, "vertical Bars"},
            {3, "Infinity Mirror"},
            {4, "Torus and Disk test Scene"},
            {5, "Portal Scene"},
            {6, "completely Random Spheres"},
            {7, "Smartphone render"},
            {8, "General Testing Scene"},
            {9, "Minimalist flat Background"},
            {10, "Smooth Shadow" },
            {11, "Floor" }
        };
        public static Scene giveScene(int index_of_Scene, int resX, int resY)
        {
            if (index_of_Scene == 1)
            {
                return SceneContainer.scene1(resX, resY); 
            }
            else if (index_of_Scene == 2)
            {
                return SceneContainer.scene2(resX, resY);
            }
            else if (index_of_Scene == 3)
            {
                return SceneContainer.scene3(resX, resY);
            }
            else if (index_of_Scene == 4)
            {
                return SceneContainer.scene4(resX, resY);
            }
            else if (index_of_Scene == 5)
            {
                return SceneContainer.scene5(resX, resY);
            }
            else if (index_of_Scene == 6)
            {
                return SceneContainer.scene6(resX, resY);
            }
            else if (index_of_Scene == 7)
            {
                return SceneContainer.scene7(resX, resY);
            }
            else if (index_of_Scene == 8)
            {
                return SceneContainer.scene8(resX, resY);
            }
            else if (index_of_Scene == 9)
            {
                return SceneContainer.scene9(resX, resY);
            }
            else if (index_of_Scene == 10)
            {
                return SceneContainer.scene10(resX, resY);
            }
            else if (index_of_Scene == 11)
            {
                return SceneContainer.scene11(resX, resY);
            }
            else
            {
                throw new Exception("Wähl eine existierende Scene aus");
            }
        }
    }
}
