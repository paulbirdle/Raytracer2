﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Ray
    {
        private Vector direction; //normalized
        private Vector start;

        public Ray(Vector direction, Vector start)
        {
            this.direction = direction.normalize();
            this.start = start;
        }

        public Ray(Vector direction, Vector start, bool normalized)
        {
            if(normalized == true)
            {
                this.direction = direction;
                this.start = start;
            }
            else
            {
                this.direction = direction.normalize();
                this.start = start;
            }
        }

        public Ray()
        {
            start = new Vector();
            direction = new Vector();
        }

        public Vector Direction
        {
            get { return direction; }
        }

        public Vector Start
        {
            get { return start; }
        }

        public Vector position_at_time(double t)
        {
            return start + t * direction;
        }

    }
}
