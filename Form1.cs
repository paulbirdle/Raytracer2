using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Raytracer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int scene = (int)SceneSelector.Value;
            int depth = (int)DepthSelector.Value;
            string res = (string)ResolutionSelector.SelectedItem;
            int ssaa = Convert.ToInt32(AAMultiplierSelector.SelectedItem);
            render(scene, depth, res, ssaa);
        }

        private Scene scene1(int resX, int resY)
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

        private Scene scene2(int resX, int resY)
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


        private void render(int scene_to_Render, int depth, string auflösung, int kantenglaettung)
        {
            if (string.IsNullOrEmpty(auflösung))
            {
                throw new ArgumentException($"\"{nameof(auflösung)}\" kann nicht NULL oder leer sein.", nameof(auflösung));
            }

            int resX, resY;

            Dictionary<string, int[]> resDict = new Dictionary<string, int[]>
            {
                {"360p", new int[2]{640, 360} },
                {"720p", new int[2]{1080, 720} },
                {"1080p", new int[2]{1920, 1080} },
                {"1440p", new int[2]{2560, 1440} },
                {"4k", new int[2]{3840, 2160} },
                {"8k", new int[2]{7680, 4320} },
            };
            int[] res = resDict[auflösung];
            resX = res[0];
            resY = res[1];

            int ssaa = kantenglaettung; // Faktor: vielfaches der Ausgaberesolution, also Faktor fuer die Renderaufloesung;
            if (ssaa % 2 != 0 && ssaa != 1)
            {
                throw new Exception("Nur vielfache von 2 oder nur 1 fuer Kantenglättung moeglich!");
            }

            Scene scene;
            if (scene_to_Render == 1)
            {
                scene = scene1(resX * ssaa, resY * ssaa); // höhere Renderauflösung wird übergeben
            }
            else if (scene_to_Render == 2)
            {
                scene = scene2(resX * ssaa, resY * ssaa);
            }
            else
            {
                throw new Exception("Wähl eine existierende Scene aus");
            }

            Bitmap outputBM = new Bitmap(resX, resY);

            Ray.numRay = 0;
            Vector.numVec = 0;
            DateTime before = DateTime.Now;
            RaytracerColor[,] col = scene.render(depth);
            DateTime after = DateTime.Now;

            TimeSpan duration = after - before;
            statistics.Text = "Renderdauer   :  " + duration.TotalSeconds.ToString() + " s";

            before = DateTime.Now;

            double factor = 1.0 / (ssaa * ssaa);

            if (ssaa == 1)
            {
                for (int x = 0; x < resX; x++)
                {
                    for (int y = 0; y < resY; y++)
                    {
                        outputBM.SetPixel(x, y, col[x, y].Col);
                        continue;
                    }
                }
            }
            else
            {
                int actX;
                int actY;
                int r = 0;
                int b = 0;
                int g = 0;

                for (int x = 0; x < resX; x++)
                { 
                    for (int y = 0; y < resY; y++)
                    {
                        //so macht es keine kantigen Farbübergänge... 
                        r = 0;
                        b = 0;
                        g = 0;
                        actX = ssaa * x;
                        actY = ssaa * y;

                        for (int s1 = 0; s1 < ssaa; s1++)
                        {
                            for (int s2 = 0; s2 < ssaa; s2++)
                            {
                                r += col[actX + s1, actY + s2].R;
                                g += col[actX + s1, actY + s2].G;
                                b += col[actX + s1, actY + s2].B;
                            }
                        }
                        r = (int)(factor * r);
                        g = (int)(factor * g);
                        b = (int)(factor * b);

                        outputBM.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
            }

            pictureBox1.Height = pictureBox1.Width * outputBM.Height / outputBM.Width;
            show(outputBM);
            save(outputBM);
            after = DateTime.Now;

            duration = after - before;
            statistics.Text += "\nAnzeigedauer  :  " + duration.TotalSeconds.ToString() + "s";
            statistics.Text += "\nAnzahl Rays    :  " + Ray.numRay.ToString(); 
            statistics.Text += "\nAnzahl Vectors:  " + Vector.numVec.ToString();
        }

        private void show(Bitmap map)
        {
            if (pictureBox1.Image != null) { pictureBox1.Image.Dispose(); }
            using (Image myImage = new Bitmap(map,pictureBox1.Size))
            {
                pictureBox1.Image = (Image)myImage.Clone();
                pictureBox1.Update();
            }
        }
        private void save(Bitmap map)
        {
            map.Save("scene.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene.png");
        }
    }
}
