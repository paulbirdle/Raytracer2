using System;

namespace Raytracer
{
    class Cone : Entity
    {
        private readonly Vector tip;
        private readonly Vector dir;
        private readonly double r;
        private readonly double h;
        private readonly Material material;

        private readonly Disk disk;
        private readonly ConeEnvelope envelope;
        private readonly EntityGroup group;

        public Cone(Vector tip, Vector direction, double radius, double height, Material material)
        {
            this.tip = tip;
            dir = direction.normalize();
            r = Math.Abs(radius);
            h = Math.Abs(height);
            this.material = material;

            disk = new Disk(tip + h * dir, dir, r, material);
            envelope = new ConeEnvelope(tip, dir, r, h, material);

            group = new EntityGroup(new Entity[2] { disk, envelope }, new Hitsphere(tip + (h/2)*dir, Math.Sqrt(h*h + r*r)/2));
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
