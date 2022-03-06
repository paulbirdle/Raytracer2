using System;

namespace Raytracer
{
    class HitQuadrilateral : Hitentity
    {
        private readonly Vector[] corners;
        private readonly Vector n;

        private readonly Vector[] n_side; //zeigt immer nach außen

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
            n_side = Get_n_side();
        }

        public HitQuadrilateral(Vector corner1, Vector corner2, Vector corner3, Vector corner4)
        {
            corners = new Vector[4] { corner1, corner2, corner3, corner4 };
            n = getNormal();
            n_side = Get_n_side();
            if (Math.Abs((corners[3] - corners[2]) * n) > 1e-10)
            {
                throw new Exception("Die Ecken liegen nicht in einer Ebene");
            }
            
        }

        public Vector[] Get_n_side()
        {
            Vector[] res = new Vector[4];
            for (int i = 0; i < 4; i++)
            {
                res[i] = n ^ (corners[i] - corners[(i + 1) % 4]).normalize();
            }
            return res;
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

            double scalprod = v * n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Viereck
            {
                return false;
            }

            double t = (tri - x) * n / scalprod;
            if (t < 1e-10) return false;
            Vector intersection = ray.position_at_time(t);

            for (int i = 0; i < 4; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                if ((intersection - corners[i]) * n_side[i] /** (((corners[(i + 2) % 4] - corners[i]) * n_side))*/ > -1e-10)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
