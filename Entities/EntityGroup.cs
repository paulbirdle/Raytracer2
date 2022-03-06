using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class EntityGroup : Entity
    {
        private Entity[] entities;
        private Hitentity hitentity;
        private int size;

        public EntityGroup(Entity[] entities, Hitentity hitentity)
        {
            this.entities = entities;
            this.hitentity = hitentity;
            size = entities.Length;
        }

        public override double get_intersection(Ray ray)
        {
            if(!hitentity.hits(ray))
            {
                return -1;
            }

            double tmin = double.PositiveInfinity;
            double t;
            for(int i = 0; i < size; i++)
            {
                t = entities[i].get_intersection(ray);
                if (t < tmin && t > 1e-10) tmin = t;
            }
            if (tmin == double.PositiveInfinity) return -1;
            return tmin;
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            if (!hitentity.hits(ray))
            {
                n = null;
                material = null;
                return -1;
            }

            double tmin = double.PositiveInfinity;
            double t;
            int l = -1;

            for (int i = 0; i < size; i++)
            {
                if (entities[i] == null) continue;

                t = entities[i].get_intersection(ray);

                if (t > 1e-10 && t < tmin)
                {
                    tmin = t;
                    l = i;
                }
            }

            if (l == -1)//nicht getroffen
            {
                n = null;
                material = null;
                return -1;
            }
            else //getroffen
            {
                entities[l].get_intersection(ray, out n, out material);
                return tmin;
            }
        }
    }
}
