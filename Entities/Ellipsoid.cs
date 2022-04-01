using System;

namespace Raytracer
{
    class Ellipsoid : Entity
    {
        private readonly Vector center;
        private readonly Material material;
        private readonly Matrix B; //realer Raum --> lokales Koordinatensystem

        private readonly Matrix Bt;
        private readonly Matrix C;

        public Ellipsoid(Vector center, double[] a, Vector axis1, Vector axis2, Material material)
        {
            this.center = center;
            axis1 = axis1.normalize();
            axis2 = (axis2 - axis2 * axis1 * axis1).normalize();
            this.material = material;

            Vector axis3 = axis1 ^ axis2;
            B = new Matrix(new double[3, 3] { { axis1.X / a[0], axis1.Y / a[0], axis1.Z / a[0] }, { axis2.X / a[1], axis2.Y / a[1], axis2.Z / a[1] }, { axis3.X / a[2], axis3.Y / a[2], axis3.Z / a[2] } });
            Bt = Matrix.Transpose(B);
            C = Bt * B;
        }


        public Ellipsoid(Vector center, double a1, double a2, double a3, Vector axis1, Vector axis2, Material material)
            : this(center, new double[3] { a1, a2, a3 }, axis1, axis2, material)
        { }


        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector s = ray.Start;
            Vector v = ray.Direction;
            Vector x = s - center;

            x = B * x;
            v = B * v;

            double a = v * v;
            double b = 2 * v * x;
            double c = x * x - 1;

            double t = Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });
            if (t == -1)//nötig?
            {
                n = null;
                material = null;
                return -1;
            }
            else
            {
                n = (C * (ray.position_at_time(t) - center)).normalize();
                n *= Math.Sign(-ray.Direction * n);
                material = this.material;
                return t;
            }
        }

        public override double get_intersection(Ray ray)
        {
            Vector s = ray.Start;
            Vector v = ray.Direction;
            Vector x = s - center;

            x = B * x;
            v = B * v;

            double a = v * v;
            double b = 2 * v * x;
            double c = x * x - 1;

            return Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a });
        }
    }
}
