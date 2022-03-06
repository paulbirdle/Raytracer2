using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class HitQuadrilateral : Hitentity
    {
        private Vector[] corners;
        private Vector n;

        public HitQuadrilateral(Vector[] corners)
        {
            if (corners.Length != 4)
            {
                throw new Exception("Das ist kein Viereck");
            }
            if (Math.Abs((corners[3] - corners[2]) * n) > 1e-10)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            this.corners = corners;
            n = getNormal();
        }

        public HitQuadrilateral(Vector corner1, Vector corner2, Vector corner3, Vector corner4)
        {
            corners = new Vector[4] { corner1, corner2, corner3, corner4 };
            if (corners[2] == null || corners[3] == null)
            {
                throw new Exception(" ");
            }
            n = getNormal();
            if (Math.Abs((corners[3] - corners[2]) * n) > 1e-10)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            
        }

        public Vector getNormal()
        {
            return ((corners[0] - corners[1]) ^ (corners[0] - corners[2])).normalize();
        }

        public override bool hits(Ray ray)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;
            Vector tri = corners[0];

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
            {
                return false;
            }

            double t = (tri - x) * n / scalprod;
            if (t < 1e-10) return false;
            Vector intersection = ray.position_at_time(t);

            Vector n_side;
            for (int i = 0; i < 4; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                n_side = n ^ (corners[i] - corners[(i + 1) % 4]).normalize(); // zeigt nach außen

                if (((intersection - corners[i]) * n_side) /** (((corners[(i + 2) % 4] - corners[i]) * n_side))*/ > -1e-10)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
