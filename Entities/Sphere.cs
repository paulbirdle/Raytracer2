using System;

namespace Raytracer
{
    class Sphere : Entity
    {
        private readonly double radius;
        private readonly Vector center;
        private readonly Material material;

        private readonly double squarerad;

        public Sphere(Vector center, double radius, Material material) 
        {
            this.radius = Math.Abs(radius);
            this.center = center;
            this.material = material;

            squarerad = radius * radius;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = 2 * direction * v_to_start;
            double c = v_to_start * v_to_start - squarerad;

            double t = Poly.GetSmallestPositiveRoot(new double[2] { b, c });
            if(t == -1)
            {
                n = null;
                material = null;
                return -1;
            }
            else
            {
                n = (ray.position_at_time(t) - center).normalize();
                n *= Math.Sign(-direction * n);
                material = this.material;
                return t;
            }
        }

        public override double get_intersection(Ray ray)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = 2 * direction * v_to_start;
            double c = v_to_start * v_to_start - squarerad;

            return Poly.GetSmallestPositiveRoot(new double[2] { b, c });
        }
    }
}
