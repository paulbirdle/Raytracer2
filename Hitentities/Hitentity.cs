using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Hitentity
    {
        public virtual bool hits(Ray ray)
        {
            return true;
        }
    }
}
