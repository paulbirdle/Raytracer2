using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class CandleLight : Lightsource
    {
        private Vector position;
        double lengthscale;

        public CandleLight(Vector position, double lengthscale, RaytracerColor color)
            : base(color)
        {
            this.position = position;
            this.lengthscale = lengthscale;
        }

        public override double Intensity(Vector point)
        {
            double distance = (position - point).norm();
            return Math.Min(1, Math.Pow(distance/lengthscale, -2));
        }

        public override Vector Direction(Vector point)
        {
            return (position - point).normalize();
        }

        public override bool is_visible(Vector point, Entity[] entities)
        {
            Ray ray = new Ray(position - point, point);
            double tmax = (position - point).norm();
            double t;
            for (int l = 0; l < entities.Length; l++)
            {
                t = entities[l].get_intersection(ray);
                if (t >= 0 && t < tmax) return false;
            }
            return true;
        }
    }
}
