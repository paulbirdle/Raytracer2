using System;

namespace Raytracer
{
    class Triangle : Entity
    {
        private readonly Vector[] corners;
        private readonly Material material;
        private readonly Vector n;

        private readonly Vector[] n_side; //zeigt immer nach außen
        private readonly Vector edge1, edge2;


        public Triangle(Vector[] corners, Material material)
        {
            if(corners.Length != 3)
            {
                throw new Exception("Das ist kein Dreieck");
            }
            this.corners = corners;
            this.material = material;
            n = getNormal();
            n_side = Get_n_side();
            edge1 = corners[1] - corners[0];
            edge2 = corners[2] - corners[0];
        }

        public Triangle(Vector corner1, Vector corner2, Vector corner3, Material material)
        {
            corners = new Vector[3] { corner1, corner2, corner3 };
            this.material = material;
            n = getNormal();
            n_side = Get_n_side();
            edge1 = corners[1] - corners[0];
            edge2 = corners[2] - corners[0];
        }

        public Vector[] Get_n_side()
        {
            Vector[] res = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                res[i] = n ^ (corners[i] - corners[(i + 1) % 3]).normalize();
            }
            return res;
        }
        public Vector getNormal()
        {
            return ((corners[0] - corners[1]) ^ (corners[0] - corners[2])).normalize();
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector v_ = ray.Direction;
            Vector s_ = ray.Start;

            Vector h, s, q;
            double t, a, f, u, v;
            
            h = v_ ^ edge2;
            a = edge1 * h;
            if (a > -1e-6 && a < 1e-6)//parallel
            {
                t = -1;
            }
            else
            {
                f = 1.0 / a;
                s = s_ - corners[0];
                u = f * s * h;
                if (u < 0.0 || u > 1.0)
                {
                    t = -1;
                }
                else
                {
                    q = s ^ edge1;
                    v = f * v_ * q;
                    if (v < 0.0 || u + v > 1.0)
                    {
                        t = -1;
                    }
                    else
                    {
                        t = f * edge2 * q;
                        if (t < 1e-6) // no ray intersection
                        {
                            t = -1;
                        }
                    }
                }
            }

            if(t == -1)
            {
                n = null;
                material = null;
                return -1;
            }

            material = this.material;
            n = this.n * Math.Sign(-v_ * this.n);
            return t;
        }

        public override double get_intersection(Ray ray)
        {
            Vector v_ = ray.Direction;
            Vector s_ = ray.Start;

            Vector h, s, q;
            double t, a, f, u, v;

            h = v_ ^ edge2;
            a = edge1 * h;
            if (a > -1e-6 && a < 1e-6)//parallel
            {
                return -1;
            }
            else
            {
                f = 1.0 / a;
                s = s_ - corners[0];
                u = f * s * h;
                if (u < 0.0 || u > 1.0)
                {
                    return -1;
                }
                else
                {
                    q = s ^ edge1;
                    v = f * v_ * q;
                    if (v < 0.0 || u + v > 1.0)
                    {
                        return -1;
                    }
                    else
                    {
                        t = f * edge2 * q;
                        if (t < 1e-6) // no ray intersection
                        {
                            return -1;
                        }
                    }
                }
            }

            return t;
        }

        /*public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;
            Vector tri = corners[0];

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Dreieck
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

            for (int i = 0; i < 3; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                if((intersection-corners[i])*n_side[i] > -1e-10)
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
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Dreieck
            {
                return -1;
            }

            double t = (tri - x) * n / scalprod;
            if (t < 1e-10) return -1;
            Vector intersection = x + t * v;

            for (int i = 0; i < 3; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                if ((intersection - corners[i]) * n_side[i] > -1e-10)
                {
                    return -1;
                }
            }

            return t;
        }*/
    }
}
