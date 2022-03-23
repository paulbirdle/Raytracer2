using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading.Tasks;

//TODO: 
//IMPORTANT: Cuboid und Hitbox haben einen Bug, welcher bei perfekt so darauf schaut, dass sich fordere und hintere Kante überschneiden, dazu führt, dass im gerenderten Bild ein schwarzer Strich auftaucht. 
//Cylinder, Tetrahedron; Quadrilaterals und Cuboid effizienter
//Ellipsoid
//render gibt gleich Bitmap zurück (schwierig mit dem Multithreading)
//Progressbar (auch schwierig mit dem Multithreading)
//Effitienteres Antialiasing z.B. nur bei Kanten in höherer Auflösung Rendern
//Blur-Material oder generel mal ein Bild Unscharf machen
//Transparente Objekte mit oder ohne Brechungsindex
//Große Lightsources, weiche Schatten


namespace Raytracer
{
    public partial class Form1 : Form
    {
        int starting_Scene = 9;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SceneSelector.Value = starting_Scene;
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

            if (checkBox1.Checked == true)
            {
                BM = showAlias(aliasDetection(col, resX, resY));
            }
            else if ((string)aliasingSelector.SelectedItem == "SSAA")
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

        private Scene sceneSelection(int index_of_Scene, int resX, int resY)
        {
            if (index_of_Scene == 1)
            {
                return SceneContainer.scene1(resX, resY); // höhere Renderauflösung wird übergeben
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
            else
            {
                throw new Exception("Wähl eine existierende Scene aus");
            }
        }

        Dictionary<string, int[]> resDictionary = new Dictionary<string, int[]>
        {
            {"90p"  , new int[2]{160,    90} },
            {"180p" , new int[2]{320,   180} },
            {"360p" , new int[2]{640,   360} },
            {"720p" , new int[2]{1280,  720} },
            {"1080p", new int[2]{1920, 1080} },
            {"1440p", new int[2]{2560, 1440} },
            {"4k"   , new int[2]{3840, 2160} },
            {"8k"   , new int[2]{7680, 4320} }
        };

        Dictionary<int, string> sceneDictionary = new Dictionary<int, string>
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
            {10, "Smooth Shadow" }
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
                        if(col[x,y] == null) throw new Exception("Irgendwas mit resX, resY Falsch?");
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
            double threshhold = 3; // ganz guter Wert I guess; mhhh muss nochmal sehen
            bool[,] edges = new bool[resX, resY];


            int minG;
            int maxG; 
            int minR;
            int maxR; 
            int minB;
            int maxB;


            for (int x = 1; x < resX - 1; x++)
            {
                for (int y = 1; y < resY - 1; y++)
                {
                    minG = col[x - 1, y - 1].G;
                    maxG = minG;

                    for (int i = -1; i < 2; i++) // 3x3 Suchfeld
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (col[x + i, y + j].G < minG)
                            {
                                minG = col[x + i, y + j].G;
                            }
                            if (col[x + i, y + j].G > maxG)
                            {
                                maxG = col[x + i, y + j].G;
                            }
                        }
                    }
                    if (maxG - minG > threshhold)
                    {
                        edges[x, y] = true;
                        continue;
                    }


                    minR = col[x - 1, y - 1].R;
                    maxR = minR;

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (col[x + i, y + j].R < minR)
                            {
                                minR = col[x + i, y + j].R;
                            }
                            if (col[x + i, y + j].R > maxR)
                            {
                                maxR = col[x + i, y + j].R;
                            }
                        }
                    }
                    if (maxR - minR > threshhold)
                    {
                        edges[x, y] = true;
                        continue;
                    }

                    minB = col[x - 1, y - 1].B;
                    maxB = minB;

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (col[x + i, y + j].B < minB)
                            {
                                minB = col[x + i, y + j].B;
                            }
                            if (col[x + i, y + j].B > maxB)
                            {
                                maxB = col[x + i, y + j].B;
                            }
                        }
                    }
                    if (maxB - minB > threshhold)
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
