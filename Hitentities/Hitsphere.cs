using System;

namespace Raytracer
{
    class Hitsphere : Hitentity
    {
        private readonly Vector center;
        private readonly double radius;

        private readonly double squarerad;

        public Hitsphere(Vector center, double radius)
        {
            this.center = center;
            this.radius = radius;

            squarerad = radius * radius;
        }

        public override bool hits(Ray ray)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = direction * v_to_start;
            double c = v_to_start * v_to_start - squarerad;

            double discr = b * b - c;

            if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
            {
                return false;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b + sqrt;

            if (t1 < 1e-10 && t2 < 1e-10) //Kugel hinter ray
            {
                return false;
            }
            return true;
        }
    }
}
