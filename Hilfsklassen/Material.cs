using System.Drawing;

namespace Raytracer
{
    class Material
    {
        private readonly RaytracerColor color;
        private readonly double reflectivity;   //0: reflektiert nicht         1: reflektiert 100%
        private readonly double smoothness;     //0: mega grob                 infty: perfekt glatt
        private readonly double specularReflectivity; //0-1
        private readonly double diffuseReflectivity;  //0-1


        public Material()
        {
            color = new RaytracerColor(Color.Black);
            reflectivity = 0;
            smoothness = 0;
            specularReflectivity = 0;
            diffuseReflectivity = 0;
        }

        public Material(RaytracerColor color, double reflectivity, double smoothness)
        {
            this.color = color;
            this.smoothness = smoothness;
            this.reflectivity = reflectivity;
            specularReflectivity = 0.8;
            diffuseReflectivity = 0.2;
        }

        public Material(RaytracerColor color, double reflectivity, double smoothness, double specularReflectivity, double diffuseReflectivity)
        {
            this.color = color;
            this.smoothness = smoothness;
            this.reflectivity = reflectivity;
            this.specularReflectivity = specularReflectivity;
            this.diffuseReflectivity = diffuseReflectivity;
        }

        public RaytracerColor Col
        {
            get { return color; }
        }

        public double Smoothness
        {
            get { return smoothness; }
        }

        public double Reflectivity
        {
            get { return reflectivity; }
        }

        public double DiffuseReflectivity
        {
            get { return diffuseReflectivity; }
        }

        public double SpecularReflectivity
        {
            get { return specularReflectivity; }
        }

        public static Material Iron
        {
            get { return new Material(RaytracerColor.Brown, 0, 0); }
        }

        public static Material PolishedMetal
        {
            get { return new Material(RaytracerColor.Gray, 0.3, 0.8); }
        }

        public static Material Mirror
        {
            get { return new Material(RaytracerColor.White, 1.0, 0.9, 0, 0); }
        }

        public static Material TealBG
        {
            get{ return new Material(RaytracerColor.Teal, 0, 0.5, 0.3, 0.7); }
        }

        public static Material RoughLight
        {
            get { return new Material(RaytracerColor.White, 0, 0); }
        }
        public static Material MetalicRed
        {
            get { return new Material(new RaytracerColor(Color.Red), 1, 70, 0.7, 0.15); }
        }
        public static Material Matte
        {
            get { return new Material(new RaytracerColor(Color.Gray),0 ,20 ,0.2 ,0.8 ); }
        }
    }
}
