using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using Math_Implementation;
using CollisionDetectionSelector.Primitives;

namespace CollisionDetectionSelector
{
    class Scene
    {
        public OBJ RootObject = new OBJ(null);

        public void Render()
        {
            RootObject.Render();
        }

        public OBJ Raycast(Ray ray, out float t)
        {
            return RecursiveRaycast(RootObject, ray, out t);
        }

        public OBJ Raycast(Ray ray)
        {
            float t = 0.0f;
            return RecursiveRaycast(RootObject, ray, out t);
        }

        protected OBJ RecursiveRaycast(OBJ current, Ray ray, out float t)
        {
            if(Collisions.Raycast(ray,current,out t))
            {
                return current;
            }
            if(current.Children!=null)
            {
                foreach (OBJ child in current.Children)
                {
                    RecursiveRaycast(child, ray, out t);  
                }
            }
            return null;
        }
    }
}
