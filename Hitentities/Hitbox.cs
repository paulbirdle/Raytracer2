using System;

namespace Raytracer
{
    class Hitbox : Hitentity //besser HitSphere verwenden
    {
        private readonly Vector center;
        private readonly double[] a;
        private readonly Vector[] normals; //up, right, front

        private Vector[] corners;
        private HitQuadrilateral[] sides;
        private readonly Hitsphere hitsphere;

        public Hitbox(Vector center, Vector up, Vector front, double[] sidelengths)
        {
            if (Math.Abs(front * up) > 1e-10) throw new Exception("front muss senkrecht auf up stehen");
            this.center = center;
            normals = new Vector[3];
            normals[0] = up.normalize();
            normals[2] = front.normalize();// (front - (front * this.up) * this.up).normalize();
            if(sidelengths.Length == 3) a = sidelengths;
            normals[1] = normals[0] ^ normals[2];
            corners = new Vector[8];

            refresh_Quads();

            double radius = 0;   // hier hitsphere anlegen
            for (int i = 0; i < 8; i++)
            {
                if (i == 0) radius = (corners[i] - center).norm();
                else if ((corners[i] - center).norm() > radius)
                {
                    radius = (corners[i] - center).norm();
                }
            }
            hitsphere = new Hitsphere(center, radius);
        }

        private void refresh_Quads()
        {
            Refresh_Corners();
            sides = new HitQuadrilateral[6];
            sides[0] = new HitQuadrilateral(corners[0], corners[1], corners[3], corners[2]); //oben
            sides[1] = new HitQuadrilateral(corners[0], corners[1], corners[4], corners[5]); //rechts
            sides[2] = new HitQuadrilateral(corners[0], corners[4], corners[6], corners[2]); //vorne
            sides[3] = new HitQuadrilateral(corners[4], corners[5], corners[7], corners[6]); //unten
            sides[4] = new HitQuadrilateral(corners[2], corners[3], corners[7], corners[6]); //links
            sides[5] = new HitQuadrilateral(corners[1], corners[3], corners[7], corners[5]); //hinten
        }

        private void Refresh_Corners()
        {
            corners = new Vector[8];
            for (int i = 0; i < 2; i++)//oben unten
            {
                for (int j = 0; j < 2; j++)//rechts links
                {
                    for (int k = 0; k < 2; k++)//vorne hinten
                    {
                        corners[k + 2 * j + 4 * i] = center + (Math.Pow(-1, i) * a[0] / 2.0) * normals[0] + (Math.Pow(-1, j) * a[1] / 2.0) * normals[1] + (Math.Pow(-1, k) * a[2] / 2.0) * normals[2];
                    }
                }
            }
        }

        public override bool hits(Ray ray)
        {
            bool hs = hitsphere.hits(ray);
            if (hs == false) return false;

            //Vector v = ray.Direction;
            for(int i = 0; i < 6; i++)
            {
                int j = i;
                //if (normals[i] * v >= 0) j += 3; //man muss nur drei der sechs Flächen kontrollieren, edit: nur wenn der ray von außen kommt
                if (sides[j].hits(ray)) return true;
            }
            return false;
        }
    }
}
