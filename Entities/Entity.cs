using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer2
{
    class Entity
    {
        public Entity()
        {
            
        }

        public virtual double get_intersection(Ray ray, out Vector n, out Material material)
        {
            //return -1: kein Schnittpunkt
            //sonst Abstand von Ray origin zu Schnittpunkt
            n = new Vector();
            material = new Material();
            return -1;
        }
    }
}
