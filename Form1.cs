using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading.Tasks;

//TODO: 
//Cuboid und Hitbox Bug fixen
//Rectangle, Square
//Voraussichtliche Zeit
//GUI
//Erstellen von Szenen einfacher machen
//
//Transparente Objekte mit oder ohne Brechungsindex
//Große Lightsources, weiche Schatten
//
//render gibt gleich Bitmap zurück
//Blur-Material oder generell mal ein Bild unscharf machen
//Möglicherweise bessere Edgedetection
//Progressbar bzw. Msaa anpassen, dass auch da 0-100% durchlaufen werden. Gerade läuft das so 1.5mal durch...
//Dynamische Renderresolution-Anpassung, nach Grad der Kantenstärke z.B. 90p in komplett gleichen Gebieten, 1080 in Übergängen, 8k bei harten Kanten oder so... Keine Ahnung wie man das dann nennt...
//Kantenglättung ohne weiteres Rendern, also lediglich erkennen von Kanten und dann diese irgendwie verrrechnen, dass die Treppeneffekte nicht mehr so sichtbar sind
//
// einzelne bitmaps fuer jeden thread... maybe
// oder nicht immer neue threads aufmachen, koennte viel Zeit brauchen immer neue auf zu machen. also neue Klasse mit Teil von Bild oder so 
//

namespace Raytracer
{
    public partial class Form1 : Form
    {
        int starting_Scene = 12;

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
            if (SceneSelector.Value > SceneContainer.sceneDictionary.Count) return;
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
                 scene = SceneContainer.giveScene(scene_to_Render, resX * a, resY * a);
            }
            else if ((string)aliasingSelector.SelectedItem == "MSAA")
            {
                 scene = SceneContainer.giveScene(scene_to_Render, resX, resY);
            }
            else throw new Exception("Keine Kantenglaettung ausgewaehlt?");

            scene.ProgressChanged += Scene_ProgressChanged;

            Ray.numRay = 0;
            Vector.numVec = 0;
            double[] statistic = new double[3];

            DateTime before = DateTime.Now;

            RaytracerColor[,] col = scene.render(depth);    //unser Bösewicht (was Zeit angeht)

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

        private bool[,] aliasDetection(RaytracerColor[,] col, int resX, int resY)
        {
            double threshhold = 20; // ganz guter Wert I guess; mhhh muss nochmal sehen
            bool[,] edges = new bool[resX, resY];

            Parallel.For(1, resX - 1, x =>
            {
                int minG;
                int maxG;
                int minR;
                int maxR;
                int minB;
                int maxB;

                int value;
                for (int y = 1; y < resY - 1; y++)
                {
                    minG = col[x - 1, y - 1].G;
                    maxG = minG;

                    for (int i = -1; i < 2; i++) // 3x3 Suchfeld
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            value = col[x + i, y + j].G;
                            if (value < minG)
                            {
                                minG = value;
                            }
                            if (value > maxG)
                            {
                                maxG = value;
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
                            value = col[x + i, y + j].R;
                            if (value < minR)
                            {
                                minR = value;
                            }
                            if (value > maxR)
                            {
                                maxR = value;
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
                            value = col[x + i, y + j].B;
                            if ( value < minB)
                            {
                                minB = value;
                            }
                            if (value > maxB)
                            {
                                maxB = value;
                            }
                        }
                    }
                    if (maxB - minB > threshhold)
                    {
                        edges[x, y] = true;
                    }
                }
            });
            return edges;
        }

        private Bitmap showAlias(bool [,] alias)
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
            if ((int)SceneSelector.Value <= SceneContainer.sceneDictionary.Count)
            {
                SceneDescription.Text = "Scene description: ";
                SceneDescription.Text += "\n" + SceneContainer.sceneDictionary[(int)SceneSelector.Value];
            }
            else
            {
                SceneDescription.Text = "Scene description: ";
                SceneDescription.Text += "\nNo Scene " + SceneSelector.Value.ToString() + " exists";
            }
        }


        private void Scene_ProgressChanged(int progress)
        {
            Invoke((Action)delegate
            {
                progressBar1.Value = progress;
            });
        }


    }
}
