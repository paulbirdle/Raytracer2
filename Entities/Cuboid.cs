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

        private readonly EntityGroup group;

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material material)
        {
            this.center = center;
            normals = new Vector[3];
            normals[0] = up.normalize();
            normals[2] = front.normalize();
            this.material = new Material[1] { material };
            multiple_materials = false;

            normals[1] = normals[2] ^ normals[0];
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Quads();

            group = new EntityGroup(sides, new Hitsphere(center, Math.Sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]) / 2));
        }

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material[] materials)
        {
            this.center = center;
            normals = new Vector[3];
            normals[0] = up.normalize();
            normals[2] = front.normalize();
            material = materials;
            multiple_materials = true;

            normals[1] = normals[2] ^ normals[0];
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Quads();

            group = new EntityGroup(sides, new Hitsphere(center, Math.Sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]) / 2));
        }


        private Quadrilateral[] Get_Quads()
        {
            Quadrilateral[] ret = new Quadrilateral[6];
            if(multiple_materials == false)
            {
                Material mat = material[0];
                ret[0] = new Quadrilateral(corners[0], corners[1], corners[3], corners[2], mat); //oben
                ret[1] = new Quadrilateral(corners[0], corners[1], corners[5], corners[4], mat); //rechts
                ret[2] = new Quadrilateral(corners[0], corners[4], corners[6], corners[2], mat); //vorne
                ret[3] = new Quadrilateral(corners[7], corners[5], corners[4], corners[6], mat); //unten
                ret[4] = new Quadrilateral(corners[7], corners[6], corners[2], corners[3], mat); //links
                ret[5] = new Quadrilateral(corners[7], corners[3], corners[1], corners[5], mat); //hinten
            }
            else
            {
                ret[0] = new Quadrilateral(corners[0], corners[1], corners[3], corners[2], material[0]); //oben
                ret[1] = new Quadrilateral(corners[0], corners[1], corners[5], corners[4], material[1]); //rechts
                ret[2] = new Quadrilateral(corners[0], corners[4], corners[6], corners[2], material[2]); //vorne
                ret[3] = new Quadrilateral(corners[7], corners[5], corners[4], corners[6], material[3]); //unten
                ret[4] = new Quadrilateral(corners[7], corners[6], corners[2], corners[3], material[4]); //links
                ret[5] = new Quadrilateral(corners[7], corners[3], corners[1], corners[5], material[5]); //hinten
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
                        ret[k + 2 * j + 4 * i] = center + Math.Pow(-1, i) * a[0] / 2.0 * normals[0] + Math.Pow(-1, j) * a[1] / 2.0 * normals[1] + Math.Pow(-1, k) * a[2] / 2.0 * normals[2];
                    }
                }
            }
            return ret;
        }

        public override double get_intersection(Ray ray)
        {
            //return group.get_intersection(ray);
            Vector v = ray.Direction;
            double outside = isInside(ray) ? -1 : 1;

            double tmin = double.PositiveInfinity;
            double t;
            int j;

            for (int i = 0; i < 3; i++)
            {
                j = v * normals[i] * outside > 0 ? i + 3 : i;

                t = sides[j].get_intersection(ray);
                if (t != -1 && t < tmin) tmin = t;
            }

            if (tmin == double.PositiveInfinity) return -1;
            return tmin;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            //return group.get_intersection(ray, out n, out material);
            Vector v = ray.Direction;
            double outside = isInside(ray) ? -1 : 1;

            double tmin = double.PositiveInfinity;
            double t;
            int l = 0;
            int j;

            for(int i = 0; i < 3; i++)
            {
                j = v * normals[i] * outside > 0 ? i + 3 : i;

                t = sides[j].get_intersection(ray);
                if(t != -1 && t < tmin)
                {
                    tmin = t;
                    l = j;
                }
            }

            if(tmin == double.PositiveInfinity)
            {
                material = null;
                n = null;
                return -1;
            }
            material = multiple_materials ? this.material[l] : this.material[0];
            n = l < 3 ? normals[l % 3] : -normals[l % 3];
            return tmin;
        }

        public Vector giveCenter()
        {
            return center;
        }

        public bool isInside(Ray ray)
        {
            Vector x = ray.Start + 1e-6 * ray.Direction - center;
            if (Math.Abs(x * normals[0]) <= a[0] && Math.Abs(x * normals[1]) <= a[1] && Math.Abs(x * normals[2]) <= a[2])
            {
                return true;
            }
            return false;
        }
    }
}
