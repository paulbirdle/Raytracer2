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
            Vector s = ray.Start;
            Vector v = ray.Direction;

            double a = s * n;
            double b = v * n;
            Vector u = s - a * n - center;
            Vector w = v - b * n;
            double A = b * b + w * w;
            double B = 2 * u * w + 2 * a * b;
            double C = u*u + R * R - r * r + a * a;

            //double aa = A * A;
            double aa = 2 * B / A;
            double bb = B * B / A*A + 2 * C/A - 4 * R * R * w * w/A*A;
            double cc = 2 * B * C/A*A - 4 * R * R * 2 * u * w/A*A;
            double dd = C * C/A*A - 4 * R * R * u * u/A*A;

            Complex[] z = Poly.solve_quartic(aa,bb,cc,dd);

            //x = s + t*v
            //(x*n)^2 + (norm((x-x*n*n)-center)-R)^2 = r^2

            //r^2 = (a + t*b)^2 + ||u||^2 + 2*t*u*w + t^2*||w||^2-2R||u+t*w|| + R^2

            //2R||u+t*w||   = (a + t*b)^2 + ||u||^2 + 2*t*u*w + t^2*||w||^2 + R^2 - r^2
            //              = ||u||^2+R^2-r^2+a^2 + t*(2*u*w+2*a*b) + t^2*(b^2+||w||^2)
            //              = C + Bt + At^2
            //4R^2(||u||^2 + 2*t*u*w + t^2*||w||^2) = C^2 + t*(2*B*C) + t^2*(B^2 + 2*A*C) + t^3(2*A*B) + t^4(A^2)
            //<==>  0 = aat^4 + bbt^3 + cct^2 + ddt + ee
        }
    }
}
