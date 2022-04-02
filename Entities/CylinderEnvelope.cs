using System;

namespace Raytracer
{
    class CylinderEnvelope : Entity
    {
        Vector center;
        Vector n;
        double h;
        double r2;
        Material material;

        public CylinderEnvelope(Vector center, Vector dir, double h, double r, Material material)
        {
            this.center = center;
            n = dir.normalize();
            r2 = r * r;
            this.h = Math.Abs(h);
            this.material = material;
        }

        public CylinderEnvelope(Vector top, Vector bottom, double r, Material material)
        {
            center = 0.5 * (top + bottom);
            n = (top - bottom).normalize();
            h = (top - bottom).norm();
            r2 = r * r;
            this.material = material;
        }

        public override double get_intersection(Ray ray)
        {
            Vector s = ray.Start - center;
            Vector v = ray.Direction;

            double vn = v * n;
            double sn = s * n;

            double a = 1 - vn * vn;
            double b = 2 * (s * v -  sn * vn);
            double c = s * s - sn * sn - r2;

            double t = Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });

            if (t == -1) return -1;

            Vector x = ray.position_at_time(t);
            double he = (x - center) * n;
            if (Math.Abs(he) > h/2) return -1;

            return t;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector s = ray.Start - center;
            Vector v = ray.Direction;

            double vn = v * this.n;
            double sn = s * this.n;

            double a = 1 - vn * vn;
            double b = 2 * (s * v - sn * vn);
            double c = s * s - sn * sn - r2;

            double t = Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });

            Vector x = ray.position_at_time(t);
            double he = (x - center) * this.n;
            if (t == -1 || Math.Abs(he) > h/2)
            {
                n = null;
                material = null;
                return -1;
            }

            material = this.material;
            n = (x - center - he * this.n).normalize();
            return t;
        }
    }
}
