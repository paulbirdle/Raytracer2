using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class PointLight : Lightsource
    {
        private Vector position;

        public PointLight(Vector position, RaytracerColor color)
            :base(color)
        {
            this.position = position;
        }

        public override double Intensity(Vector point)
        {
            return 1.0;
        }

        public override Vector Direction(Vector point)//to lightsource
        {
            return (position - point).normalize();
        }

        public override bool is_visible(Vector point, Entity[] entities)
        {
            Vector v = position - point;
            double tmax = v.norm();
            Ray ray = new Ray(v / tmax, point, true);

            double t;
            for(int l = 0; l < entities.Length; l++)
            {
                if(entities[l] == null) continue;
                t = entities[l].get_intersection(ray);
                if (t >= 0 && t < tmax) return false;
            }
            return true;
        }
    }
}
