using System.Threading;

namespace Raytracer
{
    class Ray
    {
        public static long numRay = 0;
        public Ray(Vector direction, Vector start)
        {
#if DEBUG
            Interlocked.Increment(ref numRay);
#endif
            Direction = direction.normalize();
            Start = start;
        }

        public Ray(Vector direction, Vector start, bool normalized)
        {
#if DEBUG
            Interlocked.Increment(ref numRay);
#endif
            if (normalized == true)
            {
                Direction = direction;
                Start = start;
            }
            else
            {
                Direction = direction.normalize();
                Start = start;
            }
        }

        public Ray()
        {
            numRay++;
            Start = null;
            Direction = null;
        }

        public Vector Direction { get; }

        public Vector Start { get; }

        public Vector position_at_time(double t)
        {
            return Start + t * Direction;
        }

    }
}
