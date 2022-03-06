using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Triangle : Entity
    {
        private Vector[] corners;
        private Material material;
        private Vector n;
        

        public Triangle(Vector[] corners, Material material)
        {
            if(corners.Length != 3)
            {
                throw new Exception("Das ist kein Dreieck");
            }
            this.corners = corners;
            this.material = material;
            n = getNormal();
        }

        public Triangle(Vector corner1, Vector corner2, Vector corner3, Material material)
        {
            corners = new Vector[3] { corner1, corner2, corner3 };
            this.material = material;
            n = getNormal();
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
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Dreieck
            {
                n = null;
                material = null;
                return -1;
            }

            double t = (tri - x) * this.n / scalprod;
            if (v == null)
            {
                throw new Exception(" ");
            }
            Vector intersection = x + t * v;

            Vector n_side;
            for (int i = 0; i < 3; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                n_side = this.n ^ (corners[i] - corners[(i + 1) % 3]).normalize();

                if((intersection-corners[i])*n_side*((corners[(i + 2) % 3] - corners[i])*n_side) < 1e-10)
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

            double scalprod = v * this.n;
            if (Math.Abs(scalprod) < 1e-10)//ray parallel zu Dreieck
            {
                return -1;
            }

            double t = (tri - x) * this.n / scalprod;
            if (v == null)
            {
                throw new Exception(" ");
            }
            Vector intersection = x + t * v;

            Vector n_side;
            for (int i = 0; i < 3; i++)
            {   //checke ob intersection auf der richtigen Seite von corners[i] - corners[i+1] liegt:
                n_side = this.n ^ (corners[i] - corners[(i + 1) % 3]).normalize();

                if ((intersection - corners[i]) * n_side * ((corners[(i + 2) % 3] - corners[i]) * n_side) < 1e-10)
                {
                    return -1;
                }
            }

            return t;
        }
    }
}
