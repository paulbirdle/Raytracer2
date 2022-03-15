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

            double t;
            Vector x;
            double H;

            double a = Math.Pow(v * dir, 2) * (1 / h2 + 1 / r2) - 1 / r2;
            double b = 2 * (v * dir) * (s * dir) * (1 / h2 + 1 / r2) - 2 * s * v / r2;
            double c = Math.Pow(s * dir, 2) * (1 / h2 + 1 / r2) - s.SquareNorm() / r2;

            if(Math.Abs(a) < 1e-10) t = -c / b; //lineare Gleichung
            else //quadratische Gleichung
            {
                b /= a;
                c /= a;
                double discr = b * b / 4 - c;

                if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
                {
                    n = null;
                    material = null;
                    return -1;
                }
                else //quadratische Gleichung hat Lösung
                {
                    double sqrt = Math.Sqrt(discr);
                    double t1 = -b / 2 - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
                    double t2 = -b / 2 + sqrt;

                    if (t1 < 1e-10 && t2 < 1e-10) //Kegel hinter ray
                    {
                        n = null;
                        material = null;
                        return -1;
                    }
                    else if (t1 < 1e-10 && t2 > 0) //start im Kegel
                    {
                        t = t2;
                    }
                    else t = t1; //Kegel vor ray
                }
            }

            x = s + t * v;
            H = x * dir;
            if (H < 0 || H > h)
            {
                n = null;
                material = null;
                return -1;
            }
            n = (h * (x * dir * dir - x) + r * dir).normalize();
            n *= Math.Sign(-n * v);
            material = this.material;
            return t;
        }

        public override double get_intersection(Ray ray)
        {
            Vector start = ray.Start;
            Vector v = ray.Direction;
            Vector s = start - tip;

            double t;
            Vector x;
            double H;

            double a = Math.Pow(v * dir, 2) * (1 / h2 + 1 / r2) - 1 / r2;
            double b = 2 * (v * dir) * (s * dir) * (1 / h2 + 1 / r2) - 2 * s * v / r2;
            double c = Math.Pow(s * dir, 2) * (1 / h2 + 1 / r2) - s.SquareNorm() / r2;

            if (Math.Abs(a) < 1e-10) t = -c / b; //lineare Gleichung
            else //quadratische Gleichung
            {
                b /= a;
                c /= a;
                double discr = b * b / 4 - c;

                if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
                {
                    return -1;
                }
                else //quadratische Gleichung hat Lösung
                {
                    double sqrt = Math.Sqrt(discr);
                    double t1 = -b / 2 - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
                    double t2 = -b / 2 + sqrt;

                    if (t1 < 1e-10 && t2 < 1e-10) //Kegel hinter ray
                    {
                        return -1;
                    }
                    else if (t1 < 1e-10 && t2 > 0) //start im Kegel
                    {
                        t = t2;
                    }
                    else t = t1; //Kegel vor ray
                }
            }

            x = s + t * v;
            H = x * dir;
            if (H < 0 || H > h)
            {
                return -1;
            }
            return t;
        }
    }
}
