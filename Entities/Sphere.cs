using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Sphere : Entity
    {
        private readonly double radius;
        private readonly Vector center;
        private readonly Material material;

        private readonly double squarerad;

        public Sphere(Vector center, double radius, Material material) 
        {
            this.radius = Math.Abs(radius);
            this.center = center;
            this.material = material;

            squarerad = radius * radius;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = direction * v_to_start;
            double c = v_to_start * v_to_start - squarerad;

            double discr = b * b - c;

            if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
            {
                n = null;
                material = null;
                return -1;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b + sqrt;

            if(t1 < 1e-10 && t2 < 1e-10) //Kugel hinter ray
            {
                n = null;
                material = null;
                return -1;
            }
            else if(t1 < 1e-10 && t2 > 0) //start in der Kugel
            {
                n = (center - ray.position_at_time(t2)).normalize();
                material = this.material;
                return t2;
            }
            //Kugel vor ray
            n = (ray.position_at_time(t1) - center).normalize();
            material = this.material;
            return t1;
        }

        public override double get_intersection(Ray ray)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = direction * v_to_start;
            double c = v_to_start * v_to_start - squarerad;

            double discr = b * b - c;

            if (discr < 1e-10)//quadratische Gleichung hat keine Lösung
            {
                return -1;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b + sqrt;

            if (t1 < 1e-10 && t2 < 1e-10) //Kugel hinter ray
            {
                return -1;
            }
            else if (t1 < 1e-10 && t2 > 0) //start in der Kugel
            {
                return t2;
            }
            //Kugel vor ray
            return t1;
        }
    }
}
