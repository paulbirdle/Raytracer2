namespace Raytracer
{
    class Spotlight : Lightsource
    {
        private readonly Vector position;
        private readonly Vector direction;
        private readonly double angle;

        public Spotlight(Vector position, Vector direction, double angle, RaytracerColor color)
            : base(color)
        {
            this.position = position;
            this.direction = direction;
            this.angle = angle;
        }

        public Spotlight(Vector position, Vector direction, double angle, RaytracerColor color, double intensity)
            : base(color, intensity)
        {
            this.position = position;
            this.direction = direction;
            this.angle = angle;
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

            if (Vector.angle(direction, -v) > angle) return 0;
            return MaxIntensity;
        }

        public override Vector Direction(Vector point)
        {
            return (position - point).normalize();
        }
    }
}
