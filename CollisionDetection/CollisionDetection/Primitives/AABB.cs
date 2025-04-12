using System;
using OpenTK.Graphics.OpenGL;
using Math_Implementation;
using OpenTK.Platform.Windows;

namespace CollisionDetectionSelector.Primitives
{
    class AABB
    {
        public Point Min = new Point();
        public Point Max = new Point();

        public Point Center
        {
            get
            {
                float x = Min.X + Max.X;
                float y = Min.Y + Max.Y;
                float z = Min.Z + Max.Z;
                return new Point(x/2, y/2, z/2);
                // FIGURE OUT CENTER POINT
            }
            // NO SET!
        }

        public Vector3 Extents
        {
            get
            {
                float x = Max.X - Center.X;
                float y = Max.Y - Center.Y;
                float z = Max.Z - Center.Z;
                return new Vector3(x,y,z);
                // FIGURE OUT EXTENTS
            }
            // NO SET!
        }

        public bool IsValid
        {
            get
            {
                return Min.X < Max.X && Min.Y < Max.Y && Min.Z < Max.Z;
            }
            // TODO: Implement, getter only, no setter!
        }

        public void Fix()
        {
            if(Min.X>Max.X)
                (Min.X,Max.X) = (Max.X,Min.X);
            if(Min.Y>Max.Y)
                (Min.Y,Max.Y) = (Max.Y,Min.Y);
            if(Min.Z>Max.Z)
                (Min.Z,Max.Z) = (Max.Z,Min.Z);
            // TODO: Implement
        }

        public AABB()
        {
            Min = new Point(-1, -1, -1);
            Max = new Point(1, 1, 1);
            // TODO: Make a unit AABB
            // Min = -1, max = +1 on all axis
        }

        public AABB(Point min, Point max)
        {
            Min.X = Math.Min(min.X, max.X);
            Max.X = Math.Max(min.X, max.X);
            Min.Y = Math.Min(min.Y, max.Y);
            Max.Y = Math.Max(min.Y, max.Y);
            Min.Z = Math.Min(min.Z, max.Z);
            Max.Z = Math.Max(min.Z, max.Z);
            // TODO: Set Min and Max
            // Don't forget, could be invalid
        }

        public AABB(Point center, Vector3 extents)
        {
            Min.X = center.X - extents.X;
            Max.X = center.X + extents.X;
            Min.Y = center.Y - extents.Y;
            Max.Y = center.Y + extents.Y;
            Min.Z = center.Z - extents.Z;
            Max.Z = center.Z + extents.Z;
            // TODO: Set Min and Max
        }

        // No need to change below this, render and to-string
        // functions are provided
        public void Render()
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(Min.X, Min.Y, Max.Z);
            GL.Vertex3(Max.X, Min.Y, Max.Z);
            GL.Vertex3(Max.X, Max.Y, Max.Z);
            GL.Vertex3(Min.X, Max.Y, Max.Z);

            GL.Vertex3(Max.X, Min.Y, Max.Z);
            GL.Vertex3(Max.X, Min.Y, Min.Z);
            GL.Vertex3(Max.X, Max.Y, Min.Z);
            GL.Vertex3(Max.X, Max.Y, Max.Z);

            GL.Vertex3(Min.X, Max.Y, Max.Z);
            GL.Vertex3(Max.X, Max.Y, Max.Z);
            GL.Vertex3(Max.X, Max.Y, Min.Z);
            GL.Vertex3(Min.X, Max.Y, Min.Z);

            GL.Vertex3(Min.X, Min.Y, Min.Z);
            GL.Vertex3(Min.X, Max.Y, Min.Z);
            GL.Vertex3(Max.X, Max.Y, Min.Z);
            GL.Vertex3(Max.X, Min.Y, Min.Z);

            GL.Vertex3(Min.X, Min.Y, Min.Z);
            GL.Vertex3(Max.X, Min.Y, Min.Z);
            GL.Vertex3(Max.X, Min.Y, Max.Z);
            GL.Vertex3(Min.X, Min.Y, Max.Z);

            GL.Vertex3(Min.X, Min.Y, Min.Z);
            GL.Vertex3(Min.X, Min.Y, Max.Z);
            GL.Vertex3(Min.X, Max.Y, Max.Z);
            GL.Vertex3(Min.X, Max.Y, Min.Z);

            GL.End();
        }

        public override string ToString()
        {
            string result = "Min: (" + Min.X + ", " + Min.Y + ", " + Min.Z + "), ";
            result += "Max: ( " + Max.X + ", " + Max.Y + ", " + Max.Z + ")";
            return result;
        }
    }
}
