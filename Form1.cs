using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

//TODO: 
//vielleicht Cylinder etc. beginnen
//render gibt gleich Bitmap zurück (schwierig mit dem Multithreading)
//Progressbar (auch schwierig mit dem Multithreading)
//Effitienteres Antialiasing z.B. nur bei Kanten in höherer Auflösung Rendern
//Blur-Material oder generel mal ein Bild Unscharf machen
//Transparente Objekte mit oder ohne Brechungsindex
//Große Lightsources, weiche Schatten

//ERLEDIGT:
//Helligkeit für Lightsource
//Spotlight

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
            int scene_selected_at_startup = 3;
            SceneSelector.Value = scene_selected_at_startup;
            displaySceneDescription();
            double[] stats = new double[4] { 0, 0, 0, 0 };
            displayStatistics(stats);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SceneSelector.Value > sceneDictionary.Count) return;
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

            int a = kantenglaettung; // Faktor: vielfaches der Ausgaberesolution, also Faktor fuer die Renderaufloesung;
            if (a % 2 != 0 && a != 1) throw new Exception("Nur vielfache von 2 oder nur 1 fuer Kantenglättung moeglich!");

            Scene scene;
            if ((string)aliasingSelector.SelectedItem == "SSAA")
            {
                 scene = sceneSelection((int)SceneSelector.Value, resX * a, resY * a);
            }
            else if ((string)aliasingSelector.SelectedItem == "MSAA")
            {
                 scene = sceneSelection((int)SceneSelector.Value, resX, resY);
            }
            else throw new Exception("Keine Kantenglaettung ausgewaehlt?");

            Ray.numRay = 0;
            Vector.numVec = 0;
            double[] statistic = new double[3];

            DateTime before = DateTime.Now;


            RaytracerColor[,] col = scene.render(depth);

            DateTime after = DateTime.Now;
            TimeSpan duration = after - before;
            statistic[0] = duration.TotalSeconds;


            before = DateTime.Now;
            Bitmap BM = new Bitmap(resX,resY);

            if ((string)aliasingSelector.SelectedItem == "SSAA")
            {
                BM = convertToBitmap(col, col.GetLength(0) / a, col.GetLength(1) / a, a); // um andere Resolution am Ende z.b fuer Smartphone zuzulassen
            }
            else if ((string)aliasingSelector.SelectedItem == "MSAA")
            {
                if(a == 1)
                {
                    BM = convertToBitmap(col, resX, resY, 1);
                }
                else
                {
                   // BM = showAlias(aliasDetection(col, resX, resY));
                    BM = convertToBitmap(scene.msaa(col, aliasDetection(col, resX, resY), a, depth), resX, resY, 1);
                }
            }
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

        private Scene sceneSelection(int index_of_Scene, int resX,int resY)
        {
            Scene scene;
            if (index_of_Scene == 1)
            {
                return scene = SceneContainer.scene1(resX, resY); // höhere Renderauflösung wird übergeben
            }
            else if (index_of_Scene == 2)
            {
                return scene = SceneContainer.scene2(resX, resY);
            }
            else if (index_of_Scene == 3)
            {
                return scene = SceneContainer.scene3(resX, resY);
            }
            else if (index_of_Scene == 4)
            {
                return scene = SceneContainer.scene4(resX, resY);
            }
            else if (index_of_Scene == 5)
            {
                return scene = SceneContainer.scene5(resX, resY);
            }
            else if (index_of_Scene == 6)
            {
                return scene = SceneContainer.scene6(resX, resY);
            }
            else if (index_of_Scene == 7)
            {
                return scene = SceneContainer.scene7(resX, resY);
            }
            else if (index_of_Scene == 8)
            {
                return scene = SceneContainer.scene8(resX, resY);
            }
            else
            {
                throw new Exception("Wähl eine existierende Scene aus");
            }
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
            {2, "Epic Floor mind. 4min\nACHTUNG DAUERT LANG!"},
            {3, "Infinity Mirror"},
            {4, "Torus and Disk test Scene"},
            {5, "Portal Scene"},
            {6, "completely Random Spheres"},
            {7, "Smartphone render"},
            {8, "General Testing Scene"}
        };

        private void save(Bitmap map)
        {
            map.Save("scene.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene.png");
        }

        private void displayStatistics(double[] stats)
        {
            int rdau = 24 - stats[0].ToString().Length;
            int konv = 24 - stats[1].ToString().Length; 
            int anzd = 24 - stats[2].ToString().Length;
            int anzR = 25 - Ray.numRay.ToString().Length;
            int anzV = 25 - Vector.numVec.ToString().Length;
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

        private Boolean[,] aliasDetection(RaytracerColor[,] col, int resX, int resY)
        {
            double threshhold = 5; // ganz guter Wert I guess; mhhh muss nochmal sehen
            int sizeLayer = 2;  // TODO: für größere Werte implementieren
            Boolean[,] edges = new Boolean[resX, resY];

            for(int x = 0; x < resX - sizeLayer; x++)
            {
                for(int y = 0; y < resY - sizeLayer; y++)
                {
                    edges[x, y] = false;
                    int iG = Math.Min(Math.Min(col[x, y].G, col[x + 1, y].G), Math.Min(col[x + 1, y].G, col[x + 1, y + 1].G));
                    int iGM = Math.Max(Math.Max(col[x, y].G, col[x + 1, y].G), Math.Max(col[x + 1, y].G, col[x + 1, y + 1].G));
                    if(iGM - iG > threshhold)
                    {
                        edges[x, y] = true;
                        continue;
                    }
                    int iR = Math.Min(Math.Min(col[x, y].R, col[x + 1, y].R), Math.Min(col[x + 1, y].R, col[x + 1, y + 1].R));
                    int iRM = Math.Max(Math.Max(col[x, y].R, col[x + 1, y].R), Math.Max(col[x + 1, y].R, col[x + 1, y + 1].R));
                    if (iRM - iR > threshhold)
                    {
                        edges[x, y] = true;
                        continue;
                    }
                    int iB = Math.Min(Math.Min(col[x, y].B, col[x + 1, y].B), Math.Min(col[x + 1, y].B, col[x + 1, y + 1].B));
                    int iBM = Math.Min(Math.Min(col[x, y].B, col[x + 1, y].B), Math.Min(col[x + 1, y].B, col[x + 1, y + 1].B));
                    if (iBM - iB > threshhold)
                    {
                        edges[x, y] = true;
                        continue;
                    }
                }
            }
            return edges;
        }

        private Bitmap showAlias(Boolean [,] alias)
        {
            Bitmap outputBM = new Bitmap(alias.GetLength(0), alias.GetLength(1));
            for(int x = 0; x< alias.GetLength(0); x++)
            {
                for (int y = 0; y < alias.GetLength(1); y++)
                {
                    if(alias[x,y])
                    {
                        outputBM.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        outputBM.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return outputBM;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cd C:\RaytracerVideos\<FolderName>
            //ffmpeg -r 30 -f image2 -s 1080x720 -i img%06d.png -vcodec libx264 -crf 5 -pix_fmt yuv420p scene.mp4


            //MovingCameraScene scene = MovingCameraScene.CircularTrackingShot(SceneContainer.scene1(640, 360), new Vector(20, 0, 0), new Vector(0, 0, 1), Math.PI / 2, 60);
            //scene.SaveFrames(2);

            int[] res = resDictionary["720p"];
            Scene scene = SceneContainer.scene4(res[0], res[1]);
            Vector rotationCenter = new Vector(0, 0, 0);
            Vector axis = new Vector(0, 0, 1);
            double angle = 2 * Math.PI;
            int frames = 480;
            int depth = 3;

            int fps = 15;

            MovingCameraScene videoScene = MovingCameraScene.CircularTrackingShot(scene, rotationCenter, axis, angle, frames);

            int sec = (int)(DateTime.Now - new DateTime(2022, 3, 9)).TotalSeconds;
            string foldername = sec.ToString();
            videoScene.SaveFrames(depth, foldername);

            MessageBox.Show("fertig");

            string ffmpeg1 = @"C:\RaytracerVideos\" + foldername;
            string ffmeg2 = "ffmpeg - r "+ fps.ToString() + " - f image2 - s " + res[0].ToString() + "x" + res[1].ToString() + " - i img % 06d.png - vcodec libx264 - crf 5 - pix_fmt yuv420p scene.mp4";
        }

        private void SceneSelector_ValueChanged(object sender, EventArgs e)
        {
            displaySceneDescription();
        }

        private void displaySceneDescription()
        {
            if ((int)SceneSelector.Value <= sceneDictionary.Count)
            {
                SceneDescription.Text = "Scene description: ";
                SceneDescription.Text += "\n" + sceneDictionary[(int)SceneSelector.Value];
            }
            else
            {
                SceneDescription.Text = "Scene description: ";
                SceneDescription.Text += "\nNo Scene " + SceneSelector.Value.ToString() + " exists";
            }
        }

    }
}
