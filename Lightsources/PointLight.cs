namespace Raytracer
{
    class PointLight : Lightsource
    {
        private readonly Vector position;

        public PointLight(Vector position, RaytracerColor color)
            :base(color)
        {
            this.position = position;
        }

        public PointLight(Vector position, RaytracerColor color, double intensity)
            : base(color, intensity)
        {
            this.position = position;
        }

        public override double Intensity(Vector point, Entity[] entities)
        {
            Vector v = position - point;
            double tmax = v.norm();
            Ray ray = new Ray(v / tmax, point, true);

            double t;
            for (int l = 0; l < entities.Length; l++)
            {
                if (entities[l] == null) continue;
                t = entities[l].get_intersection(ray);
                if (t >= 0 && t < tmax) return 0;
            }
            return MaxIntensity;
        }

        public override Vector Direction(Vector point)//to lightsource
        {
            return (position - point).normalize();
        }
    }
}
