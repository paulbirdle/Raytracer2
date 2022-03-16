using System;

namespace Raytracer
{
    class Hitsphere : Hitentity
    {
        private readonly Vector center;
        private readonly double r2;

        public Hitsphere(Vector center, double radius)
        {
            this.center = center;
            r2 = radius * radius;
        }

        public override bool hits(Ray ray)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = direction * v_to_start;
            double c = v_to_start * v_to_start - r2;

            return Poly.GetSmallestPositiveRoot(new double[2] { 2 * b, c }) != -1;
        }
    }
}
