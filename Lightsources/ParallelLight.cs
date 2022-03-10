namespace Raytracer
{
    class ParallelLight : Lightsource
    {
        private readonly Vector direction; //to light

        public ParallelLight(Vector direction, RaytracerColor color)
            : base(color)
        {
            this.direction = direction.normalize();
        }

        public ParallelLight(Vector direction, RaytracerColor color, double intensity)
            : base(color, intensity)
        {
            this.direction = direction.normalize();
        }

        public override double Intensity(Vector point, Entity[] entities)
        {
            Ray ray = new Ray(direction, point);
            for (int l = 0; l < entities.Length; l++)
            {
                if (entities[l] == null) continue;
                if (entities[l].get_intersection(ray) > 1e-10) return 0;
            }

            return MaxIntensity;
        }

        public override Vector Direction(Vector point)//to light
        {
            return direction;
        }
    }
}
