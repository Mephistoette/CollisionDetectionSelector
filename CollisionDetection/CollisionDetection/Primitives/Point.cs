using System;
using Math_Implementation;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionSelector.Primitives
{
    internal class Point
    {
        protected Vector3 position = new Vector3();
        public Vector3 Position
        {
            get
            {
                return position;
            } // TODO (Make new vector)
            set
            {
                position.X = value.X;
                position.Y = value.Y;
                position.Z = value.Z;
            } // TODO (Make new vector)
        }
        public float X
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }// TODO: get, set
        public float Y
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        } // TODO: get, set
        public float Z
        {
            get
            {
                return position.Z;
            }
            set
            {
                position.Z = value;
            }
        } // TODO: get, set

        public Point() // TODO
        {
            Position =  new Vector3();
        }
        public Point(float x, float y, float z) // TODO
        {
            Position = new Vector3(x,y, z);
        }
        public Point(Vector3 v) // TODO (Make new)
        {
            Position = new Vector3(v.X,v.Y,v.Z);
        }

        public Vector3 ToVector() // TODO (Return new)
        {
            return new Vector3(Position.X, Position.Y, Position.Z);   
        }
        public void FromVector(Vector3 v) // TODO (Make new)
        {
            Position = new Vector3(v.X, v.Y, v.Z);
        }

        #region Rendering  
        public void Render()
        {
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(Position.X, Position.Y, Position.Z);
            GL.End();
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
        #endregion
    }
}
