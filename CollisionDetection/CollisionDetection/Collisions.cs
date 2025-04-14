﻿using System;
using Math_Implementation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollisionDetectionSelector.Primitives;
using System.ComponentModel.Design;
using OpenTK.Graphics.OpenGL;

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

        // TODO: Implement this function
        public static bool Raycast(Ray ray, Sphere sphere, out float t)
        {
            Vector3 p0 = ray.Position.Position;
            Vector3 d = ray.Normal;
            Vector3 c = sphere.Position.Position;
            float r = sphere.Radius;

            Vector3 e = c - p0;
            // Using Length here would cause floating point error to creep in
            float Esq = Vector3.LengthSquared(e);
            float a = Vector3.Dot(e, d);
            float b = (float)Math.Sqrt(Esq - (a * a));
            float f = (float)Math.Sqrt((r * r) - (b * b));

            // No collision
            if (r * r - Esq + a * a < 0f)
            {
                t = -1;
                return false;// -1 is invalid.
            }
            // Ray is inside
            else if (Esq < r * r)
            {
                t = a + f;
                return true;// Just reverse direction
            }
            // else Normal intersection
            t = a - f;
            return true;
        }

        // I've implemented these for you!

        // Conveniance method, returns t without an out param
        // If no collision happened, will return -1
        public static float Raycast(Ray ray, Sphere sphere)
        {
            float t = -1;
            if (!Raycast(ray, sphere, out t))
            {
                return -1;
            }
            return t;
        }

        // Conveniance method, returns the point of intersection
        // instead of p
        public static bool Raycast(Ray ray, Sphere sphere, out Point p)
        {
            float t = -1;
            bool result = Raycast(ray, sphere, out t);
            p = new Point(ray.Position.ToVector() + ray.Normal * t);
            return result;
        }

        // TODO: Implement ONLY THIS ONE method:
        public static bool Raycast(Ray ray, AABB aabb, out float t)
        {
            float tNear = float.MinValue;
            float tFar = float.MaxValue;
            for(int i = 0; i<3;++i)
            {
                if(i==0)
                {
                    if(Math.Abs(ray.Normal.X)<0.00001f)
                    {
                        if(ray.Position.X<aabb.Min.X||ray.Position.X>aabb.Max.X)
                        {
                            t = -1;
                            return false;
                        }
                        continue;
                    }
                    else
                    {
                        tNear = Math.Max(tNear, Math.Min((aabb.Min.X - ray.Position.X) / ray.Normal.X, (aabb.Max.X - ray.Position.X) / ray.Normal.X));
                        tFar = Math.Min(tFar, Math.Max((aabb.Min.X - ray.Position.X) / ray.Normal.X, (aabb.Max.X - ray.Position.X) / ray.Normal.X));
                    }
                }

                else if(i==1)
                {
                     if(Math.Abs(ray.Normal.Y)<0.00001f)
                     {
                        if(ray.Position.Y<aabb.Min.Y||ray.Position.Y>aabb.Max.Y)
                        {
                            t = -1;
                            return false;
                        }
                        continue;
                     }
                     else
                    {
                        tNear = Math.Max(tNear, Math.Min((aabb.Min.Y - ray.Position.Y) / ray.Normal.Y, (aabb.Max.Y - ray.Position.Y) / ray.Normal.Y));
                        tFar = Math.Min(tFar, Math.Max((aabb.Min.Y - ray.Position.Y) / ray.Normal.Y, (aabb.Max.Y - ray.Position.Y) / ray.Normal.Y));
                    }
                }

                else if(i==2)
                {
                    if(Math.Abs(ray.Normal.Z)<0.00001f)
                    {
                        if(ray.Position.Z<aabb.Min.Z||ray.Position.Z>aabb.Max.Z)
                        {
                            t = -1;
                            return false;
                        }
                        continue;
                    }
                    else
                    {
                        tNear = Math.Max(tNear, Math.Min((aabb.Min.Z - ray.Position.Z) / ray.Normal.Z, (aabb.Max.Z - ray.Position.Z) / ray.Normal.Z));
                        tFar = Math.Min(tFar, Math.Max((aabb.Min.Z - ray.Position.Z) / ray.Normal.Z, (aabb.Max.Z - ray.Position.Z) / ray.Normal.Z));
                    }
                }
            }
            if (tFar > tNear && tFar > 0.00001f)
            {
                if (tNear > 0.00001f) t = tNear;
                else t = tFar;
                return true;
            }
           
            t = -1;
            return false;
        }

        // I've implemented the blow methods for you.
        // Nothing to do past this point

        public static float Raycast(Ray ray, AABB aabb)
        {
            float t = -1;
            if (!Raycast(ray, aabb, out t))
            {
                return -1;
            }
            return t;
        }

        public static bool Raycast(Ray ray, AABB aabb, out Point p)
        {
            float t = -1;
            bool result = Raycast(ray, aabb, out t);
            p = new Point(ray.Position.ToVector() + ray.Normal * t);
            return result;
        }

        // TODO: Implement ONLY THIS ONE method:
        public static bool Raycast(Ray ray, Plane plane, out float t)
        {
            float nd = Vector3.Dot(ray.Normal,plane.Normal);
            float pn = Vector3.Dot(ray.Position.Position, plane.Normal);

            if(Math.Abs(nd)<0.00001f)
            {
                t = -1;
                return false;
            }

            t = (plane.Distance - pn) / nd;
            if(t<0.00001f)
            {
                t = -1; 
                return false;
            }

            return true;
        }


        // I've implemented the blow methods for you.
        // Nothing to do past this point

        // Conveniance method, returns t without an out param
        // If no collision happened, will return -1
        public static float Raycast(Ray ray, Plane plane)
        {
            float t = -1;
            if (!Raycast(ray, plane, out t))
            {
                return -1;
            }
            return t;
        }

        // Conveniance method, returns the point of intersection
        public static bool Raycast(Ray ray, Plane plane, out Point p)
        {
            float t = -1;
            bool result = Raycast(ray, plane, out t);
            p = new Point(ray.Position.ToVector() + ray.Normal * t);
            return result;
        }

        public static bool LineTest(Line line, Sphere sphere, out Point result)
        {
            Ray r = new Ray();
            r.Position = new Point(line.start.X, line.start.Y, line.start.Z);
            //normal points from start to end, by value
            //the normal setter will automatically normalize this
            r.Normal = (line.end.ToVector() - line.start.ToVector());

            //line case logic
            float t = -1;
            if (!Raycast(r, sphere, out t))
            { //this changes t
              //if raycast returns false the point was never on the line
                result = new Point(0f, 0f, 0f);
                return false;
            }
            //if t is < 0 , point is behind the start point
            if (t < 0)
            {
                //by value, call new point don't use refernece
                result = new Point(line.start.ToVector());
                return false;
            }
            //if t is longer than length of line, intersection is after start point
            else if (t * t > line.LengthSquared)
            {
                result = new Point(line.end.ToVector());
                return false;
            }
            // If we made it here, the line intersected the sphere
            result = new Point(r.Position.ToVector() + r.Normal * t);
            return true;
        }

        public static bool LineTest(Line line, AABB aabb, out Point result)
        {
            //create ray out of line
            Ray r = new Ray();
            r.Position = new Point(line.start.X, line.start.Y, line.start.Z);
            r.Normal = (line.end.ToVector() - line.start.ToVector());

            //begin linecast logic
            float t = -1;
            if (!Raycast(r, aabb, out t))
            {
                //false = point was never in aabb
                result = new Point(0f, 0f, 0f);
                return false;
            }
            if (t < 0)
            { //behind start point
                result = new Point(line.start.ToVector());
                return false;
            }
            else if (t * t > line.LengthSquared)
            {//intersection after start point
                result = new Point(line.end.ToVector());
                return false;
            }
            //passed all tests
            result = new Point(r.Position.ToVector() + r.Normal * t);
            return true;
        }

        public static bool LineTest(Line line, Plane plane, out Point result)
        {
            Ray r = new Ray();
            r.Position = new Point(line.start.X, line.start.Y, line.start.Z);
            r.Normal = line.end.ToVector() - line.start.ToVector();

            float t = -1;
            if (!Raycast(r, plane, out t))
            {
                result = new Point(0f, 0f, 0f);
                return false;
            }
            if (t < 0)
            {
                result = new Point(line.start.ToVector());
                return false;
            }
            if (t * t > line.LengthSquared)
            {
                result = new Point(line.end.ToVector());
                return false;
            }
            result = new Point(r.Position.ToVector() + r.Normal * t);
            return true;
        }

        public static bool PointInTriangle(Triangle triangle, Point point)
        {
            //First find the point on the plane containing the triangle
            Plane plane = new Plane(triangle.p0,triangle.p1,triangle.p2);
            if (!PointOnPlane(point, plane)) return false;
            //Point closestPointOnPlane = ClosestPoint(plane, point);
            //if(closestPointOnPlane!=point) return false;

            /*
             * // Compute vectors        
            v0 = C - A
            v1 = B - A
            v2 = P - A

            // Compute dot products
            dot00 = dot(v0, v0)
            dot01 = dot(v0, v1)
            dot02 = dot(v0, v2)
            dot11 = dot(v1, v1)
            dot12 = dot(v1, v2)

            // Compute barycentric coordinates
            invDenom = 1 / (dot00 * dot11 - dot01 * dot01)
            u = (dot11 * dot02 - dot01 * dot12) * invDenom
            v = (dot00 * dot12 - dot01 * dot02) * invDenom

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1)

             */
            //Barycentric 
            Vector3 ab = triangle.p1.ToVector() - triangle.p0.ToVector();
            Vector3 ac = triangle.p2.ToVector() - triangle.p0.ToVector();
            Vector3 ap = point.ToVector() - triangle.p0.ToVector();

            float dot00 = Vector3.Dot(ab, ab);
            float dot01 = Vector3.Dot(ab, ac);
            float dot02 = Vector3.Dot(ab, ap);
            float dot11 = Vector3.Dot(ac, ac);
            float dot12 = Vector3.Dot(ac, ap);

            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);

            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            if (0<=u&&0<=v&&u+v<=1) return true;
            return false;
        }
    }
}
