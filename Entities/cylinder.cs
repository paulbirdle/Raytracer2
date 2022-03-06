using System;

namespace Raytracer
{
    class Cylinder : Entity
    {
        Vector top;
        Vector bottom;
        double radius;
        Material material;

        public Cylinder(Vector top, Vector bottom, double radius)
        {
            this.top = top;
            this.bottom = bottom;
            this.radius = radius;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {

            n = null;
            material = null;
            return -1;
        }
        public override double get_intersection(Ray ray)
        {

            return -1;
            Vector n = (top-bottom)  ^ ray.Direction;
            Vector inters1;
            Vector inters2;
            Vector inters3;

            Vector r = new Vector((top - bottom).Y, -(top - bottom).X, 0);
            r = r.normalize() * radius;

            n = n.normalize();
            double dist_side = (n * ray.Start) - (n * bottom);
            if (dist_side <= radius)
            {
                double dist_bottom = ((top - bottom) * bottom - (top - bottom) * ray.Start) / ((top - bottom) * ray.Direction); 
                double dist_top = ((bottom-top) * top - (bottom - top) * ray.Start) / ((bottom - top) * ray.Direction);

                if (dist_bottom <= radius || dist_top <= radius)
                {
                    
                }
            }
            else return -1;
        }
    }
}
