using OpenTK.Graphics.OpenGL;
using Math_Implementation;
using CollisionDetectionSelector.Primitives;

namespace CollisionDetectionSelector.Samples
{
    class ThreePlaneIntersectionSample : Application
    {
        protected Vector3 cameraAngle = new Vector3(120.0f, -10f, 20.0f);
        protected float rads = (float)(System.Math.PI / 180.0f);

        Plane plane1 = new Plane(new Vector3(1f, 0f, 0f), 0f);
        Plane plane2 = new Plane(new Vector3(0f, 1f, 0f), 0f);
        Plane plane3 = new Plane(new Vector3(0f, 0f, 1f), 0f);

        public override void Initialize(int width, int height)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.PointSize(5f);
        }

        public override void Render()
        {
            Vector3 eyePos = new Vector3();
            eyePos.X = cameraAngle.Z * -(float)System.Math.Sin(cameraAngle.X * rads * (float)System.Math.Cos(cameraAngle.Y * rads));
            eyePos.Y = cameraAngle.Z * -(float)System.Math.Sin(cameraAngle.Y * rads);
            eyePos.Z = -cameraAngle.Z * (float)System.Math.Cos(cameraAngle.X * rads * (float)System.Math.Cos(cameraAngle.Y * rads));

            Matrix4 lookAt = Matrix4.LookAt(eyePos, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            GL.LoadMatrix(Matrix4.Transpose(lookAt).Matrix);

            DrawOrigin();

            GL.Color3(1f, 0f, 0f);
            plane1.Render();
            GL.Color3(0f, 1f, 0f);
            plane2.Render();
            GL.Color3(0f, 0f, 1f);
            plane3.Render();

            GL.Disable(EnableCap.DepthTest);
            GL.Color3(0f, 1f, 1f);
            Point midPoint = Collisions.Intersection(plane1, plane2, plane3);
            midPoint.Render();
            GL.Enable(EnableCap.DepthTest);
        }

        public override void Update(float deltaTime)
        {
            cameraAngle.X += 45.0f * deltaTime;
        }

        protected void DrawOrigin()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 1f, 0f);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 1f);
            GL.End();
        }

        public override void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.MatrixMode(MatrixMode.Projection);
            float aspect = (float)width / (float)height;
            Matrix4 perspective = Matrix4.Perspective(60, aspect, 0.01f, 1000.0f);
            GL.LoadMatrix(Matrix4.Transpose(perspective).Matrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}