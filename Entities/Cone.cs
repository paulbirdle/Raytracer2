using System;

namespace Raytracer
{
    class Cone : Entity
    {
        private readonly EntityGroup group; //{disk,  envelope}

        public Cone(Vector tip, Vector direction, double radius, double height, Material material)
        {
            Vector dir = direction.normalize();
            double r = Math.Abs(radius);
            double h = Math.Abs(height);

            Disk disk = new Disk(tip + h * dir, dir, r, material);
            ConeEnvelope envelope = new ConeEnvelope(tip, dir, r, h, material);
            group = new EntityGroup(new Entity[2] { disk, envelope }, new Hitsphere(tip + (h / 2) * dir, Math.Sqrt(h * h / 4 + r * r)));
        }

        public Cone(Vector tip, Vector direction, double radius, double height, Material envelopeMaterial, Material diskMaterial)
        {
            Vector dir = direction.normalize();
            double r = Math.Abs(radius);
            double h = Math.Abs(height);

            Disk disk = new Disk(tip + h * dir, dir, r, diskMaterial);
            ConeEnvelope envelope = new ConeEnvelope(tip, dir, r, h, envelopeMaterial);
            group = new EntityGroup(new Entity[2] { disk, envelope }, new Hitsphere(tip + (h / 2) * dir, Math.Sqrt(h * h / 4 + r * r)));
        }

        public override double get_intersection(Ray ray, out Vector n, out Material material)
        {
            return group.get_intersection(ray, out n, out material);
        }

        public override double get_intersection(Ray ray)
        {
            return group.get_intersection(ray);
        }
    }
}
