using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer2
{
    class Vector
    {
        private double[] v;

        public Vector(double x, double y, double z)
        {
            v = new double[3] { x, y, z };
        }

        public Vector()
        {
            v = new double[3] { 0, 0, 0 };
        }

        public double X
        {
            get { return v[0]; }
            set { v[0] = value; }
        }

        public double Y
        {
            get { return v[1]; }
            set { v[1] = value; }
        }

        public double Z
        {
            get { return v[2]; }
            set { v[2] = value; }
        }

        public double get(int i)
        {
            return v[i];
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

        public static double angle (Vector v, Vector w)
        {
            return Math.Acos(v * w / (v.norm() * w.norm()));
        }

        public static Vector rotate (Vector to_be_rotated, Vector axis, double angle)
        {
            Matrix A = Matrix.Rotation(axis, angle);
            return A * to_be_rotated;
        }

        public double norm()
        {
            return Math.Sqrt(this * this);
        }

        public double squareNorm()
        {
            return this * this;
        }

        public Vector normalize()
        {
            return this / norm();
        }

        public Vector reflect_at(Vector v) //reflektiert this an v und gibt reflektierten Vektor zurück
        {
            v = v.normalize();
            v *= (v * this);
            return this + 2*(v-this);
        }
    }
}
