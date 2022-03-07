using System;
using System.Collections.Generic;

namespace Raytracer
{
	class Poly
	{
		/*public static double[] solveRealQuarticRoots(double a, double b, double c, double d, double e)
		{
			double s1 = 2 * c * c * c - 9 * b * c * d + 27 * (a * d * d + b * b * e) - 72 * a * c * e;
			double q1 = c * c - 3 * b * d + 12 * a * e;
			double discrim1 = -4 * q1 * q1 * q1 + s1 * s1;
			if (discrim1 > 0)
			{
				double s2 = s1 + Math.Sqrt(discrim1);
				double q2 = Math.Pow(s2 / 2, 1 / 3);
				double s3 = q1 / (3 * a * q2) + q2 / (3 * a);
				double discrim2 = (b * b) / (4 * a * a) - (2 * c) / (3 * a) + s3;
				if (discrim2 > 0)
				{
					double s4 = Math.Sqrt(discrim2);
					double s5 = (b * b) / (2 * a * a) - (4 * c) / (3 * a) - s3;
					double s6 = (-(b * b * b) / (a * a * a) + (4 * b * c) / (a * a) - (8 * d) / a) / (4 * s4);
					double discrim3 = (s5 - s6);
					double discrim4 = (s5 + s6);
					// actual root values, may not be set
					double r1 = 0, r2 = 0, r3 = 0, r4 = 0;

					if (discrim3 > 0)
					{
						double sqrt1 = Math.Sqrt(s5 - s6);
						r1 = -b / (4 * a) - s4 / 2 + sqrt1 / 2;
						r2 = -b / (4 * a) - s4 / 2 - sqrt1 / 2;
					}
					else if (discrim3 == 0)
					{
						// repeated root case
						r1 = -b / (4 * a) - s4 / 2;
					}
					if (discrim4 > 0)
					{
						double sqrt2 = Math.Sqrt(s5 + s6);
						r3 = -b / (4 * a) + s4 / 2 + sqrt2 / 2;
						r4 = -b / (4 * a) + s4 / 2 - sqrt2 / 2;
					}
					else if (discrim4 == 0)
					{
						r3 = -b / (4 * a) + s4 / 2;
					}
					if (discrim3 > 0 && discrim4 > 0) return new double[4]{ r1,r2,r3,r4};

					else if (discrim3 > 0 && discrim4 == 0) return new double[3] { r1,r2,r3};

					else if (discrim3 > 0 && discrim4 < 0) return new double[2] { r1,r2};

					else if (discrim3 == 0 && discrim4 > 0) return new double[3] { r1,r3,r4};

					else if (discrim3 == 0 && discrim4 == 0) return new double[2] { r1,r3};

					else if (discrim3 == 0 && discrim4 < 0) return new double[1] { r1};

					else if (discrim3 < 0 && discrim4 > 0) return new double[2] { r3,r4};

					else if (discrim3 < 0 && discrim4 == 0) return new double[1] { r3};

					//else if (discrim3 < 0 && discrim4 < 0) return new double[0];
				}
			}
			return new double[0];
		}*/

		public static double QuarticNumerically(double a, double b, double c, double d, double x0) //x^4+ax^3+bx^2+cx+d
		{
			double x = x0;
			double dx = double.PositiveInfinity;
			int count = 0;

			while(Math.Abs(dx) > 1e-8 || count < 5)
			{
				count++;
				if (count > 100) return double.NaN;

				double x2 = x * x;
				double x3 = x2 * x;
				double x4 = x3 * x;

				double f = x4 + a * x3 + b * x2 + c * x + d;
				double f_ = 4 * x3 + 3 * b * x2 + 2 * b * x + c;
				if (Math.Abs(f_) < 1e-10) return double.NaN;
				
				dx = f/f_;
				x -= dx;
			}
			return x;
		}

		public static double CubicNumerically(double a, double b, double c, double x0)
		{
			double x = x0;
			double dx = double.PositiveInfinity;
			int count = 0;

			while (Math.Abs(dx) > 1e-8 || count < 5)
			{
				count++;
				if (count > 100) return double.NaN;

				double x2 = x * x;
				double x3 = x2 * x;

				double f = x3 + a * x2 + b * x + c;
				double f_ = 3 * x2 + 2 * a * x + b;
				if (Math.Abs(f_) < 1e-10) return double.NaN;

				dx = f / f_;
				x -= dx;
			}
			return x;
		}

		public static double[] DivQuart(double[] a, double x0) // return coefficients of (x^4+a1x^3+a2x^2+a3x+a4) / (x-x0) = x^3+ex^2+fx+g
		{
			double[] ret = new double[3];
			ret[0] = a[0] + x0;
			ret[1] = a[1] + x0 * ret[0];
			ret[2] = a[2] + x0 * ret[1];

			double rem = a[3] + x0 * ret[2];
			if (Math.Abs(rem) > 1e-6) throw new Exception(" ");
			return ret;
		}

		public static double[] DivCube(double[] a, double x0)
		{
			double[] ret = new double[2];
			ret[0] = a[0] + x0;
			ret[1] = a[1] + x0 * ret[0];

			double rem = a[2] + x0 * ret[1];
			if (Math.Abs(rem) > 1e-6) throw new Exception(" ");
			return ret;
		}

		public static double GetRoot(double a, double b, double c, double d, double x0)//x^4 + ax^3 + bx^2 + cx + d
		{
			double x1 = QuarticNumerically(a, b, c, d, x0);
			if (x1 == double.NaN) return -1;

			double[] cub = DivQuart(new double[4] { a, b, c, d }, x1);

			double x2 = CubicNumerically(cub[0], cub[1], cub[2], x0);
			if (x2 == double.NaN) throw new Exception("");

			double[] quad = DivCube(cub, x2);
			double D = quad[0] * quad[0] / 4 - quad[1];

			double ret;

			if (D < 0)
			{
				ret = PosMin(new double[2] { x1, x2 });
			}
			else
			{
				double sqrt = Math.Sqrt(D);
				ret = PosMin(new double[4] { x1, x2, -quad[0] / 2 + sqrt, -quad[0] / 2 - sqrt });
			}
			return ret;
		}

		public static double PosMin(double[] x)
		{
			double xmin = double.PositiveInfinity;
			double x_;
			for(int i = 0; i < x.Length; i++)
			{
				x_ = x[i];
				if (x_ <= 1e-6) continue;
				if (x_ < xmin) xmin = x_;
			}
			if (xmin == double.PositiveInfinity) return -1;
			return xmin;
		}


		public static Complex[] SolveCubic(double a2, double a1, double a0) //zeros of f = t^3 + a2*t^2 + a1*t + a0 
		{
			double p = a1 - a2 * a2 / 3;
			double q = a0 - a1 * a2 / 3 + 2 * a1 * a1 / 27; //f(t-a2/3) = g = t^3 + pt + q

			double D = q * q / 4 + p * p * p / 27;
			Complex sqrtD = Complex.Sqrt(D);

			Complex u = Complex.CubeRoot(-q / 2 + sqrtD);
			Complex v = -p / 3 * u;

			Complex xi = Complex.Exp(Complex.i * 2 * Math.PI / 3);
			Complex xi2 = xi * xi;

			return new Complex[3] { u + v, xi * u + xi2 * v, xi2 * u + xi * v };
		}


		/*static double CubicRoot(double n)
		{
			return Math.Pow(Math.Abs(n), 1 / 3) * Math.Sign(n);
		}


		public static List<double> SolveCubic(double A1, double A2, double A3)
		{
			List<double> output = new List<double>();

			double P = -(A1 * A1 / 3) + A2;
			double Q = (2.0 * A1 * A1 * A1 / 27.0) - (A1 * A2 / 3.0) + A3;
			double cubeDiscr = Q * Q / 4.0 + P * P * P / 27.0;
			if (cubeDiscr > 1e-10)
			{
				double u = CubicRoot(-Q / 2.0 + Math.Sqrt(cubeDiscr));
				double v = CubicRoot(-Q / 2.0 - Math.Sqrt(cubeDiscr));
				output.Add(u + v - (A1 / 3.0));
				return output;
			}
			else if (Math.Abs(cubeDiscr) < 1e-10) //hier ist noch was falsch
			{
				double u = CubicRoot(-Q / 2.0);
				output.Add(2*u - (A1 / 3.0));
				output.Add(-u - (A1 / 3.0));
			}
			else if (cubeDiscr < -1e-10)
			{
				double r = CubicRoot(Math.Sqrt(-(P * P * P / 27.0)));
				double alpha = Math.Atan(Math.Sqrt(-cubeDiscr) / (-Q / 2.0));
				output.Add(r * (Math.Cos(alpha / 3.0) + Math.Cos((6 * Math.PI - alpha) / 3.0)) - A1 / 3.0);
				output.Add(r * (Math.Cos((2 * Math.PI + alpha) / 3.0) + Math.Cos((4 * Math.PI - alpha) / 3.0)) - A1 / 3.0);
				output.Add(r * (Math.Cos((4 * Math.PI + alpha) / 3.0) + Math.Cos((2 * Math.PI - alpha) / 3.0)) - A1 / 3.0);
			}
			return output;
		}


		public static int solveP3(out double[] x, double a, double b, double c) //wieviele Lösungen hat x^3 + ax^2 + bx + c
		{
			double z;
			double result;

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
				a /= 3; 
				q = -2 * Math.Sqrt(q);
				x[0] = q * Math.Cos(t / 3) - a;
				x[1] = q * Math.Cos((t + 2 * Math.PI) / 3) - a;
				x[2] = q * Math.Cos((t - 2 * Math.PI) / 3) - a;

				for(int i = 0; i < 3; i++)
				{
					z = x[i];
					result = z * z * z + a * z * z + b * z + c;
					if(Math.Abs(result) > 1)
					{
						bool bo = false;
					}
				}

				return 3;
			}
			else
			{
				A = -Math.Pow(Math.Abs(r) + Math.Sqrt(r2 - q3), 1 / 3);
				if (r < 0) A = -A;
				B = (Math.Abs(A) < 1e-4 ? 0 : q / A);

				a /= 3;
				x[0] = (A + B) - a;
				x[1] = -0.5 * (A + B) - a;
				x[2] = 0.5 * Math.Sqrt(3) * (A - B);
				if (Math.Abs(x[2]) < 1e-3) 
				{ 
					x[2] = x[1]; 
					return 2; 
				}

				return 1;
			}
		}*/

		//---------------------------------------------------------------------------
		// Solve quartic equation x^4 + a*x^3 + b*x^2 + c*x + d
		// (attention - this function returns dynamically allocated array. It has to be released afterwards)
		/*public static Complex[] solve_quartic(double a, double b, double c, double d)
		{
			double a3 = -b;
			double b3 = a * c - 4 * d;
			double c3 = -a * a * d - c * c + 4 * b * d;

			// cubic resolvent
			// y^3 − b*y^2 + (ac−4d)*y − a^2*d−c^2+4*b*d = 0

			//int iZeroes = solveP3(out double[] x3, a3, b3, c3);

			//ziemlich kompliziert:
			Complex[] x = SolveCubic(a3, b3, c3);
			int iZeroes = 0;
			for(int i = 0; i < 3; i++)
			{
				Complex z_ = x[i];
				if (Math.Abs(z_.Im) < 1e-6 || Math.Abs(z_.Im) < 1e-3 * Math.Abs(z_.Re)) iZeroes++;
			}
			if (iZeroes == 0 || iZeroes == 2) throw new Exception(" ");
			double[] x3 = new double[iZeroes];
			int count = 0;
			for(int i = 0; i < 3; i++)
			{
				Complex z_ = x[i];
				if (Math.Abs(z_.Im) < 1e-6) x3[count] = z_.Re; count++;
			}


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
			if (Math.Abs(D) < 1e-4) //in other words - D==0
			{
				q1 = q2 = y * 0.5;
				// g1+g2 = a && g1+g2 = b-y   <=>   g^2 - a*g + b-y = 0    (p === g)
				D = a * a - 4 * (b - y);
				if (Math.Abs(D) < 1e-3) //in other words - D==0
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
			if (D < 0)
			{
				retval[0] = new Complex(-p1 * 0.5, Math.Sqrt(-D) * 0.5);
				retval[1] = retval[0].Conj;
			}
			else
			{
				sqD = Math.Sqrt(D);
				retval[0] = new Complex((-p1 + sqD) * 0.5);
				retval[1] = new Complex((-p1 - sqD) * 0.5);
			}

			// solving quadratic eq. - x^2 + p2*x + q2 = 0
			D = p2 * p2 - 4 * q2;
			if (D < 0)
			{
				retval[2] = new Complex(-p2 * 0.5, Math.Sqrt(-D) * 0.5);
				retval[3] = retval[2].Conj;
			}
			else
			{
				sqD = Math.Sqrt(D);
				retval[2] = new Complex((-p2 + sqD) * 0.5);
				retval[3] = new Complex((-p2 - sqD) * 0.5);
			}

			Complex z;		//überprüfen
			Complex[] result = new Complex[4];
			for(int i = 0; i < 4; i++)
			{
				z = retval[i];
				result[i] = z * z * z * z + a * z * z * z + b * z * z + c * z + d;
				if(result[i].Norm > 1)
				{
					bool bo = false;
				}
			}

			return retval;
		}*/
	}
}
