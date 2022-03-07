using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Complex
    {
        private double[] x;

        public Complex()
        {
            x = new double[2] { 0, 0 };
        }

        public Complex(double a, double b)
        {
            x = new double[2] { a, b };
        }

        public Complex(double a)
        {
            x = new double[2] { a, 0 };
        }

        public double Re
        {
            get { return x[0]; }
            set { x[0] = value; }
        }

        public double Im
        {
            get { return x[1]; }
            set { x[1] = value; }
        }

        public static Complex operator +(Complex y, Complex z)
        {
            return new Complex(y.Re + z.Re, y.Im + z.Im);
        }

        public static Complex operator *(double y, Complex z)
        {
            return new Complex(y*z.Re, y*z.Im);
        }

        public static Complex operator *(Complex z, double y)
        {
            return y * z;
        }

        public static Complex operator -(Complex y, Complex z)
        {
            return y + (-1) * z;
        }

        public static Complex operator *(Complex y, Complex z)
        {
            return new Complex(y.Re * z.Re - y.Im * z.Im, y.Re * z.Im + y.Im * z.Re);
        }

        public static Complex operator +(double y, Complex z)
        {
            return new Complex(y) + z;
        }

        public static Complex operator +(Complex z, double y)
        {
            return y + z;
        }

        public static Complex operator /(Complex z, double x)
        {
            return new Complex(z.Re/x, z.Im/x);
        }

        public static Complex operator /(Complex x, Complex y)
        {
            return x * inv(y);
        }

        public static Complex operator /(double x, Complex z)
        {
            return inv(z / x);
        }

        public static Complex inv(Complex z)
        {
            return z.Conj/z.SquareNorm;
        }

        public Complex Conj
        {
            get { return new Complex(this.Re, -this.Im); }
        }

        public double SquareNorm
        {
            get { return x[0] * x[0] + x[1] * x[1]; }
        }

        public double Norm
        {
            get { return Math.Sqrt(SquareNorm); }
        }

        public double Arg
        {
            get { return Math.Atan2(Im, Re); }
        }

        public static Complex i
        {
            get { return new Complex(0, 1); }
        }

        public static Complex Exp(Complex z)
        {
            return Math.Exp(z.Re) * (Math.Cos(z.Im) + i * Math.Sin(z.Im));
        }

        public static Complex Log(Complex z)//principal branch of the complex logarithm
        {
            return Math.Log(z.Norm) + i * Math.Atan2(z.Im, z.Re);
        }

        public static Complex Pow(Complex z, double r)//Probleme: z = 0 und r < 0
        {
            if (z.Norm < 1e-12 && r < 0) throw new Exception(" ");

            return Math.Pow(z.SquareNorm, r / 2) * Complex.Exp(i * z.Arg * r);
        }

        public static Complex Sqrt(double x)
        {
            if (Math.Abs(x) < 1e-12) return new Complex(0);

            if (x < -1e-12) return i * Math.Sqrt(-x);

            return new Complex(Math.Sqrt(x));
        }

        public static Complex CubeRoot(Complex z)//principal Cube Root of z
        {
            return Math.Pow(z.SquareNorm, 1 / 6) * Complex.Exp(i*z.Arg / 3);
        }



    }
}
