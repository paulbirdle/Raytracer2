﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Timers;

//TODO: 
//seltsames Rauschen im Bild beheben, liegt an Multithreading
//vielleicht Cylinder etc. beginnen
//Hitboxen für Scene3
//render gibt gleich Bitmap zurück (schwierig mit dem Multithreading)
//Progressbar
//

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
            int scene_displayed_at_startup = 6;
            SceneSelector.Value = scene_displayed_at_startup;
            double[] stats = new double[4] { 0, 0, 0, 0 };
            displayStatistics(stats);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int scene = (int)SceneSelector.Value;
            int depth = (int)DepthSelector.Value;
            string res = (string)ResolutionSelector.SelectedItem;
            int ssaa = Convert.ToInt32(AAMultiplierSelector.SelectedItem);
            render(scene, depth, res, ssaa);
        }

        private void render(int scene_to_Render, int depth, string auflösung, int kantenglaettung)
        {
            if (string.IsNullOrEmpty(auflösung)) throw new ArgumentException($"\"{nameof(auflösung)}\" kann nicht NULL oder leer sein.", nameof(auflösung));

            int[] res = resDictionary[auflösung];
            int resX = res[0];
            int resY = res[1];

            int ssaa = kantenglaettung; // Faktor: vielfaches der Ausgaberesolution, also Faktor fuer die Renderaufloesung;
            if (ssaa % 2 != 0 && ssaa != 1) throw new Exception("Nur vielfache von 2 oder nur 1 fuer Kantenglättung moeglich!");


            Scene scene;
            if (scene_to_Render == 1)
            {
                scene = SceneContainer.scene1(resX * ssaa, resY * ssaa); // höhere Renderauflösung wird übergeben
            }
            else if (scene_to_Render == 2)
            {
                scene = SceneContainer.scene2(resX * ssaa, resY * ssaa);
            }
            else if (scene_to_Render == 3)
            {
                scene = SceneContainer.scene3(resX * ssaa, resY * ssaa);
            }
            else if (scene_to_Render == 4)
            {
                scene = SceneContainer.scene4(resX * ssaa, resY * ssaa);
            }
            else if(scene_to_Render == 5)
            {
                scene = SceneContainer.scene5(resX * ssaa, resY * ssaa);
            }
            else if (scene_to_Render == 6)
            {
                scene = SceneContainer.scene6(resX * ssaa, resY * ssaa);
            }
            else
            {
                throw new Exception("Wähl eine existierende Scene aus");
            }

            Ray.numRay = 0;
            Vector.numVec = 0;
            double[] statistic = new double[3];

            DateTime before = DateTime.Now;
            RaytracerColor[,] col = scene.render(depth);
            //Bitmap BM = scene.renderBM(depth);
            DateTime after = DateTime.Now;
            TimeSpan duration = after - before;
            statistic[0] = duration.TotalSeconds;

            before = DateTime.Now;
            Bitmap BM = convertToBitmap(col, resX, resY, ssaa);
            after = DateTime.Now;
            duration = after - before;
            statistic[1] = duration.TotalSeconds;
            //statistic[1] = 0;

            before = DateTime.Now;
            save(BM);
            after = DateTime.Now;
            duration = after - before;
            statistic[2] = duration.TotalSeconds;
            
            displayStatistics(statistic);
        }

        Dictionary<string, int[]> resDictionary = new Dictionary<string, int[]>
        {
            {"360p", new int[2]{640, 360} },
            {"720p", new int[2]{1080, 720} },
            {"1080p", new int[2]{1920, 1080} },
            {"1440p", new int[2]{2560, 1440} },
            {"4k", new int[2]{3840, 2160} },
            {"8k", new int[2]{7680, 4320} },
        };
        Dictionary<int, string> sceneDictionary = new Dictionary<int, string>
        {
            {1, "3 Ball Scene"},
            {2, "8 Ball Scene with entityGroup"},
            {3, "Infinity Mirror"},
            {4, "Torus and Disk test Scene"},
            {5, "Portal Scene"},
            {6, "completely Random Spheres"},
        };

        private void save(Bitmap map)
        {
            map.Save("scene.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene.png");
        }

        private void displayStatistics(double[] stats)
        {
            int rdau = 20 - stats[0].ToString().Length;
            int konv = 20 - stats[1].ToString().Length; 
            int anzd = 20 - stats[2].ToString().Length;
            int anzR = 21 - Ray.numRay.ToString().Length;
            int anzV = 21 - Vector.numVec.ToString().Length;
            statistics.Text = "Renderdauer               :  " + stats[0].ToString().PadLeft(rdau) + " s";
            statistics.Text += "\n\nBitmapkonvertierung  :  " + stats[1].ToString().PadLeft(konv) + "s";
            statistics.Text += "\n\nAnzeigedauer               :  " + stats[2].ToString().PadLeft(anzd) + "s";
            statistics.Text += "\n\nAnzahl Rays                  :  " + Ray.numRay.ToString().PadLeft(anzR);
            statistics.Text += "\n\nAnzahl Vectors              :  " + Vector.numVec.ToString().PadLeft(anzV);
        }

        private Bitmap convertToBitmap(RaytracerColor[,] col, int resX, int resY, int ssaa)
        {
            Bitmap outputBM = new Bitmap(resX,resY);
            if (ssaa == 1)
            {
                for (int x = 0; x < resX; x++)
                {
                    for (int y = 0; y < resY; y++)
                    {
                        if(col[x,y] == null) throw new Exception("asdlfkj");
                        outputBM.SetPixel(x, y, col[x, y].Col);
                        continue;
                    }
                }
            }
            else
            {
                int actX;
                int actY;
                RaytracerColor[] c = new RaytracerColor[ssaa * ssaa]; // in dieses array werden die Farben eines Blocks reingeschrieben, um sie dann als avg auf die BM zu schreiben
                int i;
                int j;

                for (int x = 0; x < resX; x++)
                {
                    for (int y = 0; y < resY; y++)
                    {
                        actX = ssaa * x;
                        actY = ssaa * y;
                        for (int s1 = 0; s1 < ssaa; s1++)
                        {
                            for (int s2 = 0; s2 < ssaa; s2++)
                            {
                                i = actX + s1;
                                j = actY + s2;
                                c[ssaa * s1 + s2] = col[i, j];
                            }
                        }
                        outputBM.SetPixel(x, y, RaytracerColor.avg(c));
                    }
                }
            }
            return outputBM;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Scene scene = SceneContainer.scene4(640, 360);
            Torus torus = (Torus)(scene.giveEntities()[0]);

            Vector start = scene.giveCamera().Position;
            Vector end = torus.Center + 0*torus.rr*torus.N;
            Ray ray = new Ray(end-start, start);

            double t = torus.get_intersection(ray);
        }

        private void SceneSelector_ValueChanged(object sender, EventArgs e)
        {
            SceneDescription.Text = "Scene description: ";
            SceneDescription.Text += "\n"+ sceneDictionary[(int)SceneSelector.Value];
        }
    }
}
