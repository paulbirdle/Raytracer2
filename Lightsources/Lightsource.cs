namespace Raytracer
{
    class Lightsource
    {
        public Lightsource(RaytracerColor color)
        {
            this.Col = color;
        }

        public RaytracerColor Col { get; }

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
