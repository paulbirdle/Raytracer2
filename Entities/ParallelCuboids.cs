using System;
using System.Collections.Generic;

namespace Raytracer
{
    class ParallelCuboids : Entity
    {
        protected int N;
        protected readonly Vector[] n;
        protected readonly Vector[] centers;
        protected readonly double[,] a;             //a[number cuboid, side]
        protected readonly Material[,] mat;         //mat[number of cuboid, side]
        protected readonly bool allSameMaterial;
        protected readonly bool[] OneMaterial;
        protected readonly Vector[,,] SideMiddle;   //SideMiddle[cuboid, side, back/front]
        protected readonly HitEllipsoid[] hitEllipsoid;

        public ParallelCuboids(Vector[] n, Vector[] centers, double[,] a, Material mat)
        {
            N = centers.Length;
            this.n = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                this.n[i] = n[i].normalize();
            }
            this.centers = centers;
            this.a = a;
            this.mat = new Material[1, 1] { { mat } };
            this.allSameMaterial = true;

            SideMiddle = new Vector[N, 3, 2];
            hitEllipsoid = new HitEllipsoid[N];
            double f = Math.Sqrt(3) / 2;
            for (int i = 0; i < N; i++)
            {
                hitEllipsoid[i] = new HitEllipsoid(centers[i], new double[3] { f * a[i, 0], f * a[i, 1], f * a[i, 2] }, this.n[0], this.n[1]);
                for(int j = 0; j < 3; j++)
                {
                    SideMiddle[i, j, 0] = centers[i] + (a[i, j] / 2) * this.n[j];
                    SideMiddle[i, j, 1] = centers[i] - (a[i, j] / 2) * this.n[j];
                }
            }
            
        }

        public ParallelCuboids(Vector[] n, Vector[] centers, double[,] a, Material[] mat)
        {
            N = centers.Length;
            this.n = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                this.n[i] = n[i].normalize();
            }
            this.centers = centers;
            this.a = a;

            this.mat = new Material[N, 1];
            OneMaterial = new bool[N];
            allSameMaterial = true;
            for (int i = 0; i < N; i++)
            {
                this.mat[i, 1] = mat[i];
                if (mat[i] != mat[0]) allSameMaterial = false;
                OneMaterial[i] = true;
            }

            SideMiddle = new Vector[N, 3, 2];
            hitEllipsoid = new HitEllipsoid[N];
            double f = Math.Sqrt(3) / 2;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    hitEllipsoid[i] = new HitEllipsoid(centers[i], new double[3] { f * a[i, 0], f * a[i, 1], f * a[i, 2] }, this.n[0], this.n[1]);
                    SideMiddle[i, j, 0] = centers[i] + (a[i, j] / 2) * this.n[j];
                    SideMiddle[i, j, 1] = centers[i] - (a[i, j] / 2) * this.n[j];
                }
            }
        }

        public ParallelCuboids(Vector[] n, Vector[] centers, double[,] a, Material[,] mat)
        {
            N = centers.Length;
            this.n = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                this.n[i] = n[i].normalize();
            }
            this.centers = centers;
            this.a = a;

            OneMaterial = new bool[N];
            allSameMaterial = false;
            this.mat = mat;
            for (int i = 0; i < N; i++)
            {
                OneMaterial[i] = true;
                for(int j = 1; j < 6; j++)
                {
                    if(mat[i,j] != mat[i,0]) OneMaterial[i] = false;
                }
            }

            SideMiddle = new Vector[N, 3, 2];
            hitEllipsoid = new HitEllipsoid[N];
            double f = Math.Sqrt(3) / 2;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    hitEllipsoid[i] = new HitEllipsoid(centers[i], new double[3] { f * a[i, 0], f * a[i, 1], f * a[i, 2] }, this.n[0], this.n[1]);
                    SideMiddle[i, j, 0] = centers[i] + (a[i, j] / 2) * this.n[j];
                    SideMiddle[i, j, 1] = centers[i] - (a[i, j] / 2) * this.n[j];
                }
            }
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector v = ray.Direction;
            Vector x = ray.Start;
            double[] scalprod = new double[3];
            List<int> relevantSides = new List<int>();
            for(int j = 0; j < 3; j++)
            {
                scalprod[j] = v * this.n[j];
                if (Math.Abs(scalprod[j]) > 1e-6) relevantSides.Add(j);
            }
            double tmin = double.PositiveInfinity;
            double t;
            int cuboid = 0;
            int side = 0;
            int frontBack = 0;
            Vector intersect;

            for(int i = 0; i < N; i++)     //cuboids
            {
                if (!hitEllipsoid[i].hits(ray)) continue;
                foreach(int j in relevantSides)         //sides
                {
                    for(int k = 0; k < 2; k++)          //front/back
                    {
                        t = (SideMiddle[i, j, k] - x) * this.n[j] / scalprod[j];
                        if (t >= tmin || t <= 1e-10) continue;
                        intersect = x + t * v;
                        if (Math.Abs((intersect - SideMiddle[i, j, k]) * this.n[(j + 1) % 3]) <= a[i, (j + 1) % 3]/2 && Math.Abs((intersect - SideMiddle[i, j, k]) * this.n[(j + 2) % 3]) <= a[i, (j + 2) % 3]/2)
                        {
                            cuboid = i;
                            side = j;
                            frontBack = k;
                            tmin = t;
                        }
                    }
                }
            }
            if(tmin == double.PositiveInfinity)
            {
                n = null;
                material = null;
                return -1;
            }
            n = frontBack == 0 ? this.n[side] : -this.n[side];
            if (allSameMaterial) material = mat[0, 0];
            else if (OneMaterial[cuboid]) material = mat[cuboid, 0];
            else material = mat[cuboid, side + 3 * frontBack];
            return tmin;
        }

        public override double get_intersection(Ray ray)
        {
            Vector v = ray.Direction;
            Vector x = ray.Start;
            double[] scalprod = new double[3];
            List<int> relevantSides = new List<int>();
            for (int j = 0; j < 3; j++)
            {
                scalprod[j] = v * this.n[j];
                if (Math.Abs(scalprod[j]) > 1e-6) relevantSides.Add(j);
            }
            double tmin = double.PositiveInfinity;
            double t;
            Vector intersect;

            for (int i = 0; i < N; i++)                     //cuboids
            {
                if (!hitEllipsoid[i].hits(ray)) continue;
                foreach (int j in relevantSides)            //sides
                {
                    for (int k = 0; k < 2; k++)             //front/back
                    {
                        t = (SideMiddle[i, j, k] - x) * this.n[j] / scalprod[j];
                        if (t >= tmin || t <= 1e-10) continue;
                        intersect = x + t * v;
                        if (Math.Abs((intersect - SideMiddle[i, j, k]) * this.n[(j + 1) % 3]) <= a[i, (j + 1) % 3]/2 && Math.Abs((intersect - SideMiddle[i, j, k]) * this.n[(j + 2) % 3]) <= a[i, (j + 2) % 3]/2)
                        {
                            tmin = t;
                        }
                    }
                }
            }
            if (tmin == double.PositiveInfinity) return -1;
            return tmin;
        }
    }
}
