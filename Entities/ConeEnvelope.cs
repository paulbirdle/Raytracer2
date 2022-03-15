using System;

namespace Raytracer
{
    class ConeEnvelope : Entity
    {
        private readonly Vector tip;
        private readonly Vector dir;
        private readonly double r;
        private readonly double h;
        private readonly Material material;

        private readonly double h2;
        private readonly double r2;

        public ConeEnvelope(Vector tip, Vector direction, double radius, double height, Material material)
        {
            this.tip = tip;
            dir = direction.normalize();
            r = Math.Abs(radius);
            h = Math.Abs(height);
            this.material = material;

            h2 = h * h;
            r2 = r * r;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector start = ray.Start;
            Vector v = ray.Direction;

            Vector s = start - tip;

            double a = Math.Pow(v * dir, 2) * (1 / h2 + 1 / r2) - 1 / r2;
            double b = 2 * (v * dir) * (s * dir) * (1 / h2 + 1 / r2) - 2 * s * v / r2;
            double c = Math.Pow(s * dir, 2) * (1 / h2 + 1 / r2) - s.SquareNorm() / r2;

            if(Math.Abs(a) < 1e-6)
            {
                double t = -c / b;
                n = -(h * (tip + (s + t * v - tip) * dir * dir - (s + t * v)) + r * dir).normalize();
                material = this.material;
                return t;
            }

            b /= a;
            c /= a;

            double discr = b * b - c;

            if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
            {
                n = null;
                material = null;
                return -1;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b + sqrt;

            if (t1 < 1e-10 && t2 < 1e-10) //Kegel hinter ray
            {
                n = null;
                material = null;
                return -1;
            }
            else if (t1 < 1e-10 && t2 > 0) //start im Kegel
            {
                if((s + t2 * v - tip) * dir > h)
                {
                    n = null;
                    material = null;
                    return -1;
                }
                n = (h * (tip + (s + t2 * v - tip) * dir * dir - (s + t2 * v)) + r * dir).normalize();
                material = this.material;
                return t2;
            }
            //Kegel vor ray
            n = -(h * (tip + (s + t1 * v - tip) * dir * dir - (s + t1 * v)) + r * dir).normalize();
            material = this.material;
            return t1;
        }

        public override double get_intersection(Ray ray)
        {
            Vector start = ray.Start;
            Vector v = ray.Direction;

            Vector s = start - tip;

            double a = Math.Pow(v * dir, 2) * (1 / h2 + 1 / r2) - 1 / r2;
            double b = 2 * (v * dir) * (s * dir) * (1 / h2 + 1 / r2) - 2 * s * v / r2;
            double c = Math.Pow(s * dir, 2) * (1 / h2 + 1 / r2) - s.SquareNorm() / r2;

            if (Math.Abs(a) < 1e-6)
            {
                double t = -c / b;
                return t;
            }

            b /= a;
            c /= a;

            double discr = b * b - c;

            if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
            {
                return -1;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b + sqrt;

            if (t1 < 1e-10 && t2 < 1e-10) //Kegel hinter ray
            {
                return -1;
            }
            else if (t1 < 1e-10 && t2 > 0) //start im Kegel
            {
                if ((s + t2 * v - tip) * dir > h)
                {
                    return -1;
                }
                return t2;
            }
            //Kegel vor ray
            return t1;
        }
    }
}
