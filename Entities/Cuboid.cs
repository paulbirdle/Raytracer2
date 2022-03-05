using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Cuboid : Entity   //TODO
    {
        Vector center;
        Vector up;
        Vector front;
        Material material;
        double[] a;//Die Drei Seitenlängen; a = {Höhe, Breite, Tiefe}

        Vector right;
        Quadrilateral[] sides;
        Vector[] corners;

        public Cuboid(Vector center, Vector up, Vector front, double[] lengths, Material material)
        {
            this.center = center;
            this.up = up.normalize();
            this.front = front.normalize();
            this.material = material;

            this.right = up ^ front;
            if (lengths.Length == 3 && lengths[0] > 0 && lengths[1] > 0 && lengths[2] > 0)
            {
                this.a = lengths;
            }
            refresh_Quads();
        }


        private void refresh_Quads()
        {
            refresh_Corners();
            Quadrilateral[] sides = new Quadrilateral[6];
            sides[0] = new Quadrilateral(corners[0], corners[1], corners[3], corners[2], material); //oben
            sides[1] = new Quadrilateral(corners[3], corners[1], corners[5], corners[7], material); //rechts
            sides[2] = new Quadrilateral(corners[2], corners[3], corners[6], corners[6], material); //vorne
            sides[3] = new Quadrilateral(corners[4], corners[5], corners[7], corners[6], material); //unten
            sides[4] = new Quadrilateral(corners[0], corners[2], corners[4], corners[6], material); //links
            sides[5] = new Quadrilateral(corners[1], corners[0], corners[5], corners[4], material); //hinten
        }

        private void refresh_Corners()
        {
            corners = new Vector[8];
            for (int i = 0; i < 2; i++)//oben unten
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        if (up == null || right == null || front == null)
                        {
                            throw new Exception(" ");
                        }
                        corners[i + 2 * j + 4 * k] = center
                        + Math.Pow(-1, i) * a[0] / 2.0 * up
                        + Math.Pow(-1, j) * a[1] / 2.0 * right
                        + Math.Pow(-1, k) * a[2] / 2.0 * front;
                    }
                }
            }
        }

        public override double get_intersection(Ray ray)
        {
            double t = double.PositiveInfinity;
            double t_temp;
            int side = 0;
            for (int i = 0; i < 6; i++)
            {
                t_temp = sides[i].get_intersection(ray);
                if (t_temp < t)
                {
                    t = t_temp;
                    side = i;
                }
            }

            return t;
        }

        /*public override Ray interact(Ray ray)
        {
            Vector x = ray.Start;
            Vector v = ray.Direction;

            //bestimme n:
            Vector n = new Vector();
            double[] prod = new double[3] { Math.Abs(v * up), Math.Abs(v * right), Math.Abs(v * front) };
            double maxValue = prod.Max();
            int maxIndex = prod.ToList().IndexOf(maxValue);
            if (maxIndex == 0) n = up;
            else if (maxIndex == 1) n = right;
            else if (maxIndex == 2) n = front;

            return new Ray(-v.reflect_at(n), ray.position_at_time(get_intersection(ray)));
        }*/


    }
}
