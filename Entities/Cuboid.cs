using System;

namespace Raytracer
{
    class Cuboid : Entity
    {
        private readonly Vector center;
        private readonly Vector[] n; //up, right, front
        private readonly Material[] material; //oben, rechts, vorne, unten, links, hinten
        private readonly double[] a; //Die Drei Seitenlängen; a = {Höhe, Breite, Tiefe}

        private readonly Rectangle[] sides; //oben, rechts, vorne, unten, links, hinten
        private readonly Vector[] corners;
        private readonly bool multiple_materials;

        private readonly HitEllipsoid hitEllipsoid;

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material material)
        {
            this.center = center;
            n = new Vector[3];
            n[0] = up.normalize();
            n[2] = front.normalize();
            this.material = new Material[1] { material };
            multiple_materials = false;

            n[1] = n[2] ^ n[0];
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Rects();

            double f = Math.Sqrt(3)/2;
            hitEllipsoid = new HitEllipsoid(center, new double[3] { f * a[0] , f * a[1] , f * a[2] }, n[0], n[1]);
            //minimize surface area of ellipse!
        }

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material[] materials)
        {
            this.center = center;
            n = new Vector[3];
            n[0] = up.normalize();
            n[2] = front.normalize();
            material = materials;
            multiple_materials = true;

            n[1] = n[2] ^ n[0];
            if (lengths.Length == 3) a = lengths;
            corners = Get_Corners();
            sides = Get_Rects();

            double f = Math.Sqrt(3) / 2;
            hitEllipsoid = new HitEllipsoid(center, new double[3] { f * a[0], f * a[1], f * a[2] }, n[0], n[1]);
        }


        private Rectangle[] Get_Rects()
        {
            Rectangle[] ret = new Rectangle[6];
            if(multiple_materials == false)
            {
                Material mat = material[0];
                ret[0] = new Rectangle(center + (a[0] / 2) * n[0], n[0], n[1], a[1], a[2], mat); //oben
                ret[1] = new Rectangle(center + (a[1] / 2) * n[1], n[1], n[2], a[2], a[0], mat); //rechts
                ret[2] = new Rectangle(center + (a[2] / 2) * n[2], n[2], n[0], a[0], a[1], mat); //vorne
                ret[3] = new Rectangle(center - (a[0] / 2) * n[0], n[0], n[1], a[1], a[2], mat); //unten
                ret[4] = new Rectangle(center - (a[1] / 2) * n[1], n[1], n[2], a[2], a[0], mat); //links
                ret[5] = new Rectangle(center - (a[2] / 2) * n[2], n[2], n[0], a[0], a[1], mat); //hinten
            }
            else
            {
                ret[0] = new Rectangle(center + (a[0] / 2) * n[0], n[0], n[1], a[1], a[2], material[0]); //oben
                ret[1] = new Rectangle(center + (a[1] / 2) * n[1], n[1], n[2], a[2], a[0], material[1]); //rechts
                ret[2] = new Rectangle(center + (a[2] / 2) * n[2], n[2], n[0], a[0], a[1], material[2]); //vorne
                ret[3] = new Rectangle(center - (a[0] / 2) * n[0], n[0], n[1], a[1], a[2], material[3]); //unten
                ret[4] = new Rectangle(center - (a[1] / 2) * n[1], n[1], n[2], a[2], a[0], material[4]); //links
                ret[5] = new Rectangle(center - (a[2] / 2) * n[2], n[2], n[0], a[0], a[1], material[5]); //hinten
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
                        ret[k + 2 * j + 4 * i] = center + Math.Pow(-1, i) * a[0] / 2.0 * n[0] + Math.Pow(-1, j) * a[1] / 2.0 * n[1] + Math.Pow(-1, k) * a[2] / 2.0 * n[2];
                    }
                }
            }
            return ret;
        }

        public override double get_intersection(Ray ray)
        {
            if (!hitEllipsoid.hits(ray)) return -1;
            Vector v = ray.Direction;
            double outside = IsInside(ray) ? -1 : 1;

            double tmin = double.PositiveInfinity;
            double t;
            int j;

            for (int i = 0; i < 3; i++)
            {
                j = v * n[i] * outside > 0 ? i + 3 : i;

                t = sides[j].get_intersection(ray);
                if (t != -1 && t < tmin) tmin = t;
            }

            if (tmin == double.PositiveInfinity) return -1;
            return tmin;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            if (!hitEllipsoid.hits(ray)) // nötig?
            {
                material = null;
                n = null;
                return -1;
            }
            Vector v = ray.Direction;
            double outside = IsInside(ray) ? -1 : 1;

            double tmin = double.PositiveInfinity;
            double t;
            int l = 0;
            int j;

            for(int i = 0; i < 3; i++)
            {
                j = v * this.n[i] * outside > 0 ? i + 3 : i;

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
            n = l < 3 ? this.n[l % 3] : -this.n[l % 3];
            return tmin;
        }

        public Vector giveCenter()
        {
            return center;
        }

        public bool IsInside(Ray ray)
        {
            Vector x = ray.Start + 1e-6 * ray.Direction - center;
            if (Math.Abs(x * n[0]) <= a[0]/2 && Math.Abs(x * n[1]) <= a[1]/2 && Math.Abs(x * n[2]) <= a[2]/2)
            {
                return true;
            }
            return false;
        }
    }
}
