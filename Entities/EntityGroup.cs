namespace Raytracer
{
    class EntityGroup : Entity
    {
        private readonly Entity[] entities;
        private readonly Hitentity hitentity;
        private readonly int size;
        private readonly bool hitbox;

        public EntityGroup(Entity[] entities, Hitentity hitentity)
        {
            this.entities = entities;
            this.hitentity = hitentity;
            size = entities.Length;
            hitbox = true;
        }

        public EntityGroup(Entity[] entities)
        {
            this.entities = entities;
            size = entities.Length;
            hitbox = false;
        }

        public override double get_intersection(Ray ray)
        {
            if(hitbox && !hitentity.hits(ray))
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
            if (hitbox && !hitentity.hits(ray))
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
