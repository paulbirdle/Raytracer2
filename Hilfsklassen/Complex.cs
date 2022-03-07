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

        public Complex Conj
        {
            get { return new Complex(this.Re, -this.Im); }
        }

        public double Norm
        {
            get { return x[0] * x[0] + x[1] * x[1]; }
        }



    }
}
