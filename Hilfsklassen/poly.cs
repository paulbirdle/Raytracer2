using System;

namespace Raytracer
{
	static class Poly //TODO: Toleranzen
	{
		public static Complex[] SolveCubic(double[] A) //zeros of f = t^3 + a0*t^2 + a1*t + a2 
		{
			double a = A[0];
			double b = A[1];
			double c = A[2];
			double s = a / 3;
			double p = b - a * a / 3;
			double q = 2 * a * a * a / 27 - a * b / 3 + c;

			double D = q * q / 4 + p * p * p / 27;
			Complex sqrtD = Complex.Sqrt(D);

			Complex u1 = Complex.CubeRoot(-q / 2 + sqrtD);
			Complex v1 = -p / (3 * u1);

			//Complex should_be_zero = Complex.Pow(u1, 6) + q * Complex.Pow(u1, 3) - Math.Pow(p / 3.0, 3);

			Complex xi = Complex.Exp(Complex.i * 2 * Math.PI / 3);
			Complex xi2 = xi * xi;

			return new Complex[3] { u1 + v1 - s, xi * u1 + xi2 * v1 - s, xi2 * u1 + xi * v1 - s};
		}

		public static Complex[] SolveCubic(Complex[] A) //zeros of f = t^3 + a0*t^2 + a1*t + a2 
		{
			Complex a = A[0];
			Complex b = A[1];
			Complex c = A[2];
			Complex s = a / 3;
			Complex p = b - a * a / 3;
			Complex q = 2 * a * a * a / 27 - a * b / 3 + c;

			Complex D = q * q / 4 + p * p * p / 27;
			Complex sqrtD = Complex.Pow(D, 1/2.0);

			Complex u1 = Complex.CubeRoot(-q / 2 + sqrtD);
			Complex v1 = -p / (3 * u1);

			//Complex should_be_zero = Complex.Pow(u1, 6) + q * Complex.Pow(u1, 3) - Complex.Pow(p / 3.0, 3);

			Complex xi = Complex.Exp(Complex.i * 2 * Math.PI / 3);
			Complex xi2 = xi * xi;

			return new Complex[3] { u1 + v1 - s, xi * u1 + xi2 * v1 - s, xi2 * u1 + xi * v1 - s };
		}


		public static Complex[] SolveQuartic(double[] coeff)
		{
			double a = coeff[0];
			double b = coeff[1];
			double c = coeff[2];
			double d = coeff[3];

			double s = a / 4;

			//coefficients of the depressed quartic:
			double p = b - 3 * a * a / 8;
			double q = c - a * b / 2 + a * a * a / 8;
			double r = d - a * c / 4 + b * a * a / 16 - 3 * a * a * a * a / 256;

			if (Math.Abs(q) < 1e-6)//biquadratische Gleichung
			{
				double D = p * p / 4 - r;
				Complex sqrtD = Complex.Sqrt(D);
				return new Complex[4] { Complex.Pow(-p / 2 + sqrtD, 1 / 2.0) - s, Complex.Pow(-p / 2 - sqrtD, 1 / 2.0) - s, -Complex.Pow(-p / 2 + sqrtD, 1 / 2.0) - s, -Complex.Pow(-p / 2 - sqrtD, 1 / 2.0) - s };
			}
			if (Math.Abs(r) < 1e-6)//kein konstanter Term
			{
				Complex[] cubicRoots = SolveCubic(new double[2] { p, q });
				return new Complex[4] { cubicRoots[0] - s, cubicRoots[1] - s, cubicRoots[2] - s, new Complex(0) - s };
			}
			//allgemeiner Fall
			double[] resolventCoefficients = new double[3] { 2 * p, p * p - 4 * r, -q * q }; //alpha bestimmen
			Complex[] CubicRoots = SolveCubic(resolventCoefficients);
			Complex alphaSquared = CubicRoots[0];
			//Complex alpha = Complex.Pow(alphaSquared, 1 / 2.0);

			Complex D_ = (p + alphaSquared) * (p + alphaSquared) / 4 - r;					//beta bestimmen
			Complex sqrtD_ = Complex.Pow(D_, 1 / 2.0);
			Complex beta = (p + alphaSquared) / 2 + sqrtD_;
			Complex alpha = -q / (beta - r / beta);											//Vorzeichen von alpha

			//Complex[] shouldBeZero = new Complex[2] { p + alphaSquared - beta - r / beta, q / alpha + beta - r / beta };
			
			Complex D1 = alpha * alpha / 4 - beta;											//quadratische Gleichungen lösen
			Complex sqrtD1 = Complex.Pow(D1, 1 / 2.0);
			Complex D2 = alpha * alpha / 4 - r / beta;
			Complex sqrtD2 = Complex.Pow(D2, 1 / 2.0);

			return new Complex[4] { -alpha / 2 + sqrtD1 - s, -alpha / 2 - sqrtD1 - s, alpha / 2 + sqrtD2 - s, alpha / 2 - sqrtD2 - s };
		}

		public static double GetSmallestPositiveRoot(double[] coeff)
		{
			Complex[] z_ = SolveQuartic(coeff);
			double tmin = double.PositiveInfinity;
			double t;
			Complex z;
			for(int i = 0; i < 4; i++)
			{
				z = z_[i];
				if(Math.Abs(z.Im) < 1e-6)
				{
					t = z.Re;
					if(t > 1e-6 && t < tmin)
					{
						tmin = t;
					}
				}
			}
			if (tmin == double.PositiveInfinity) return -1;
			return tmin;
		}
	}
}
