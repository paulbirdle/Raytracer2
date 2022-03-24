namespace Raytracer
{
    class HitEllipsoid : Hitentity
    {
        private readonly Vector center;
        private readonly Matrix B;

        public HitEllipsoid(Vector center, double[] a, Vector axis1, Vector axis2)
        {
            this.center = center;
            axis1 = axis1.normalize();
            axis2 = (axis2 - axis2 * axis1 * axis1).normalize();
            Vector axis3 = axis1 ^ axis2;
            B = new Matrix(new double[3, 3] { { axis1.X / a[0], axis1.Y / a[0], axis1.Z / a[0] }, { axis2.X / a[1], axis2.Y / a[1], axis2.Z / a[1] }, { axis3.X / a[2], axis3.Y / a[2], axis3.Z / a[2] } });
        }

        public override bool hits(Ray ray)
        {
            Vector s = ray.Start;
            Vector v = ray.Direction;
            Vector x = s - center;

            x = B * x;
            v = B * v;

            double a = v * v;
            double b = 2 * v * x;
            double c = x * x - 1;

            return Poly.GetSmallestPositiveRoot(new double[2] { b / a, c / a }) != -1;
        }
    }
}
