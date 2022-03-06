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
        private readonly Vector[] corners;
        private readonly Material material;
        private readonly Vector n;

        private readonly Vector[] n_side; //zeigt immer nach außen


        public Quadrilateral(Vector[] corners, Material material)
        {
            if (corners.Length != 4)
            {
                throw new Exception("Das ist kein Viereck");
            }
            if (Math.Abs((corners[3]-corners[2])*n) > 1e-10)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            this.corners = corners;
            this.material = material;
            n = GetNormal();
            n_side = Get_n_side();
        }

        public Quadrilateral(Vector corner1, Vector corner2, Vector corner3, Vector corner4, Material material)
        {
            corners = new Vector[4] { corner1, corner2, corner3, corner4 };
            this.material = material;
            n = GetNormal();
            if (Math.Abs((corners[3] - corners[2]) * n) > 1e-10)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            n_side = Get_n_side();
        }

        public Vector[] Get_n_side()
        {
            Vector[] res = new Vector[4];
            for(int i = 0; i < 4; i++)
            {
                res[i] = n ^ (corners[i] - corners[(i + 1) % 4]).normalize();
            }
            return res;
        }

        private Vector GetNormal()
        {
            return ((corners[0] - corners[1]) ^ (corners[0] - corners[2])).normalize();
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;
            Vector tri = corners[0];

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
            {
                n = null;
                material = null;
                return -1;
            }

            double t = (tri - x) * this.n / scalprod;
            if(t < 1e-10)
            {
                n = null;
                material = null;
                return -1;
            }
            Vector intersection = ray.position_at_time(t);

            for (int i = 0; i < 4; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                if ((intersection - corners[i]) * n_side[i] > -1e-10)
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
            Vector tri = corners[0];

            double scalprod = v * n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
            {
                return -1;
            }

            double t = (tri - x) * n / scalprod;
            if (t < 1e-10) return -1;
            Vector intersection = ray.position_at_time(t);

            for (int i = 0; i < 4; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                if ((intersection - corners[i]) * n_side[i] > -1e-10)
                {
                    return -1;
                }
            }

            return t;
        }
    }
}
