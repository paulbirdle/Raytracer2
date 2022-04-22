using System;
using System.Threading;

namespace Raytracer
{
    class Vector 
    {
        //private readonly double[] v;
        private double vx = 0; // ca. 20% schneller...
        private double vy = 0;
        private double vz = 0;
        public static long numVec = 0;

        public Vector(double x, double y, double z)
        {
#if DEBUG
            Interlocked.Increment(ref numVec);
#endif
            vx = x;
            vy = y;
            vz = z;
        }

        public Vector()
        {
#if DEBUG
            Interlocked.Increment(ref numVec);
#endif
        }

        public double X
        {
            get { return vx; }
            set { vx = value; }
        }

        public double Y
        {
            get { return vy; }
            set { vy = value; }
        }

        public double Z
        {
            get { return vz; }
            set { vz = value; }
        }

        public double Get(int i)
        {
            return (i == 0) ? vx : (i == 1) ? vy : vz;
        }

        public static Vector operator *(double a, Vector w)
        {
            return new Vector(a * w.X, a * w.Y, a * w.Z);
        }

        public static Vector operator *(Vector w, double a)
        {
            return a * w;
        }

        public static Vector operator /(Vector w, double a)
        {
            return w * (1 / a);
        }

        public static Vector operator +(Vector v, Vector w)
        {
            return new Vector(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
        }

        public static Vector operator -(Vector v, Vector w)
        {
            return v + (-1) * w;
        }

        public static Vector operator -(Vector v)
        {
            return (-1) * v;
        }

        public static double operator *(Vector v, Vector w)
        {
            return v.X * w.X + v.Y * w.Y + v.Z * w.Z;
        }

        public static Vector operator ^(Vector v, Vector w)
        {
            return new Vector(v.Y * w.Z - v.Z * w.Y, v.Z * w.X - v.X * w.Z, v.X * w.Y - v.Y * w.X);
        }

        public static Vector operator &(Vector v, Vector w)
        {
            return new Vector(v.X * w.X, v.Y * w.Y, v.Z * w.Z);
        }



        public static double angle (Vector v, Vector w)
        {
            return Math.Acos(v * w / (v.norm() * w.norm()));
        }

        public static Vector Rotate (Vector to_be_rotated, Vector axis, double angle)
        {
            Matrix A = Matrix.Rotation(axis, angle);
            return A * to_be_rotated;
        }

        public static double weightedScalProd (Vector v, Vector w, double[] a)
        {
            return v.X * w.X / a[0] + v.Y * w.Y / a[1] + v.Z * w.Z / a[2];
        }

        public double norm()
        {
            return Math.Sqrt(this * this);
        }

        public double SquareNorm()
        {
            return this * this;
        }

        public Vector normalize()
        {
            if (this.SquareNorm() < 1e-10) throw new Exception("");
            return this / norm();
        }

        public Vector reflect_at(Vector v) //reflektiert this an v und gibt reflektierten Vektor zurück
        {
            v = v.normalize();
            v *= v * this;
            return this + 2*(v-this);
        }
    }
}
