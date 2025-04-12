using System;
using OpenTK.Graphics.OpenGL;
using Math_Implementation;

namespace CollisionDetectionSelector.Primitives
{
    class Line
    {
        public Point start = new Point();
        public Point end = new Point();

        public float Length { 
            get 
            {
               return  Vector3.Length(end.Position - start.Position);
            } 
        }
        public float LengthSquared { 
            get 
            { 
                return Vector3.LengthSquared(end.Position - start.Position);
            } 
        }

        public Line()
        {
            start = new Point(0, 0, 0);
            end = new Point(0, 0, 0);  
            // Make a line of 0 length,
            // both points should default to (0, 0, 0)
        }

        public Line(Line other)
        {
            start = other.start;
            end = other.end;
            // TODO: Copy the other line
            // Pay attention, are you assigning by reference or value
        }

        public Line(Point p1, Point p2)
        {
            start = p1;
            end = p2;
            // TODO
        }

        public Line(Vector3 p1, Vector3 p2)
        {
            start = new Point(p1.X,p1.Y, p1.Z);
            end = new Point(p2.X, p2.Y, p2.Z);
            // TODO
        }

        public Vector3 ToVector()
        {
            return end.Position - start.Position;
            // TODO: return end - start
        }

        //Render and toString must be there!
        public void Render()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(start.X, start.Y, start.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.End();
        }

        public override string ToString()
        {
            string result = "Start: (" + start.X + ", " + start.Y + ", " + start.Z + "), ";
            result += "End: ( " + end.X + ", " + end.Y + ", " + end.Z + ")";
            return result;
        }
    }
}