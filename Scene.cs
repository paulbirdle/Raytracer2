using System;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Scene
    {
        private readonly Camera cam;
        private readonly Entity[] entities;
        private readonly Lightsource[] lights;
        private readonly RaytracerColor ambientColor;

        readonly int parallelism = -1;

        public delegate void ProgressUpdate(int value);
        public event ProgressUpdate OnProgressUpdate;

        public event Action<int> ProgressChanged;
        public volatile int progress = 0;


        public Scene(Camera cam, Entity[] entities, Lightsource[] lights)
        {
            this.cam = cam;
            this.entities = entities;
            this.lights = lights;
            this.ambientColor = new RaytracerColor(Color.FromArgb(20, 20, 20)); //standard: grau
        }
        public Scene(Camera cam, Entity[] entities, Lightsource[] lights, RaytracerColor ambientColor)
        {
            this.cam = cam;
            this.entities = entities;
            this.lights = lights;
            this.ambientColor = ambientColor;
        }

        public RaytracerColor[,] render(int depth)
        {
            int resY = cam.resY;
            int resX = cam.resX;
            RaytracerColor[,] color = new RaytracerColor[resX, resY];

            ParallelOptions opt = new ParallelOptions { MaxDegreeOfParallelism = parallelism };

            int a = 10; //Anzahl for-Schleifen, die zu einer Parallel.for zusammengefasst werden
            int outer = (int)Math.Ceiling(resX / (double)a);
            for (int X = 0; X < outer; X++)
            {
                Parallel.For(a * X, Math.Min(resX, a * (X + 1)), opt, x =>
                      {
                          for (int y = 0; y < resY; y++)
                          {
                              Ray ray;
                              ray = cam.get_Rays(x, y);
                              color[x, y] = calculateRays(ray, depth);
                          }
                      });
                progress = (int)(100 * (double)a * (X+1) / resX);
                OnProgressChanged(progress);
                OnProgressUpdate?.Invoke(progress);
            }

            /*Parallel.For(0, resX, opt, x =>
            {
                for (int y = 0; y < resY; y++)
                {
                    Ray ray;
                    ray = cam.get_Rays(x, y);
                    color[x, y] = calculateRays(ray, depth);
                }
            });*/

            return color;
        }

        public RaytracerColor[,] msaa(RaytracerColor[,] col, bool[,] edges, int msaa, int depth)
        {
            Camera camera = new Camera(cam.Position, cam.Direction, cam.Up, cam.xAngle, cam.resX * msaa, cam.resY * msaa);

            int resY = cam.resY;
            int resX = cam.resX;
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = parallelism; // max Anzahl Threads, man kann also cpu auslastung ugf. festlegen, -1 ist unbegrenzt (halt hardware begrenzt)

            int a = 10;
            int outer = (int)Math.Ceiling(resX / (double)a);
            for (int X = 0; X < outer; X++)
            {
                Parallel.For(a * X, Math.Min(resX, a * (X + 1)), opt, x =>
                {
                    for (int y = 0; y < resY; y++)
                    {
                        if (edges[x, y])
                        {
                            Ray ray; // muss hier initialisiert werden, sonst reden sich die Threads gegenseitig rein -> Renderfehler
                            int actX;
                            int actY;
                            RaytracerColor[] c = new RaytracerColor[msaa * msaa]; // in dieses array werden die Farben eines Blocks reingeschrieben, um sie dann als avg auf die BM zu schreiben
                            int i;
                            int j;

                            actX = msaa * x;
                            actY = msaa * y;
                            for (int s1 = 0; s1 < msaa; s1++)
                            {
                                for (int s2 = 0; s2 < msaa; s2++)
                                {
                                    i = actX + s1;
                                    j = actY + s2;
                                    ray = camera.get_Rays(i, j);
                                    c[msaa * s1 + s2] = calculateRays(ray, depth);
                                }
                            }
                            col[x, y] = new RaytracerColor(RaytracerColor.avg(c));
                        }
                    }
                });
                progress = (int)(100 * (double)a * (X+1) / resX);
                OnProgressChanged(progress);
                OnProgressUpdate?.Invoke(progress);
            }
            return col;
        }

        private int firstIntersection(Ray ray, out Vector intersection, out double t, out Vector n, out Material material)
        {
            t = double.PositiveInfinity;
            double currentDist;
            int collisionEntity = -1;

            for(int l = 0; l < entities.Length; l++)
            {
                if (entities[l] == null) continue;

                currentDist = entities[l].get_intersection(ray); 

                if (currentDist < t && currentDist > 1e-10)
                {
                    t = currentDist;
                    collisionEntity = l;
                }
            }

            if (collisionEntity == -1)//nichts getroffen
            {
                intersection = null; // schneller als neue Instanzen zu erstellen
                n = null;
                material = null;
                return -1;
            }
            else //etwas getroffen
            {
                intersection = ray.position_at_time(t);
                entities[collisionEntity].get_intersection(ray, out n, out material);
                return collisionEntity;
            }
        }

        public RaytracerColor calculateRays(Ray ray, int depth)
        {
            if(depth < 0)
            {
                throw new Exception("depth darf nicht negativ sein!");
            }
            if(depth == 0)
            {
                return ambientColor;
            }
            else //depth > 0
            {
                int l = firstIntersection(ray, out Vector intersection, out _, out Vector n, out Material material);
                if(l == -1) //nichts getroffen
                {
                    return ambientColor;
                }
                else if(entities[l] is Portal)
                {
                    Portal portal = ((Portal)entities[l]);
                    RaytracerColor result = portal.look_into_Dimension(ray, depth);
                    return result;
                }
                else //etwas getroffen
                {
                    Vector v = ray.Direction;
                    Ray reflected_ray = new Ray((-v).reflect_at(n), intersection);
                    RaytracerColor reflected_col;
                    if (material.Reflectivity < 1e-10)
                    {
                        reflected_col = RaytracerColor.Black;
                    }
                    else
                    {
                        //if (material.Reflectivity > 1 || material.Reflectivity < 0) throw new Exception(" hier fehler");
                        reflected_col = material.Reflectivity * calculateRays(reflected_ray, depth - 1);
                    }

                    RaytracerColor specular = RaytracerColor.Black;
                    RaytracerColor diffuse = RaytracerColor.Black;


                    for (int i = 0; i < lights.Length; i++)
                    {
                        if(lights[i] == null) continue;

                        double intensity = lights[i].Intensity(intersection + 1e-10 * n, entities);
                        if (intensity > 0)
                        {
                            double angle_refl_light = Vector.angle(lights[i].Direction(intersection), reflected_ray.Direction)/2;
                            double angle_n_light = Vector.angle(lights[i].Direction(intersection), n);

                            if(Math.Abs(angle_n_light) > Math.PI/2) //TODO
                            {
                                angle_n_light = Vector.angle(lights[i].Direction(intersection), -n);
                            }

                            if(material.SpecularReflectivity >= 1e-10)
                            {
                                specular += intensity * specularIntensity(angle_refl_light, material.SpecularReflectivity, material.Smoothness) * lights[i].Col;
                            }
                            if(material.DiffuseReflectivity >= 1e-10)
                            {
                                diffuse  += intensity * diffuseIntensity(angle_n_light, material.DiffuseReflectivity) * lights[i].Col;
                            }
                        }
                    }
                    RaytracerColor result = material.Col* (diffuse + reflected_col + specular);
                    return result;
                }
            }
        }

        public double specularIntensity(double angle, double specularReflectivity, double n_spec)
        {
            return specularReflectivity * Math.Pow(Math.Cos(angle), n_spec);
        }

        public double diffuseIntensity(double angle, double diffuseReflectivity)
        {
            return diffuseReflectivity * Math.Cos(angle);
        }

        public Entity[] giveEntities()
        {
            return entities;
        }
        public Lightsource[] giveLights()
        {
            return lights;
        }
        public Camera giveCamera()
        {
            return cam;
        }
        public RaytracerColor giveAmbientColor()
        {
            return ambientColor;
        }


        private void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }

    }
}
