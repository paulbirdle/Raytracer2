namespace Raytracer
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

        public virtual double get_intersection(Ray ray)
        {
            return -1;
        }
    }
}
