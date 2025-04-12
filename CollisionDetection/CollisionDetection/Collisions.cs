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
            Vector3 sphPos = new Vector3(sphere.Position.X, sphere.Position.Y, sphere.Position.Z);
            return new Point(direction.X * sphere.Radius + sphPos.X, direction.Y * sphere.Radius + sphPos.Y, direction.Z * sphere.Radius + sphPos.Z);
        }

        public static bool PointInAABB(AABB aabb, Point point)
        {
            return point.X > aabb.Min.X && point.Y > aabb.Min.Y && point.Z > aabb.Min.Z &&
                point.X < aabb.Max.X && point.Y < aabb.Max.Y && point.Z < aabb.Max.Z;
        }

        public static Point ClosestPoint(AABB aabb, Point point)
        {
            float x = Math.Clamp(point.X, aabb.Min.X, aabb.Max.X);
            float y = Math.Clamp(point.Y, aabb.Min.Y, aabb.Max.Y);
            float z = Math.Clamp(point.Z, aabb.Min.Z, aabb.Max.Z);
            return new Point(x, y, z);
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
            float distance = Vector3.Dot(plane.Normal, point.Position) - plane.Distance;
            // If the plane normal wasn't normalized, we'd need this:
            // distance = distance / DOT(plane.Normal, plane.Normal);
            Vector3 res = point.Position - plane.Normal * distance;
            return new Point(res);
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

            return new Point(detAX / detA, detAY / detA, detAZ / detA);

        }

        public static bool PointOnLine(Point point, Line line)
        {
            float dx = line.end.X - line.start.X;
            if (Math.Abs(dx) < 0.00001f)
            {
                return Math.Abs(point.X - line.start.X) < 0.00001f &&
                       Math.Abs(point.Y - line.start.Y) < 0.00001f &&
                       Math.Abs(point.Z - line.start.Z) < 0.00001f;
            }

            float mY = (line.end.Y - line.start.Y) / dx;
            float mZ = (line.end.Z - line.start.Z) / dx;

            return Math.Abs(point.Y - line.start.Y - mY * (point.X - line.start.X)) < 0.00001f &&
                   Math.Abs(point.Z - line.start.Z - mZ * (point.X - line.start.X)) < 0.00001f;
        }

        public static Point ClosestPoint(Line ab, Point c, out float t)
        {
            t = Vector3.Dot(c.Position - ab.start.Position, ab.end.Position - ab.start.Position);
            t /= Vector3.Dot(ab.end.Position - ab.start.Position, ab.end.Position - ab.start.Position);

            t = Math.Clamp(t, 0.0f, 1.0f);
            Vector3 dir = (ab.end.Position - ab.start.Position) * t;
            Vector3 res = dir + ab.start.Position;
            return new Point(res);
        }
        public static Point ClosestPoint(Line ab, Point c)
        {
            float t = 0;
            return ClosestPoint(ab, c, out t);
        }

        public static bool PointOnRay(Point point, Ray ray)
        {
            Vector3 newNorm = point.Position - ray.Position.Position;
            newNorm.Normalize();
            if (point.X == ray.Position.X && point.Y == ray.Position.Y && point.Z == ray.Position.Z)
            {
                return true;
            }
            float t = Vector3.Dot(newNorm, ray.Normal);
            return Math.Abs(1f - t) < 0.00001f;
        }

        public static Point ClosestPoint(Ray r, Point c)
        {
            float t = Vector3.Dot(r.Normal, c.Position - r.Position.Position) / Vector3.Dot(r.Normal, r.Normal);
            t = Math.Max(t, 0.0f);
            Vector3 res = r.Normal * t + r.Position.Position;
            return new Point(res);
        }


        public static bool Intersects(Sphere s1, Sphere s2)
        {
            Vector3 difference = s1.Position.Position - s2.Position.Position;
            return difference.LengthSquared() <= (s1.Radius + s2.Radius) * (s1.Radius + s2.Radius);
        }

        // TODO: Implement this
        public static bool Intersects(Sphere sphere, AABB aabb)
        {
            Point closestPoint = ClosestPoint(aabb, sphere.Position);
            Vector3 difference = closestPoint.Position - sphere.Position.Position;
            return difference.LengthSquared() <= sphere.Radius * sphere.Radius;
        }

        // Just a conveniance function, so argument order wont matter!
        public static bool Intersects(AABB aabb, Sphere sphere)
        {
            return Intersects(sphere, aabb);
        }

        // TODO: Provide implementation
        public static bool Intersects(Sphere sphere, Plane plane)
        {
            Point closestPoint = ClosestPoint(plane, sphere.Position);
            Vector3 difference = closestPoint.Position - sphere.Position.Position;
            return difference.LengthSquared() <= sphere.Radius * sphere.Radius;
        }

        // Conveniance function
        public static bool Intersects(Plane plane, Sphere sphere)
        {
            return Intersects(sphere, plane);
        }

        public static bool Intersects(AABB a, AABB b)
        {
            return a.Min.X <= b.Max.X && a.Max.X >= b.Min.X &&
                a.Min.Y <= b.Max.Y && a.Max.X >= b.Min.Y &&
                a.Min.Z <= b.Max.Z && a.Max.Z >= b.Min.Z;
        }


        // TODO: Provide implementation for this
        public static bool Intersects(AABB aabb, Plane plane)
        {

            // Convert AABB to center-extents representation
            Point c = aabb.Center; // Compute AABB center
            Vector3 e = aabb.Extents; // Compute positive extents

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            float r = e[0] * Math.Abs(plane.Normal.X) + e[1] * Math.Abs(plane.Normal.Y) + e[2] * Math.Abs(plane.Normal.Z);

            // Compute distance of box center from plane
            float s = Vector3.Dot(plane.Normal,c.Position) - plane.Distance;

            // Intersection occurs when distance s falls within [-r,+r] interval
            return Math.Abs(s) <= r;
        }

        // Conveniance function
        public static bool Intersects(Plane plane, AABB aabb)
        {
            return Intersects(aabb, plane);
        }

        public static bool Intersects(Plane p1, Plane p2)
        {
            Vector3 d = Vector3.Cross(p1.Normal, p2.Normal);

            return Vector3.Dot(d, d) > 0.00001f;
        }
    }
}
