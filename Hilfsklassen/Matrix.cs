using System;

namespace Raytracer
{
    class Matrix
    {
        double[,] A;

        public Matrix (double[,] A)
        {
            if (A.GetLength(0) == 3 && A.GetLength(1) == 3)
            {
                this.A = A;
            }
            else
            {
                throw new Exception("Matrix nicht 3x3");
            }
        }

        public Matrix()
        {
            A = new double[3, 3];
        }

        public double Get(int i, int j)
        {
            return A[i, j];
        }

        public void Set(int i, int j, double value)
        {
            A[i, j] = value;
        }

        public static Vector operator *(Matrix A, Vector v)
        {
            double x = A.Get(0, 0) * v.X + A.Get(0, 1) * v.Y + A.Get(0, 2) * v.Z;
            double y = A.Get(1, 0) * v.X + A.Get(1, 1) * v.Y + A.Get(1, 2) * v.Z;
            double z = A.Get(2, 0) * v.X + A.Get(2, 1) * v.Y + A.Get(2, 2) * v.Z;
            return new Vector(x, y, z);
        }
        
        public static Matrix operator *(Matrix A, Matrix B)
        {
            Matrix C = new Matrix();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    double val = 0;
                    for(int k = 0; k < 3; k++)
                    {
                        val += A.Get(i, k) * B.Get(k, j);
                    }
                    C.Set(i, j, val);
                }
            }
            return C;
        }

        public static Matrix operator *(double a, Matrix A)
        {
            Matrix B = new Matrix();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    B.Set(i,j,a*A.Get(i,j));
                }
            }
            return B;
        }

        public static Matrix operator *(Matrix A, double a)
        {
            return a*A;
        }

        public static Matrix operator +(Matrix A, Matrix B)
        {
            Matrix C = new Matrix();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    C.Set(i, j, A.Get(i, j) + B.Get(i, j));
                }
            }
            return C;
        }

        public static Matrix operator -(Matrix A, Matrix B)
        {
            return A + (-1) * B;
        }

        public double Norm()
        {
            double sum = 0;
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; i < 3; j++)
                {
                    sum += Math.Pow(this.Get(i,j),2);
                }
            }
            return Math.Sqrt(sum);
        }

        public static Matrix Transpose(Matrix A)
        {
            Matrix B = new Matrix();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    B.Set(i, j, A.Get(j, i));
                }
            }
            return B;
        }

        public static Matrix Id()
        {
            Matrix I = new Matrix();
            for(int i = 0; i < 3; i++)
            {
                I.Set(i,i,1);
            }
            return I;
        }

        public static Matrix Rotation(double angle) //around x axis
        {
            Matrix A = new Matrix();
            A.Set(0, 0, 1);
            A.Set(1, 1, Math.Cos(angle));
            A.Set(1, 2, -Math.Sin(angle));
            A.Set(2, 1, Math.Sin(angle));
            A.Set(2, 2, Math.Cos(angle));

            return A;
        }

        public static Matrix Rotation(Vector axis, double angle)
        {
            Matrix omega = new Matrix();
            omega.Set(0,1,-axis.Z);
            omega.Set(0,2,axis.Y);
            omega.Set(1,0,axis.Z);
            omega.Set(1,2,-axis.X);
            omega.Set(2,0,-axis.Y);
            omega.Set(2,1,axis.X);
            return Id() + omega * Math.Sin(angle) + omega * omega* (1 - Math.Cos(angle));
        }

        public bool Is_orthogonal()
        {
            return (Transpose(this)*this - Id()).Norm() < 1e-10;
        }
    }
}
