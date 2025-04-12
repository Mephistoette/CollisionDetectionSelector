using System;
using OpenTK.Graphics.OpenGL;
using Math_Implementation;

namespace CollisionDetectionSelector.Primitives
{
    class Ray
    {
        public Point Position;
        private Vector3 _normal;

        public Vector3 Normal
        {
            get
            {
                // TODO: Returns _normal
                return _normal;
            }
            set
            {
                // TODO: Sets (and normalizes) _normal
                _normal = value;
                _normal.Normalize();
            }
        }

        public Ray()
        {
            // Make a ray that is at origin, 
            // pointing down the Z axis
            Position = new Point(0, 0, 0);
            _normal = new Vector3(0, 0, 1);
        }

        public Ray(Point p, Vector3 d)
        {
            // Don't forget to normalize _normal
            Position = new Point(p.X, p.Y, p.Z);
            _normal = new Vector3(d.X, d.Y, d.Z);
            _normal.Normalize();
        }

        public Ray(Vector3 p, Vector3 d)
        {
            // Don't forget to normalize _normal
            Position = new Point(p.X, p.Y, p.Z);
            _normal = new Vector3(d.X, d.Y, d.Z);
            _normal.Normalize();
        }

        //Render and toString must be there!
        public void Render()
        {
            GL.Begin(PrimitiveType.Lines);
            Vector3 start = Position.ToVector();
            Vector3 end = start + _normal * 5000f;
            GL.Vertex3(start.X, start.Y, start.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.End();
        }

        public override string ToString()
        {
            string result = "P: (" + Position.X + ", " + Position.Y + ", " + Position.Z + "), ";
            result += "D: ( " + _normal.X + ", " + _normal.Y + ", " + _normal.Z + ")";
            return result;
        }
    }
}