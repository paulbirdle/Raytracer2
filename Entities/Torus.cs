using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Torus : Entity
    {
        private readonly Material material;
        private readonly Vector center;
        private readonly Vector n;
        private readonly double R;
        private readonly double r;

        public Torus(Vector center, Vector n, double R, double r, Material material)
        {
            this.center = center;
            this.n = n.normalize();
            this.R = Math.Abs(R);
            this.r = Math.Abs(r);
            this.material = material;
        }

        public override double get_intersection(Ray ray)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            Vector u = x - center;
            double B = v * n;
            Vector y = u - u * n * n; //u auf Ebene projiziert
            Vector z = v - B * n;          //v auf Ebene projiziert

            double b_ = 2 * u * v;
            double c_ = u.SquareNorm() + R * R - r * r;

            double a = 2 * b_;
            double b = b_ * b_ + 2 * c_ - 4 * R * R * z.SquareNorm();
            double c = 2 * b_ * c_ - 8 * R * R * y * z;
            double d = c_ * c_ - 4 * R * R * y.SquareNorm();

            Complex[] t_ = Poly.solve_quartic(a, b, c, d);

            double tmin = double.PositiveInfinity;
            Complex t0;
            double t;
            for (int i = 0; i < 4; i++)
            {
                t0 = t_[i];
                if (Math.Abs(t0.Im) < 1e-6)//t0 reell
                {
                    t = t0.Re;
                    if (t > 1e-6 && t < tmin)
                    {
                        tmin = t;
                    }
                }
            }
            t = tmin;

            if (t == double.PositiveInfinity) return -1; //alle Lösungen komplex --> kein Schnittpunkt
            return t;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            Vector u = x - center;
            double B = v * this.n;
            Vector y = u - u * this.n * this.n; //u auf Ebene projiziert
            Vector z = v - B * this.n;          //v auf Ebene projiziert

            double b_ = 2 * u * v;
            double c_ = u.SquareNorm() + R * R - r * r;

            double a = 2 * b_;
            double b = b_ * b_ + 2 * c_ - 4 * R * R * z.SquareNorm();
            double c = 2 * b_ * c_ - 8 * R * R * y * z;
            double d = c_ * c_ - 4 * R * R * y.SquareNorm();

            Complex[] t_ = Poly.solve_quartic(a, b, c, d);

            double tmin = double.PositiveInfinity;
            Complex t0;
            double t;
            for (int i = 0; i < 4; i++)
            {
                t0 = t_[i];
                if (Math.Abs(t0.Im) < 1e-6)//t0 reell
                {
                    t = t0.Re;
                    if (t > 1e-6 && t < tmin)
                    {
                        tmin = t;
                    }
                }
            }
            t = tmin;

            if (t == double.PositiveInfinity)//alle Lösungen komplex --> kein Schnittpunkt
            {
                n = null;
                material = null;
                return -1;
            }

            Vector intersection = ray.position_at_time(t);
            Vector proj = intersection - (intersection - center) * this.n * this.n;
            Vector on_circle = center + R * (proj - center).normalize();

            material = this.material;
            n = (intersection - on_circle).normalize();
            return t;
        }
    }
}
