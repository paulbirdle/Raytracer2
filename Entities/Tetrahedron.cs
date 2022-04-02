namespace Raytracer
{
    class Tetrahedron : Entity
    {
        EntityGroup group;

        public Tetrahedron(Vector[] corners, Material material)
        {
            Entity[] entities = new Entity[4];
            for (int i = 0; i < 4; i++)
            {
                entities[i] = new Triangle(new Vector[3] { corners[(i + 1) % 4], corners[(i + 2) % 4], corners[(i + 3) % 4] }, material);
            }
            group = new EntityGroup(entities);
        }

        public Tetrahedron(Vector[] corners, Material[] materials)
        {
            Entity[] entities = new Entity[4];
            for (int i = 0; i < 4; i++)
            {
                entities[i] = new Triangle(new Vector[3] { corners[(i + 1) % 4], corners[(i + 2) % 4], corners[(i + 3) % 4] }, materials[i]);
            }
            group = new EntityGroup(entities);
        }

        public Tetrahedron(Vector corner1, Vector corner2, Vector corner3, Vector corner4, Material material1, Material material2, Material material3, Material material4)
            : this(new Vector[4] { corner1, corner2, corner3, corner4 }, new Material[4] { material1, material2, material3, material4 })
        { }

        public Tetrahedron(Vector corner1, Vector corner2, Vector corner3, Vector corner4, Material material)
            : this(new Vector[4] { corner1, corner2, corner3, corner4 }, material)
        { }

        public override double get_intersection(Ray ray)
        {
            return group.get_intersection(ray);
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            return group.get_intersection(ray, out n, out material);
        }
    }
}
