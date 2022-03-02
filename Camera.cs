using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer2
{
    class Camera
    {
        private Vector position;
        private Vector direction;
        private Vector up; // senkrecht auf direction
        private Vector right;
        private double xangle;
        private double yangle;
        private int resx;
        private int resy;

        private Vector ULcorner;
        private Vector step_down;
        private Vector step_right;


        public Camera(Vector position, Vector direction, Vector up, double xangle, int resx, int resy)
        {
            this.direction = direction.normalize();
            this.position = position;
            this.up = up.normalize();
            this.xangle = xangle;
            this.yangle = 2.0 * Math.Atan(Math.Tan((double)xangle / 2.0) * (double)resy / (double)resx); 

            this.resx = resx;
            this.resy = resy;

            if(direction*up != 0)
            {
                throw new Exception("up ist nicht rechtwinklig zu direction");
            }

            right = direction ^ up;
            right = right.normalize();

            ULcorner = direction + Math.Tan(xangle / 2.0) * (-right) + Math.Tan(yangle / 2.0) * up;
            step_right = (Math.Tan(xangle / 2.0) / (resx / 2.0)) * right;
            step_down = (Math.Tan(yangle / 2.0) / (resy / 2.0)) * (-up);
        }

        public int resX
        {
            get { return resx; }
            set { resx = value; }
        }

        public int resY
        {
            get { return resy; }
            set { resy = value; }
        }

        public double xAngle
        {
            get { return xangle; }
            set { xangle = value; }
        }

        public double yAngle
        {
            get { return yangle; }
            set { yangle = value; }
        }

        public Ray get_Rays(int x, int y)
        {
            Vector v = ULcorner + x * step_right + y * step_down; //Virtuelles Rechteck
            return new Ray(v, position);
        }

        public Vector ULCorner
        {
            get { return ULcorner; }
        }

        public Vector Step_Right
        {
            get { return step_right; }
        }

        public Vector Step_Down
        {
            get { return step_down; }
        }

        public Vector Position
        {
            get { return position; }
        }
    }
}
