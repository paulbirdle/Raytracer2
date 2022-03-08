using System;

namespace Raytracer
{
    class Cylinder : Entity
    {
        Vector top;
        Vector bottom;
        double radius;
        Material material;

        public Cylinder(Vector top, Vector bottom, double radius, Material m)
        {
            this.top = top;
            this.bottom = bottom;
            this.radius = radius;
            material = m;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            material = this.material;
            Vector o = ray.Start - bottom;
            double a = (ray.Direction.X * ray.Direction.X) + (ray.Direction.Y * ray.Direction.Y);
            double b = (ray.Direction.X * o.X) + (ray.Direction.Y * o.Y);
            double c = o.X * o.X + o.Y * o.Y + radius * radius;

            if (a == 0)
            {
                n = null;
                return -1;
            }
            double t1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            if (t1 is double.NaN || t2 is double.NaN)
            {
                n = null;
                return -1;
            }
            if (t1 < t2)
            {
                n = new Vector(-1, -1, 0);
                return t1;
            }
            else
            {
                n = new Vector(-1, -1, 0);
                return t2;
            }
        }

        public override double get_intersection(Ray ray)
        {
            Vector n = (top - bottom) ^ ray.Direction;
            n = n.normalize();
            double dist_side = (n * ray.Start) - (n * bottom);
            if (dist_side > radius)
            {
                return -1;
            }

            Vector o = ray.Start - bottom;
            double a = (ray.Direction.X * ray.Direction.X) + (ray.Direction.Y * ray.Direction.Y);
            double b = (ray.Direction.X * o.X) + (ray.Direction.Y * o.Y);
            double c = o.X * o.X + o.Y * o.Y + radius * radius;

            if (a == 0)
            {
                return -1;
            }
            double t1 = (-b + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
            double t2 = (-b - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);

            if (t1 is double.NaN || t2 is double.NaN)
            {
                return -1;
            }
            if (t1 < t2)
            {
                return t1;
            }
            else return t2;


            //Vector n = (top-bottom)  ^ ray.Direction;
            //Vector inters1;
            //Vector inters2;
            //Vector inters3;

            //Vector r = new Vector((top - bottom).Y, -(top - bottom).X, 0);
            //r = r.normalize() * radius;

            //n = n.normalize();
            //double dist_side = (n * ray.Start) - (n * bottom);
            //if (dist_side <= radius)
            //{
            //    double dist_bottom = ((top - bottom) * bottom - (top - bottom) * ray.Start) / ((top - bottom) * ray.Direction); 
            //    double dist_top = ((bottom-top) * top - (bottom - top) * ray.Start) / ((bottom - top) * ray.Direction);

            //    if (dist_bottom <= radius || dist_top <= radius)
            //    {

            //    }
            //}
            //else return -1;
        }
    }
}
