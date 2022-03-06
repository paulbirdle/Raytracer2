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

        private void save(Bitmap map)
        {
            map.Save("scene.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene.png");
        }

        private void displayStatistics(double[] stats)
        {
            statistics.Text = "Renderdauer   :  " + stats[0].ToString() + " s";
            statistics.Text += "\nBitmapkonvertierung  :  " + stats[1].ToString() + "s";
            statistics.Text += "\nAnzeigedauer  :  " + stats[2].ToString() + "s";
            statistics.Text += "\nAnzahl Rays    :  " + Ray.numRay.ToString();
            statistics.Text += "\nAnzahl Vectors:  " + Vector.numVec.ToString();
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
    }
}
