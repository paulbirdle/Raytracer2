using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer2
{
    class Lightsource
    {
        RaytracerColor color;

        public Lightsource(RaytracerColor color)
        {
            this.color = color;
        }

        public RaytracerColor Col
        {
            get { return color; }
        }

        public virtual Vector Direction(Vector point)//to light
        {
            return new Vector();
        }

        public virtual double Intensity(Vector point)
        {
            return 0;
        }

        public virtual bool is_visible(Vector point, Entity[] entities)
        {
            return false;
        }
    }
}
