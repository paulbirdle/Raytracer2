using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Rectangle : Entity// : Quadrilateral
    {
        protected readonly Vector center;
        protected readonly Vector n;
        protected readonly Vector[] m;
        protected readonly double[] a;
        protected readonly Material material;


        public Rectangle(Vector center, Vector n, Vector m1, double a1, double a2, Material material)
        {
            /*if(Math.Abs(a1-a2) < 1e-3*a1)
            {
                this = new Square();
            }*/
            this.center = center;
            this.n = n.normalize();
            m1 -= m1 * this.n * this.n;
            this.m = new Vector[2] { m1, (n ^ m1).normalize() };
            this.a = new double[2] { Math.Abs(a1), Math.Abs(a2) };
            this.material = material;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
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

            for (int i = 0; i < 2; i++)
            {
                if (Math.Abs((intersection - center) * m[i]) > a[i])
                {
                    n = null;
                    material = null;
                    return -1;
                }
            }

            n = this.n * Math.Sign(-v * this.n);
            material = this.material;
            return t;
        }

        public override double get_intersection(Ray ray)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            double scalprod = v * n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
            {
                return -1;
            }

            double t = (center - x) * n / scalprod;
            if (t < 1e-10)
            {
                return -1;
            }
            Vector intersection = ray.position_at_time(t);

            for (int i = 0; i < 2; i++)
            {
                if (Math.Abs((intersection - center) * m[i]) > a[i])
                {
                    return -1;
                }
            }
            return t;
        }
    }
}
