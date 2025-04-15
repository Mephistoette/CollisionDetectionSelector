using OpenTK.Graphics.OpenGL;
using Math_Implementation;

namespace CollisionDetectionSelector.Primitives
{
    class OBJ
    {
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
        protected bool dirty = true; // MUST BE TRUE BY DEFAULT

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

        public void Render()
        {
            GL.PushMatrix();
            // IMPORTANT: Calling the getter, not raw accessing the array!
            GL.MultMatrix(WorldMatrix.OpenGL);
            model.Render();
            GL.PopMatrix();
        }

        public void DebugRender()
        {
            GL.PushMatrix();
            // IMPORTANT: Calling the getter, not raw accessing the array!
            GL.MultMatrix(WorldMatrix.OpenGL);
            model.DebugRender();
            GL.PopMatrix();
        }

        public override string ToString()
        {
            return "Triangle count: " + model.NumCollisionTriangles;
        }

        public void RenderBVH()
        {
            GL.PushMatrix();
            GL.MultMatrix(WorldMatrix.OpenGL);
            model.RenderBVH();
            GL.PopMatrix();
        }

        public BVHNode BVHRoot
        {
            get
            {
                return model.BvhRoot;
            }
        }
    }
}