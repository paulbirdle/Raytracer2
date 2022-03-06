using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	class Poly
	{
		public static int solveP3(out double[] x, double a, double b, double c) //wieviele Lösungen hat x^3 + ax^2 + bx + c
		{
			x = new double[3];
			double a2 = a * a;
			double q = (a2 - 3 * b) / 9;
			double r = (a * (2 * a2 - 9 * b) + 27 * c) / 54;
			double r2 = r * r;
			double q3 = q * q * q;
			double A, B;
			if (r2 < q3)
			{
				double t = r / Math.Sqrt(q3);
				if (t < -1) t = -1;
				if (t > 1) t = 1;
				t = Math.Acos(t);
				a /= 3; q = -2 * Math.Sqrt(q);
				x[0] = q * Math.Cos(t / 3) - a;
				x[1] = q * Math.Cos((t + 2 * Math.PI) / 3) - a;
				x[2] = q * Math.Cos((t - 2 * Math.PI) / 3) - a;
				return 3;
			}
			else
			{
				A = -Math.Pow(Math.Abs(r) + Math.Sqrt(r2 - q3), 1 / 3);
				if (r < 0) A = -A;
				B = (0 == A ? 0 : q / A);

				a /= 3;
				x[0] = (A + B) - a;
				x[1] = -0.5 * (A + B) - a;
				x[2] = 0.5 * Math.Sqrt(3) * (A - B);
				if (Math.Abs(x[2]) < 1e-10) { x[2] = x[1]; return 2; }

				return 1;
			}
		}

		//---------------------------------------------------------------------------
		// Solve quartic equation x^4 + a*x^3 + b*x^2 + c*x + d
		// (attention - this function returns dynamically allocated array. It has to be released afterwards)
		public static Complex[] solve_quartic(double a, double b, double c, double d)
		{
			double a3 = -b;
			double b3 = a * c - 4 * d;
			double c3 = -a * a * d - c * c + 4 * b * d;

			// cubic resolvent
			// y^3 − b*y^2 + (ac−4d)*y − a^2*d−c^2+4*b*d = 0

			double[] x3 = new double[3];
			int iZeroes = solveP3(out x3, a3, b3, c3);

			double q1, q2, p1, p2, D, sqD, y;

			y = x3[0];
			// THE ESSENCE - choosing Y with maximal absolute value !
			if (iZeroes != 1)
			{
				if (Math.Abs(x3[1]) > Math.Abs(y)) y = x3[1];
				if (Math.Abs(x3[2]) > Math.Abs(y)) y = x3[2];
			}

			// h1+h2 = y && h1*h2 = d  <=>  h^2 -y*h + d = 0    (h === q)

			D = y * y - 4 * d;
			if (Math.Abs(D) < 1e-10) //in other words - D==0
			{
				q1 = q2 = y * 0.5;
				// g1+g2 = a && g1+g2 = b-y   <=>   g^2 - a*g + b-y = 0    (p === g)
				D = a * a - 4 * (b - y);
				if (Math.Abs(D) < 1e-10) //in other words - D==0
					p1 = p2 = a * 0.5;

				else
				{
					sqD = Math.Sqrt(D);
					p1 = (a + sqD) * 0.5;
					p2 = (a - sqD) * 0.5;
				}
			}
			else
			{
				sqD = Math.Sqrt(D);
				q1 = (y + sqD) * 0.5;
				q2 = (y - sqD) * 0.5;
				// g1+g2 = a && g1*h2 + g2*h1 = c       ( && g === p )  Krammer
				p1 = (a * q1 - c) / (q1 - q2);
				p2 = (c - a * q2) / (q1 - q2);
			}

			Complex[] retval = new Complex[4];

			// solving quadratic eq. - x^2 + p1*x + q1 = 0
			D = p1 * p1 - 4 * q1;
			if (D < 0.0)
			{
				retval[0].Re = (-p1 * 0.5);
				retval[0].Im = (Math.Sqrt(-D) * 0.5);
				retval[1] = retval[0].Conj;
			}
			else
			{
				sqD = Math.Sqrt(D);
				retval[0].Re = ((-p1 + sqD) * 0.5);
				retval[1].Re = ((-p1 - sqD) * 0.5);
			}

			// solving quadratic eq. - x^2 + p2*x + q2 = 0
			D = p2 * p2 - 4 * q2;
			if (D < 0.0)
			{
				retval[2].Re = (-p2 * 0.5);
				retval[2].Im = (Math.Sqrt(-D) * 0.5);
				retval[3] = retval[2].Conj;
			}
			else
			{
				sqD = Math.Sqrt(D);
				retval[2].Re = ((-p2 + sqD) * 0.5);
				retval[3].Re = ((-p2 - sqD) * 0.5);
			}

			return retval;
		}
	}
}
