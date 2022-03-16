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
        private readonly double h2r2_;

        public ConeEnvelope(Vector tip, Vector direction, double radius, double height, Material material)
        {
            this.tip = tip;
            dir = direction.normalize();
            r = Math.Abs(radius);
            h = Math.Abs(height);
            this.material = material;

            h2 = h * h;
            r2 = r * r;
            h2r2_ = 1 / h2 + 1 / r2;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector start = ray.Start;
            Vector v = ray.Direction;
            Vector s = start - tip;

            double t;
            Vector x;
            double H;

            double vdir = v * dir;
            double sdir = s * dir;

            double a = vdir * vdir * h2r2_ - 1 / r2;
            double b = 2 * vdir * sdir * h2r2_ - 2 * s * v / r2;
            double c = sdir * sdir * h2r2_ - s.SquareNorm() / r2;

            if (Math.Abs(a) < 1e-10) t = -c / b; //lineare Gleichung
            else //quadratische Gleichung
            {
                t = Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });
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

            double vdir = v * dir;
            double sdir = s * dir;

            double a = vdir * vdir * h2r2_ - 1 / r2;
            double b = 2 * vdir * sdir * h2r2_ - 2 * s * v / r2;
            double c = sdir * sdir * h2r2_ - s.SquareNorm() / r2;

            if (Math.Abs(a) < 1e-10) t = -c / b; //lineare Gleichung
            else //quadratische Gleichung
            {
                t = Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });
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
