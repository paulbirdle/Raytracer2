using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raytracer
{
    class Scene
    {
        Camera cam;
        Entity[] entities;
        Lightsource[] lights;
        RaytracerColor ambientColor;

        public Scene(Camera cam, Entity[] entities, Lightsource[] lights, RaytracerColor ambientColor)
        {
            this.cam = cam;
            this.entities = entities;
            this.lights = lights;
            this.ambientColor = ambientColor;
        }

        public Scene(Camera cam, Entity[] entities, Lightsource[] lights)
        {
            this.cam = cam;
            this.entities = entities;
            this.lights = lights;
            this.ambientColor = new RaytracerColor(Color.FromArgb(20, 20, 20)); //standard: grau
        }

        public RaytracerColor[,] render(int depth)
        {
            RaytracerColor[,] color = new RaytracerColor[cam.resX, cam.resY];
            Vector p = cam.Position;
            Vector v = cam.ULCorner;
            Vector r = cam.Step_Right;
            Vector d = cam.Step_Down;
            int resY = cam.resY;
            int resX = cam.resX;
            
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = -1; // max Anzahl Threads, man kann also cpu auslastung ugf. festlegen, -1 ist unbegrenzt (halt hardware begrenzt)

            Parallel.For(0,resX, opt,x => // parallel mehrere Threads nutzen
            {
                for (int y = 0; y < resY; y++)
                {
                      Ray ray = cam.get_Rays(x, y);
                      color[x, y] = calculateRays(ray, depth);
                }
            });
            return color;
        }

        private int firstIntersection(Ray ray, out Vector intersection, out double t, out Vector n, out Material material)
        {
            t = double.PositiveInfinity;
            double currentDist;
            int collisionEntity = -1;

            for(int l = 0; l < entities.Length; l++)
            {
                if (entities[l] == null) continue;

                currentDist = entities[l].get_intersection(ray); //, out Vector n_tmp, out Material material_tmp

                if (currentDist < t && currentDist > 1e-6)
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

        private RaytracerColor calculateRays(Ray ray, int depth)
        {
            if(depth < 0)
            {
                throw new Exception("depth darf nicht negativ sein!");
            }
            if(depth == 0)
            {
                int l = firstIntersection(ray, out _, out _, out _, out Material material);
                /*if(l == -1)//nichts getroffen
                {*/
                    return ambientColor;
                /*}
                else //etwas getroffen
                {
                    return material.Col;
                }*/
            }
            else //depth > 0
            {
                int l = firstIntersection(ray, out Vector intersection, out _, out Vector n, out Material material);
                if(l == -1) //nichts getroffen
                {
                    return ambientColor;
                }
                else //etwas getroffen
                {
                    Vector v = ray.Direction;
                    Ray reflected_ray = new Ray((-v).reflect_at(n), intersection);
                    RaytracerColor reflected_col;
                    if (material.Reflectivity < 1e-6)
                    {
                        reflected_col = RaytracerColor.Black;
                    }
                    else
                    {
                        reflected_col = material.Reflectivity * calculateRays(reflected_ray, depth - 1);
                    }

                    RaytracerColor specular = RaytracerColor.Black;
                    RaytracerColor diffuse = RaytracerColor.Black;


                    for (int i = 0; i < lights.Length; i++)
                    {
                        if(lights[i] == null) continue;
                        if(lights[i].is_visible(intersection + 1e-6*n, entities))
                        {
                            double angle_refl_light = Vector.angle(lights[i].Direction(intersection), reflected_ray.Direction)/2;
                            double angle_n_light = Vector.angle(lights[i].Direction(intersection), n);
                            specular += lights[i].Intensity(intersection) * specularIntensity(angle_refl_light, material.SpecularReflectivity, material.Smoothness) * lights[i].Col;
                            diffuse += lights[i].Intensity(intersection) * diffuseIntensity(angle_n_light, material.DiffuseReflectivity) * lights[i].Col;
                        }
                    }
                    RaytracerColor result = material.Col * (diffuse + reflected_col + specular);
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
    }
}
