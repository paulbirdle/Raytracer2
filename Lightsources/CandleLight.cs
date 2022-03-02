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

        public CandleLight(Vector position, RaytracerColor color)
            : base(color)
        {
            this.position = position;
        }

        public override double Intensity(Vector point)
        {
            double distance = (position - point).norm();
            return Math.Min(1 / Math.Pow(distance, 2), 1);
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
                if(entities[l] == null) continue;
                t = entities[l].get_intersection(ray, out _, out _);
                if (t >= 0 && t < tmax) return false;
            }
            return true;
        }
    }
}
