using System;

namespace Raytracer
{
    class Disk : Entity
    {
        private readonly Material material;
        private readonly Vector center;
        private readonly Vector n;
        private readonly double radius;

        public Disk(Vector center, Vector n, double radius, Material material)
        {
            this.material = material;
            this.center = center;
            this.n = n.normalize();
            this.radius = radius;
        }

        public override double get_intersection(Ray ray)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            double scalprod = v * n;
            if (Math.Abs(scalprod) < 1e-10)
            {
                return -1;
            }

            double t = (center - x) * n / scalprod;
            if (t < 1e-10)
            {
                return -1;
            }
            Vector intersection = ray.position_at_time(t);

            if((intersection - center).norm() > radius)
            {
                return -1;
            }

            return t;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)
            {
                n = null;
                material = null;
                return -1;
            }

            double t = (center - x) * this.n / scalprod;
            if (t < 1e-10)
            {
                n = null;
                material = null;
                return -1;
            }
            Vector intersection = ray.position_at_time(t);

            if ((intersection - center).norm() > radius)
            {
                n = null;
                material = null;
                return -1;
            }

            material = this.material;
            n = this.n * Math.Sign(-v * this.n);
            return t;
        }
    }
}
