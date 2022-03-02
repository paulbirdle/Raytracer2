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
            //int resX = 7680; int resY = 4320; //8k
            //int resX = 3840; int resY = 2160; //4K
            //int resX = 1920; int resY = 1080; //FHD
            //int resX = 1080; int resY = 720;  //HD
            //int resX = 640; int resY = 360; //360p
            int resX = 960; int resY = 540; 

            int depth = 5;
            Bitmap flag = new Bitmap(resX, resY);
            Scene scene = scene2(resX, resY);

            DateTime before = DateTime.Now;
            RaytracerColor[,] col = scene.render(depth);
            DateTime after = DateTime.Now;

            TimeSpan duration = after - before;
            label1.Text = duration.TotalSeconds.ToString() + " s";

            before = DateTime.Now;
            for (int i = 0; i < resX; i++)
            {
                for (int j = 0; j < resY; j++)
                {
                    flag.SetPixel(i, j, col[i, j].Col);
                }
            }
            pictureBox1.Image = flag;

            flag.Save("scene1.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene1.png");
            after = DateTime.Now;

            duration = after - before;
            label2.Text = duration.TotalSeconds.ToString() + " s"+ " Rays:" + scene.countout().ToString();
        }

        private Scene scene1(int resX, int resY)
        {
            Camera theCamera = new Camera(new Vector(0, 0, 0), new Vector(1, 0, 0), new Vector(0, 0, 1), Math.PI / 8, resX, resY);

            Material MirrorRed = new Material(new RaytracerColor(Color.Red), 0.8, 100, 0.7, 0.3);
            Material MattGreen = new Material(new RaytracerColor(Color.Green), 0.3, 30, 0.7, 0.3);
            Material RoughYellow = new Material(new RaytracerColor(Color.Yellow), 0, 10, 0.7, 0.3);

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
            Camera theCamera = new Camera(new Vector(-400, -400, 400), new Vector(2, 2, -1.93), new Vector(1,1,2), Math.PI / 6.5, resX, resY);

            Entity[] theEntities = new Entity[20];
            Lightsource[] theLights = new Lightsource[2];
            //theLights[0] = new ParallelLight(new Vector(0, 1, 1), new RaytracerColor(Color.White));
            //theLights[1] = new PointLight(new Vector(-50,-100, 25), RaytracerColor.White);
            theLights[1] = new CandleLight(new Vector(-40,0,30),60,new RaytracerColor(Color.White));

            int b_s = 50; // background_size
            Material bgm = new Material(new RaytracerColor(Color.Red),0,50);
            theEntities[2] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0),  new Vector(-b_s, -b_s, 0),  new Vector(b_s, -b_s, 0),  bgm); // "Boden"
            theEntities[3] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(b_s, b_s, b_s), new Vector(b_s,-b_s, b_s),  new Vector(b_s,-b_s, 0), bgm); // "rechte Wand"
            theEntities[4] = new Quadrilateral(new Vector(b_s, b_s, 0), new Vector(-b_s, b_s, 0),  new Vector(-b_s, b_s, b_s), new Vector(b_s, b_s, b_s), bgm); // "linke Wand"

            int s_s = 6; // sphere_size natuerlich :)
            int dist = 8;
            int c = 5;
            Material materialS = new Material(new RaytracerColor(Color.White),1,40);
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    theEntities[c] = new Sphere(new Vector(Math.Pow(-1,i)*dist, Math.Pow(-1,j)*dist, s_s), s_s, materialS);
                    theEntities[c+1] = new Sphere(new Vector(Math.Pow(-1, i) * dist, Math.Pow(-1, j) * dist, s_s + 2*dist), s_s, materialS);
                    c += 2;
                }
            }

            Scene theScene = new Scene(theCamera, theEntities,theLights, RaytracerColor.Black);
            return theScene;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int resX = 7680; int resY = 4320; //8k
            //int resX = 3840; int resY = 2160; //4K
            //int resX = 1920; int resY = 1080; //FHD
            //int resX = 1080; int resY = 720;  //HD
            //int resX = 640; int resY = 360; //360p

            int depth = 5;
            Bitmap flag = new Bitmap(resX/2, resY/2);
            Scene scene = scene2(resX, resY);

            DateTime before = DateTime.Now;
            RaytracerColor[,] col = scene.render(depth);
            DateTime after = DateTime.Now;

            TimeSpan duration = after - before;
            label1.Text = duration.TotalSeconds.ToString() + " s";

            before = DateTime.Now;
            RaytracerColor avg;

            for (int i = 0; i < resX/2; i++)
            {
                for (int j = 0; j < resY/2; j++)
                {
                    avg = 0.25 * col[2*i, 2*j] + 0.25 * col[2*i + 1, 2 * j] + 0.25 * col[2 * i, 2 * j + 1] + 0.25 * col[2 * i + 1, 2 * j + 1]; 
                    flag.SetPixel(i, j, avg.Col);
                }
            }
            pictureBox1.Image = flag;

            flag.Save("scene1.png", ImageFormat.Png);
            System.Diagnostics.Process.Start("scene1.png");
            after = DateTime.Now;

            duration = after - before;
            label2.Text = duration.TotalSeconds.ToString() + " s" + " Rays:" + scene.countout().ToString();
        }
    }
}
