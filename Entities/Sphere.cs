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
        private double radius;
        private Vector center;
        private Material material;

        public Sphere(Vector center, double radius, Material material) 
        {
            this.radius = Math.Abs(radius);
            this.center = center;
            this.material = material;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            Vector start = ray.Start;
            Vector direction = ray.Direction;
            Vector v_to_start = start - center;

            double b = 2 * (direction * v_to_start);
            double c = v_to_start * v_to_start - radius * radius;

            double discr = b * b / 4 - c;

            if (discr < 1e-6)//quadratische Gleichung hat keine Lösung
            {
                n = new Vector();
                material = new Material();
                return -1;
            }
            double sqrt = Math.Sqrt(discr);
            double t1 = -b / 2 - sqrt;      //sonst: die beiden Lösungen (Schnittpunkte) der quadratischen Gleichung
            double t2 = -b / 2 + sqrt;

            if(t1 < 1e-6 && t2 < 1e-6) //Kugel hinter ray
            {
                n = new Vector();
                material = new Material();
                return -1;
            }
            else if(t1 < 1e-6 && t2 > 0) //start in der Kugel
            {
                n = (ray.position_at_time(t2) - center).normalize();
                material = this.material;
                return t2;
            }
            //Kugel vor ray
            n = (ray.position_at_time(t1) - center).normalize();
            material = this.material;
            return t1;
        }
    }
}
