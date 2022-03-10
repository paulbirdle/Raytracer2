namespace Raytracer
{
    class Lightsource
    {
        public Lightsource(RaytracerColor color)
        {
            Col = color;
            MaxIntensity = 1;
        }

        public Lightsource(RaytracerColor color, double intensity)
        {
            Col = color;
            MaxIntensity = intensity;
        }

        public RaytracerColor Col { get; }

        public double MaxIntensity { get; }

        public virtual Vector Direction(Vector point)//to light
        {
            return new Vector();
        }

        public virtual double Intensity(Vector point, Entity[] entities)
        {
            return MaxIntensity;
        }
    }
}
