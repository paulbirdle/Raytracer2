using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
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
            yangle = 2.0 * Math.Atan(Math.Tan(xangle / 2.0) * resy / resx); 

            this.resx = resx;
            this.resy = resy;

            if(direction*up != 0)
            {
                if(this.direction == null)
                {
                    throw new Exception(" ");
                }
                this.up = (this.up - (this.up * this.direction) * this.direction).normalize();
                //throw new Exception("Up falsch");
            }

            right = direction ^ up;
            right = right.normalize();

            if (right == null || -right == null || this.up == null)
            {
                throw new Exception(" ");
            }
            ULcorner = direction + Math.Tan(xangle / 2.0) * (-right) + Math.Tan(yangle / 2.0) * this.up;
            double stepLength = 2.0 * Math.Tan(xangle / 2.0) / resx;
            step_right = stepLength * right;
            step_down = -stepLength * this.up;
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
            if (step_right == null || step_down == null)
            {
                throw new Exception(" ");
            }
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
