using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public double get(int i, int j)
        {
            return A[i, j];
        }

        public void set(int i, int j, double value)
        {
            A[i, j] = value;
        }

        public static Vector operator *(Matrix A, Vector v)
        {
            double x = A.get(0, 0) * v.X + A.get(0, 1) * v.Y + A.get(0, 2) * v.Z;
            double y = A.get(1, 0) * v.X + A.get(1, 1) * v.Y + A.get(1, 2) * v.Z;
            double z = A.get(2, 0) * v.X + A.get(2, 1) * v.Y + A.get(2, 2) * v.Z;
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
                        val += A.get(i, k) * B.get(k, j);
                    }
                    C.set(i, j, val);
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
                    B.set(i,j,a*A.get(i,j));
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
                    C.set(i, j, A.get(i, j) + B.get(i, j));
                }
            }
            return C;
        }

        public static Matrix operator -(Matrix A, Matrix B)
        {
            return A + (-1) * B;
        }

        public double norm()
        {
            double sum = 0;
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; i < 3; j++)
                {
                    sum += Math.Pow(this.get(i,j),2);
                }
            }
            return Math.Sqrt(sum);
        }

        public static Matrix transpose(Matrix A)
        {
            Matrix B = new Matrix();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    B.set(i, j, A.get(j, i));
                }
            }
            return B;
        }

        public static Matrix Id()
        {
            Matrix I = new Matrix();
            for(int i = 0; i < 3; i++)
            {
                I.set(i,i,1);
            }
            return I;
        }

        public static Matrix Rotation(double angle) //around x axis
        {
            Matrix A = new Matrix();
            A.set(0, 0, 1);
            A.set(1, 1, Math.Cos(angle));
            A.set(1, 2, -Math.Sin(angle));
            A.set(2, 1, Math.Sin(angle));
            A.set(2, 2, Math.Cos(angle));

            return A;
        }

        public static Matrix Rotation(Vector axis, double angle)
        {
            /*Matrix T = move_to_x_axis(axis);
            Matrix A = Rotation(angle);

            return transpose(T) * A * T;*/

            Matrix omega = new Matrix();
            omega.set(0,1,-axis.Z);
            omega.set(0,2,axis.Y);
            omega.set(1,0,axis.Z);
            omega.set(1,2,-axis.X);
            omega.set(2,0,-axis.Y);
            omega.set(2,1,axis.X);

            return Id() + omega * Math.Sin(angle) + omega * omega* (1 - Math.Cos(angle));
        }

        public bool is_orthogonal()
        {
            return (transpose(this)*this - Id()).norm() < 1e-6;
        }
    }
}
