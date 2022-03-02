using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Quadrilateral : Entity
    {
        private Vector[] corners;
        private Material material;
        private Vector n;


        public Quadrilateral(Vector[] corners, Material material)
        {
            if (corners.Length != 4)
            {
                throw new Exception("Das ist kein Viereck");
            }
            n = getNormal();
            if (Math.Abs((corners[3]-corners[2])*n) > 1e-6)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            this.corners = corners;
        }

        public Quadrilateral(Vector corner1, Vector corner2, Vector corner3, Vector corner4, Material material)
        {
            corners = new Vector[4] { corner1, corner2, corner3, corner4 };
            this.material = material;
            n = getNormal();
            if (Math.Abs((corners[3] - corners[2]) * n) > 1e-6)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
        }

        public Vector getNormal()
        {
            return ((corners[0] - corners[1]) ^ (corners[0] - corners[2])).normalize();
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;
            Vector tri = corners[0];

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-6)//ray parallel zu Viereck
            {
                n = new Vector();
                material = new Material();
                return -1;
            }

            double t = (tri - x) * this.n / scalprod;
            Vector intersection = ray.position_at_time(t);

            Vector n_side;
            for (int i = 0; i < 4; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                n_side = this.n ^ (corners[i] - corners[(i + 1) % 4]).normalize();

                if ((intersection - corners[i]) * n_side * ((corners[(i + 2) % 4] - corners[i]) * n_side) < 1e-6)
                {
                    n = new Vector();
                    material = new Material();
                    return -1;
                }
            }

            n = this.n * Math.Sign(-v * this.n);
            material = this.material;
            return t;
        }
    }
}
