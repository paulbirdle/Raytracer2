using System;

namespace Raytracer
{
    class Portal : Entity
    {
        Entity portalShape;
        Scene dimension;

        public Portal(Entity shape, Scene dimension)
        {
            portalShape = shape;
            Entity[] e = new Entity[dimension.giveEntities().Length];
            dimension.giveEntities().CopyTo(e, 0);
            Lightsource[] l = new Lightsource[dimension.giveLights().Length];
            dimension.giveLights().CopyTo(l, 0);
            this.dimension = new Scene(dimension.giveCamera(),e,l);


            for(int i = 0; i<e.Length; i++)
            {
                if(e[i] is Portal)
                {
                    throw new Exception("KEIN PORTAL IM PORTAL");
                }
            }

        }
        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            n = null;
            material = null;
            return -1;
        }
        public override double get_intersection(Ray ray)
        {
            return portalShape.get_intersection(ray);
        }

        public RaytracerColor look_into_Dimension(Ray ray, int depth)
        {
            Entity[] e = dimension.giveEntities();
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i] is Portal)
                {
                    throw new Exception("KEIN PORTAL IM PORTAL");
                }
            }
            return dimension.calculateRays(ray, depth);
        }
    }
}
