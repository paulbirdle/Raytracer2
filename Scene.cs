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
            Vector p = cam.Position;
            Vector v = cam.ULCorner;
            Vector r = cam.Step_Right;
            Vector d = cam.Step_Down;

            
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = -1; // max Anzahl Threads, man kann also cpu auslastung ugf. festlegen, -1 ist unbegrenzt (halt hardware begrenzt)
            
            Parallel.For(0,resX, opt,x => // parallel mehrere Threads nutzen
            {
                for (int y = 0; y < resY; y++)
                {
                    Ray ray; // muss hier initialisiert werden, sonst reden sich die Threads gegenseitig rein -> Renderfehler
                    ray = cam.get_Rays(x, y);
                    color[x, y] = calculateRays(ray, depth);
                }
            });
            return color;
        }

        public Bitmap renderBM(int depth)
        {
            int resY = cam.resY;
            int resX = cam.resX;
            Bitmap bitmap = new Bitmap(resX, resY);
            Vector p = cam.Position;
            Vector v = cam.ULCorner;
            Vector r = cam.Step_Right;
            Vector d = cam.Step_Down;

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = -1; // max Anzahl Threads, man kann also cpu auslastung ugf. festlegen, -1 ist unbegrenzt (halt hardware begrenzt)

            Parallel.For(0, resX, opt, x => // parallel mehrere Threads nutzen
            {
                for (int y = 0; y < resY; y++)
                {
                    Ray ray = cam.get_Rays(x, y);
                    //Ray ray = new Ray(v, p);
                    Color col = calculateRays(ray, depth).Col;
                    bitmap.SetPixel(x, y, col);
                    //v += d;
                    continue;
                }
                //v += r - resY * d;
            });
            return bitmap;
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

                        if (lights[i].is_visible(intersection + 1e-10*n, entities))
                        {
                            double angle_refl_light = Vector.angle(lights[i].Direction(intersection), reflected_ray.Direction)/2;
                            double angle_n_light = Vector.angle(lights[i].Direction(intersection), n);

                            if(Math.Abs(angle_n_light) > Math.PI/2) //TODO
                            {
                                angle_n_light = Vector.angle(lights[i].Direction(intersection), -n);
                            }

                            if(material.SpecularReflectivity >= 1e-10)
                            {
                                specular += lights[i].Intensity(intersection) * specularIntensity(angle_refl_light, material.SpecularReflectivity, material.Smoothness) * lights[i].Col;
                            }
                            if(material.DiffuseReflectivity >= 1e-10)
                            {
                                diffuse  += lights[i].Intensity(intersection) * diffuseIntensity(angle_n_light, material.DiffuseReflectivity) * lights[i].Col;
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

    }
}
