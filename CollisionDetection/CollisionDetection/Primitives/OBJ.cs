using OpenTK.Graphics.OpenGL;
using Math_Implementation;

namespace CollisionDetectionSelector.Primitives
{
    class OBJ
    {
        public OBJ Parent = null;
        public List<OBJ> Children = new List<OBJ>();
        OBJLoader model = null;

        // VERY IMPORTANT THAT THESE HAVE DEFAULT VALUES
        protected Vector3 position = new Vector3(0f, 0f, 0f);
        protected Vector3 rotation = new Vector3(0f, 0f, 0f);
        protected Vector3 scale = new Vector3(1f, 1f, 1f);

        public OBJ(OBJLoader loader)
        {
            model = loader;
        }

        protected Matrix4 worldMatrix;
        protected bool dirtySelf = true;
        protected bool dirty
        {
            get
            {
                return dirtySelf;
            }
            set
            {
                dirtySelf = value;
                if (value)
                {
                    foreach (OBJ child in Children)
                    {
                        child.dirty = true;
                    }
                }
            }
        }

        public Matrix4 WorldMatrix
        {
            get
            {
                if (dirty)
                {
                    Matrix4 translation = Matrix4.Translate(position);

                    Matrix4 pitch = Matrix4.XRotation(rotation.X);
                    Matrix4 yaw = Matrix4.YRotation(rotation.Y);
                    Matrix4 roll = Matrix4.ZRotation(rotation.Z);
                    Matrix4 orientation = roll * pitch * yaw;

                    Matrix4 scaling = Matrix4.Scale(scale);

                    worldMatrix = translation * orientation * scaling;

                    if (Parent != null)
                    {
                        worldMatrix = Parent.WorldMatrix * worldMatrix;
                    }

                    // DO NOT FORGET THIS! The first version
                    // I wrote, i forgot to clear the dirty flag
                    // and my game stopped running!
                    dirty = false;
                }
                return worldMatrix;
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                dirty = true;
            }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                dirty = true;
            }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                dirty = true;
            }
        }

        public AABB BoundingBox
        {
            get
            {
                return model.BoundingBox;
            }
        }

        public Sphere BoundingSphere
        {
            get
            {
                return model.BoundingSphere;
            }
        }

        public Triangle[] Mesh
        {
            get
            {
                return model.CollisionMesh;
            }
        }


        private int ChildrenRender(bool normal, bool bvh, bool debug)
        {
            int result = 0;
            if (Children != null)
            {
                foreach (OBJ child in Children)
                {
                    if (normal)
                    {
                        result += child.Render();
                    }
                    else if (bvh)
                    {
                        child.RenderBVH();
                    }
                    else if (debug)
                    {
                        child.DebugRender();
                    }
                    if (child.Children != null)
                    {
                        result += child.ChildrenRender(normal, bvh, debug);
                    }
                }
            }
            return result;
        }


        public int Render()
        {

            int result = 0;

            GL.PushMatrix();
            //always getter
            GL.MultMatrix(WorldMatrix.OpenGL);
            if (model != null)
            {
                model.Render();
                result++;
            }
            GL.PopMatrix();

            result += ChildrenRender(true, false, false);
            return result;
        }

        public void DebugRender()
        {
            GL.PushMatrix();
            GL.MultMatrix(WorldMatrix.OpenGL);
            if (model != null)
            {
                model.DebugRender();
            }
            GL.PopMatrix();
            ChildrenRender(false, false, true);
        }

        public override string ToString()
        {
            return "Triangle count: " + model.NumCollisionTriangles;
        }

        public void RenderBVH()
        {
            GL.PushMatrix();
            GL.MultMatrix(WorldMatrix.OpenGL);
            if (model != null)
            {
                model.RenderBVH();
            }
            GL.PopMatrix();
            ChildrenRender(false, true, false);
        }

        public BVHNode BVHRoot
        {
            get
            {
                return model.BvhRoot;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return model == null;
            }
        }
    }
}