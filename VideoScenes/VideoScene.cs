using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Raytracer
{
    class VideoScene
    {
        protected Scene[] scenes;
        protected int n; //Anzahl frames

        public VideoScene (Scene[] scenes)
        {
            this.scenes = scenes;
            n = scenes.Length;
        }

        public VideoScene()
        {
            this.scenes = new Scene[0];
            n = 0;
        }

        public Scene[] Scenes
        {
            get { return scenes; }
        }

        public static VideoScene operator +(VideoScene A, VideoScene B)
        {
            return new VideoScene(A.Scenes.ToList().Concat(B.Scenes.ToList()).ToArray());
        }
    }
}
