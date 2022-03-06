using System;

namespace Raytracer
{
    class Cuboid : Entity
    {
        private readonly Vector center;
        private readonly Vector[] normals; //up, right, front
        private readonly Material[] material; //oben, rechts, vorne, unten, links, hinten
        private readonly double[] a;//Die Drei Seitenlängen; a = {Höhe, Breite, Tiefe}

        private readonly Quadrilateral[] sides; //oben, rechts, vorne, unten, links, hinten
        private readonly Vector[] corners;
        private readonly bool multiple_materials;

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material material)
        {
            this.center = center;
            normals[0] = up.normalize();
            normals[2] = front.normalize();
            this.material = new Material[1] { material };
            multiple_materials = false;

            normals[1] = up ^ front;
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Quads();
        }

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material[] materials)
        {
            this.center = center;
            normals[0] = up.normalize();
            normals[2] = front.normalize();
            material = materials;
            multiple_materials = true;

            normals[1] = up ^ front;
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Quads();
        }


        private Quadrilateral[] Get_Quads()
        {
            Quadrilateral[] ret = new Quadrilateral[6];
            if(multiple_materials == false)
            {
                Material mat = material[0];
                ret[0] = new Quadrilateral(corners[0], corners[1], corners[3], corners[2], mat); //oben
                ret[1] = new Quadrilateral(corners[3], corners[1], corners[5], corners[7], mat); //rechts
                ret[2] = new Quadrilateral(corners[2], corners[3], corners[6], corners[6], mat); //vorne
                ret[3] = new Quadrilateral(corners[4], corners[5], corners[7], corners[6], mat); //unten
                ret[4] = new Quadrilateral(corners[0], corners[2], corners[4], corners[6], mat); //links
                ret[5] = new Quadrilateral(corners[1], corners[0], corners[5], corners[4], mat); //hinten
            }
            else
            {
                ret[0] = new Quadrilateral(corners[0], corners[1], corners[3], corners[2], material[0]); //oben
                ret[1] = new Quadrilateral(corners[3], corners[1], corners[5], corners[7], material[1]); //rechts
                ret[2] = new Quadrilateral(corners[2], corners[3], corners[6], corners[6], material[2]); //vorne
                ret[3] = new Quadrilateral(corners[4], corners[5], corners[7], corners[6], material[3]); //unten
                ret[4] = new Quadrilateral(corners[0], corners[2], corners[4], corners[6], material[4]); //links
                ret[5] = new Quadrilateral(corners[1], corners[0], corners[5], corners[4], material[5]); //hinten
            }
            return ret;
        }

        private Vector[] Get_Corners()
        {
            Vector[] ret = new Vector[8];
            for (int i = 0; i < 2; i++)//oben unten
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        ret[i + 2 * j + 4 * k] = center + Math.Pow(-1, i) * a[0] / 2.0 * up + Math.Pow(-1, j) * a[1] / 2.0 * right + Math.Pow(-1, k) * a[2] / 2.0 * front;
                    }
                }
            }
            return ret;
        }

        public override double get_intersection(Ray ray)
        {
            double tmin = double.PositiveInfinity;
            double t;
            for (int i = 0; i < 6; i++)
            {
                t = sides[i].get_intersection(ray);
                if (t > 1e-10 && t < tmin)
                {
                    tmin = t;
                }
            }
            if (tmin == double.PositiveInfinity) return -1;
            return tmin;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            double tmin = double.PositiveInfinity;
            double t;
            int side = -1;
            for (int i = 0; i < 6; i++)
            {
                t = sides[i].get_intersection(ray);
                if (t > 1e-10 && t < tmin)
                {
                    tmin = t;
                    side = i;
                }
            }
            if (tmin == double.PositiveInfinity)
            {
                n = null;
                material = null;
                return -1;
            }
            material = this.material[side];
            n = normals[side % 3];
            if (side >= 3) n *= -1;
            return tmin;
        }
    }
}
