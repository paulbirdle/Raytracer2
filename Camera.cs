using System;

namespace Raytracer
{
    class Camera
    {
        private readonly Vector position;
        private readonly Vector direction;
        private readonly Vector up; // senkrecht auf direction
        private readonly Vector right;
        private readonly double xangle;
        private readonly double yangle;
        private readonly int resx;
        private readonly int resy;

        private readonly Vector ULcorner;
        private readonly Vector step_down;
        private readonly Vector step_right;


        public Camera(Vector position, Vector direction, Vector up, double xangle, int resx, int resy)
        {
            this.direction = direction.normalize();
            this.position = position;
            this.up = up.normalize();
            this.xangle = xangle;
            yangle = 2.0 * Math.Atan(Math.Tan(xangle / 2.0) * resy / resx); 

            this.resx = resx;
            this.resy = resy;

            if(this.direction*this.up != 0)
            {
                this.up = (this.up - (this.up * this.direction) * this.direction).normalize();
            }

            right = this.direction ^ this.up;
            right = right.normalize();

            ULcorner = this.direction + Math.Tan(xangle / 2.0) * (-right) + Math.Tan(yangle / 2.0) * this.up;
            double stepLength = 2.0 * Math.Tan(xangle / 2.0) / resx;
            step_right = stepLength * right;
            step_down = -stepLength * this.up;
        }

        public int resX
        {
            get { return resx; }
        }

        public int resY
        {
            get { return resy; }
        }

        public double xAngle
        {
            get { return xangle; }
        }

        public double yAngle
        {
            get { return yangle; }
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

        public Vector Direction
        {
            get { return direction; }
        }

        public Vector Up
        {
            get { return up; }
        }
    }
}
