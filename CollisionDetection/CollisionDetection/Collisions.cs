using System;
using Math_Implementation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollisionDetectionSelector.Primitives;

namespace CollisionDetectionSelector
{
    internal class Collisions
    {
        public static bool PointInSphere(Sphere sphere, Point point)
        {
            // TODO
            float x = sphere.Position.X - point.Position.X;
            float y = sphere.Position.Y - point.Position.Y; 
            float z = sphere.Position.Z - point.Position.Z;
            Vector3 differece = new Vector3(x, y, z);
            return differece.LengthSquared() < sphere.Radius * sphere.Radius;
        }

        public static Point ClosestPoint(Sphere sphere, Point point)
        {
            float x = sphere.Position.X - point.Position.X;
            float y = sphere.Position.Y - point.Position.Y;
            float z = sphere.Position.Z - point.Position.Z;
            Vector3 direction = new Vector3(-x, -y, -z);
            direction.Normalize();
            Vector3 sphPos =  new Vector3(sphere.Position.X, sphere.Position.Y, sphere.Position.Z);
            return new Point(direction.X * sphere.Radius+ sphPos.X, direction.Y * sphere.Radius+ sphPos.Y, direction.Z * sphere.Radius + sphPos.Z);
        }

        public static bool PointInAABB(AABB aabb, Point point)
        {
            return point.X>aabb.Min.X&&point.Y>aabb.Min.Y&&point.Z>aabb.Min.Z&&
                point.X<aabb.Max.X&&point.Y<aabb.Max.Y&&point.Z<aabb.Max.Z;
        }

        public static Point ClosestPoint(AABB aabb, Point point)
        {
            float x = Math.Clamp(point.X,aabb.Min.X,aabb.Max.X);
            float y = Math.Clamp(point.Y,aabb.Min.Y,aabb.Max.Y);
            float z = Math.Clamp(point.Z,aabb.Min.Z,aabb.Max.Z);
            return new Point(x,y,z);
        }

        public static bool PointOnPlane(Point point, Plane plane)   // Should return a positive number, a negative number or 0
        {
            return Math.Abs(Vector3.Dot(point.Position, plane.Normal) - plane.Distance) < 0.00001f;
        }
        public static float DistanceFromPlane(Point point, Plane plane)
        {
            return Vector3.Dot(point.Position, plane.Normal) - plane.Distance;
        }

        public static Point ClosestPoint(Plane plane, Point point)
        {
            Vector3 closestVector =  Vector3.Projection(point.Position, plane.Normal) - plane.Normal * plane.Distance;
            return new Point(closestVector.X,closestVector.Y,closestVector.Z);
        }

        public static Point Intersection(Plane p1, Plane p2, Plane p3) //Cramer's Rule
        {
            float[] array = { p1.Normal.X,p1.Normal.Y,p1.Normal.Z,
            p2.Normal.X,p2.Normal.Y,p2.Normal.Z,
            p3.Normal.X,p3.Normal.Y,p3.Normal.Z};
            Matrix3 coA = new Matrix3(array);
            float detA = Matrix3.Determinant(coA);

            float[] arrayX = { p1.Distance,p1.Normal.Y,p1.Normal.Z,
            p2.Distance,p2.Normal.Y,p2.Normal.Z,
            p3.Distance,p3.Normal.Y,p3.Normal.Z};
            Matrix3 coAX = new Matrix3(arrayX);
            float detAX = Matrix3.Determinant(coAX);

            float[] arrayY = { p1.Normal.X,p1.Distance,p1.Normal.Z,
            p2.Normal.X,p2.Distance,p2.Normal.Z,
            p3.Normal.X,p3.Distance,p3.Normal.Z};
            Matrix3 coAY = new Matrix3(arrayY);
            float detAY = Matrix3.Determinant(coAY);

            float[] arrayZ = { p1.Normal.X,p1.Normal.Y,p1.Distance,
            p2.Normal.X,p2.Normal.Y,p2.Distance,
            p3.Normal.X,p3.Normal.Y,p3.Distance};
            Matrix3 coAZ = new Matrix3(arrayZ);
            float detAZ = Matrix3.Determinant(coAZ);

            return new Point(detAX/detA,detAY/detA,detAZ/detA);

        }
    }
}
