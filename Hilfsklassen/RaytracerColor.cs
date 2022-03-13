using System;
using System.Drawing;

namespace Raytracer
{
    class RaytracerColor
    {
        Color color;
        
        public RaytracerColor(Color color)
        {
            this.color = color;
        }

        public RaytracerColor(int r, int g, int b)
        {
            color = Color.FromArgb(Math.Min(r, 255), Math.Min(g, 255), Math.Min(b, 255));
        }

        public Color Col
        {
            get { return color; }
        }

        public int R
        { 
            get { return color.R; } 
        }

        public int G
        {
            get { return color.G; }
        }

        public int B
        {
            get { return color.B; }
        }

        public static RaytracerColor operator *(RaytracerColor x, RaytracerColor y)
        {
            int R = x.R * y.R / 255;
            int G = x.G * y.G / 255;
            int B = x.B * y.B / 255;

            return new RaytracerColor(Color.FromArgb(R, G, B));
        }

        public static RaytracerColor operator +(RaytracerColor x, RaytracerColor y)
        {
            int r = Math.Min(255, x.R + y.R);
            int g = Math.Min(255, x.G + y.G);
            int b = Math.Min(255, x.B + y.B);
            return new RaytracerColor(Color.FromArgb(r, g, b));
        }

        public static RaytracerColor operator *(double a, RaytracerColor x) //0 <= a <= 1
        {
            return new RaytracerColor((int)(a * x.R), (int)(a * x.G), (int)(a * x.B));
        }

        public static RaytracerColor operator *(RaytracerColor x, double a) //0 <= a <= 1
        {
            return a * x;
        }

        public static RaytracerColor weighted_avg(RaytracerColor A, RaytracerColor B, double weight)  // 0.5 entspricht average
        {
            return weight * A + (1 - weight) * B;
        }

        public static Color avg(RaytracerColor[] colors)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int length = colors.Length;
            for ( int i = 0; i< length; i++)
            {
                r += colors[i].R;
                g += colors[i].G;
                b += colors[i].B;
            }
            return Color.FromArgb(r / length, g / length, b / length);
        }

        public static Bitmap convertToBitmap(RaytracerColor[,] col, int resX, int resY, int ssaa)
        {
            Bitmap outputBM = new Bitmap(resX, resY);
            if (ssaa == 1)
            {
                for (int x = 0; x < resX; x++)
                {
                    for (int y = 0; y < resY; y++)
                    {
                        if (col[x, y] == null) throw new Exception("asdlfkj");
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

        public static RaytracerColor Black
        {
            get { return new RaytracerColor(Color.Black); }
        }

        public static RaytracerColor White
        {
            get { return new RaytracerColor(Color.White); }
        }

        public static RaytracerColor Gray
        {
            get { return new RaytracerColor(Color.Gray); }
        }

        public static RaytracerColor Brown
        {
            get { return new RaytracerColor(Color.Brown); }
        }

        public static RaytracerColor Teal
        {
            get { return new RaytracerColor(Color.Teal); }
        }

        public static RaytracerColor Red
        {
            get { return new RaytracerColor(Color.Red); }
        }

        public static RaytracerColor Green
        {
            get { return new RaytracerColor(Color.Green); }
        }

        public static RaytracerColor Blue
        {
            get { return new RaytracerColor(Color.Blue); }
        }

        public static RaytracerColor Yellow
        {
            get { return new RaytracerColor(Color.Yellow); }
        }

        public static RaytracerColor Orange
        {
            get { return new RaytracerColor(Color.Orange); }
        }

        public static RaytracerColor Pink
        {
            get { return new RaytracerColor(Color.Pink); }
        }


        public RaytracerColor darken(double percent)
        {
            return RaytracerColor.weighted_avg(Black, this, percent);
        }

        public RaytracerColor brighten(double percent)
        {
            return RaytracerColor.weighted_avg(White, this, percent);
        }
        

    }
}
