using System;

namespace Raytracer
{
    class CandleLight : Lightsource
    {
        private readonly Vector position;
        readonly double lengthscale;

        public CandleLight(Vector position, double lengthscale, RaytracerColor color)
            : base(color)
        {
            this.position = position;
            this.lengthscale = lengthscale;
        }

        public override double Intensity(Vector point)
        {
            double distance = (position - point).norm();
            return Math.Min(1, Math.Pow(distance/lengthscale, -2)); //?
        }

        public override Vector Direction(Vector point)
        {
            return (position - point).normalize();
        }

        public override bool is_visible(Vector point, Entity[] entities)
        {
            Vector v = position - point;
            double tmax = v.norm();
            Ray ray = new Ray(v/tmax, point, true);
            double t;
            for (int l = 0; l < entities.Length; l++)
            {
                if(entities[l] == null) continue;
                t = entities[l].get_intersection(ray);
                if (t >= 0 && t < tmax) return false;
            }
            return true;
        }
    }
}
