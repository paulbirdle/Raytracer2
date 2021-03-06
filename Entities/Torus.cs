using System;

namespace Raytracer
{
    class Torus : Entity //TODO: Hitbox
    {
        private readonly Material material;
        private readonly Vector center;
        private readonly Vector n;
        private readonly double R;
        private readonly double r;

        private readonly Hitsphere hitsphere;

        public Torus(Vector center, Vector n, double R, double r, Material material)
        {
            this.center = center;
            this.n = n.normalize();
            this.R = Math.Abs(R);
            this.r = Math.Abs(r);
            this.material = material;
            hitsphere = new Hitsphere(center, this.R + this.r);
        }

        public override double get_intersection(Ray ray)
        {
            if (!hitsphere.hits(ray)) return -1;
            Vector x = ray.Start;
            Vector v = ray.Direction;

            Vector u = x - center;
            double B = v * n;
            Vector y = u - u * n * n;       //u auf Ebene projiziert
            Vector z = v - B * n;          //v auf Ebene projiziert

            double b_ = 2 * u * v;
            double c_ = u.SquareNorm() + R * R - r * r;

            double a = 2 * b_;
            double b = b_ * b_ + 2 * c_ - 4 * R * R * z.SquareNorm();
            double c = 2 * b_ * c_ - 8 * R * R * y * z;
            double d = c_ * c_ - 4 * R * R * y.SquareNorm();

            return Poly.GetSmallestPositiveRoot(new double[4] { a, b, c, d });
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            if (!hitsphere.hits(ray))
            {
                n = null;
                material = null;
                return -1;
            }
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

            double t = Poly.GetSmallestPositiveRoot(new double[4] { a, b, c, d });

            if(t < 0)
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
