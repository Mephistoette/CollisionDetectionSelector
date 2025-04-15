using System;
using Math_Implementation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollisionDetectionSelector.Primitives;
using System.ComponentModel.Design;
using OpenTK.Graphics.OpenGL;
using static System.Net.Mime.MediaTypeNames;

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

        public static Point ClosestPoint(Triangle triangle, Point point)
        {
            //Firstly find the plane containing the triangle
            Plane plane = new Plane(triangle.p0,triangle.p1,triangle.p2);
            Point closestPointOnPlane = ClosestPoint(plane, point);
            if(PointInTriangle(triangle,closestPointOnPlane)) return closestPointOnPlane;

            Line ab = new Line(triangle.p0, triangle.p1);
            Line ac = new Line(triangle.p0, triangle.p2);
            Line bc = new Line(triangle.p1, triangle.p2);

            Point closestPoint1 = ClosestPoint(ab, closestPointOnPlane);
            Point closestPoint2 = ClosestPoint(ac, closestPointOnPlane);
            Point closestPoint3 = ClosestPoint(bc, closestPointOnPlane);

            Vector3 distance1 = closestPoint1.ToVector() - point.ToVector();
            Vector3 distance2 = closestPoint2.ToVector() - point.ToVector();   
            Vector3 distance3 = closestPoint3.ToVector() - point.ToVector();

            float d1 = distance1.LengthSquared();
            float d2 = distance2.LengthSquared();
            float d3 = distance3.LengthSquared();

            if (d1 <= d2 && d1 <= d3) return closestPoint1;
            else if(d2 <= d3 && d2 <= d1) return closestPoint2;
            else return closestPoint3;
        }

        // TODO: Implement this function
        public static bool Intersects(Triangle triangle, Sphere sphere)
        {
            Point point = ClosestPoint(triangle, sphere.Position);
            Vector3 distance = sphere.Position.ToVector() - point.ToVector();
            return  distance.LengthSquared()<sphere.Radius*sphere.Radius;
        }

        // This is just a conveniance function
        public static bool Intersects(Sphere sphere, Triangle triangle)
        {
            return Intersects(triangle, sphere);
        }

        public static bool Intersects(Triangle triangle, AABB aabb)
        {
            // Get the triangle points as vectors
            Vector3 v0 = triangle.p0.ToVector();
            Vector3 v1 = triangle.p1.ToVector();
            Vector3 v2 = triangle.p2.ToVector();

            // Convert AABB to center-extents form
            Vector3 c = aabb.Center.ToVector();
            Vector3 e = aabb.Extents;

            // Translate the triangle as conceptually moving the AABB to origin
            // This is the same as we did with the point in triangle test
            v0 -= c;
            v1 -= c;
            v2 -= c;

            // Compute the edge vectors of the triangle  (ABC)
            // That is, get the lines between the points as vectors
            Vector3 f0 = v1 - v0; // B - A
            Vector3 f1 = v2 - v1; // C - B
            Vector3 f2 = v0 - v2; // A - C

            // Compute the face normals of the AABB, because the AABB
            // is at center, and of course axis aligned, we know that 
            // it's normals are the X, Y and Z axis.
            Vector3 u0 = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 u1 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 u2 = new Vector3(0.0f, 0.0f, 1.0f);

            // Compute the 9 axis
            Vector3 axis_u0_f0 = Vector3.Cross(u0, f0);
            Vector3 axis_u0_f1 = Vector3.Cross(u0, f1);
            Vector3 axis_u0_f2 = Vector3.Cross(u0, f2);

            Vector3 axis_u1_f0 = Vector3.Cross(u1, f0);
            Vector3 axis_u1_f1 = Vector3.Cross(u1, f1);
            Vector3 axis_u1_f2 = Vector3.Cross(u2, f2);

            Vector3 axis_u2_f0 = Vector3.Cross(u2, f0);
            Vector3 axis_u2_f1 = Vector3.Cross(u2, f1);
            Vector3 axis_u2_f2 = Vector3.Cross(u2, f2);

            // Testing axis: axis_u0_f0
            // Project all 3 vertices of the triangle onto the Seperating axis
            float p0 = Vector3.Dot(v0, axis_u0_f0);
            float p1 = Vector3.Dot(v1, axis_u0_f0);
            float p2 = Vector3.Dot(v2, axis_u0_f0);
            // Project the AABB onto the seperating axis
            // We don't care about the end points of the prjection
            // just the length of the half-size of the AABB
            // That is, we're only casting the extents onto the 
            // seperating axis, not the AABB center. We don't
            // need to cast the center, because we know that the
            // aabb is at origin compared to the triangle!
            float r = e.X * Math.Abs(Vector3.Dot(u0, axis_u0_f0)) +
                        e.Y * Math.Abs(Vector3.Dot(u1, axis_u0_f0)) +
                        e.Z * Math.Abs(Vector3.Dot(u2, axis_u0_f0));
            // Now do the actual test, basically see if either of
            // the most extreme of the triangle points intersects r
            // You might need to write Min & Max functions that take 3 arguments
            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                // This means BOTH of the points of the projected triangle
                // are outside the projected half-length of the AABB
                // Therefore the axis is seperating and we can exit
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u0_f1);
            p1 = Vector3.Dot(v1, axis_u0_f1);
            p2 = Vector3.Dot(v2, axis_u0_f1);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u0_f1)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u0_f1)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u0_f1));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u0_f2);
            p1 = Vector3.Dot(v1, axis_u0_f2);
            p2 = Vector3.Dot(v2, axis_u0_f2);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u0_f2)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u0_f2)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u0_f2));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u1_f0);
            p1 = Vector3.Dot(v1, axis_u1_f0);
            p2 = Vector3.Dot(v2, axis_u1_f0);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u1_f0)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u1_f0)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u1_f0));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u1_f1);
            p1 = Vector3.Dot(v1, axis_u1_f1);
            p2 = Vector3.Dot(v2, axis_u1_f1);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u1_f1)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u1_f1)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u1_f1));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u1_f2);
            p1 = Vector3.Dot(v1, axis_u1_f2);
            p2 = Vector3.Dot(v2, axis_u1_f2);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u1_f2)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u1_f2)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u1_f2));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u2_f0);
            p1 = Vector3.Dot(v1, axis_u2_f0);
            p2 = Vector3.Dot(v2, axis_u2_f0);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u2_f0)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u2_f0)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u2_f0));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u2_f1);
            p1 = Vector3.Dot(v1, axis_u2_f1);
            p2 = Vector3.Dot(v2, axis_u2_f1);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u2_f1)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u2_f1)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u2_f1));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            p0 = Vector3.Dot(v0, axis_u2_f2);
            p1 = Vector3.Dot(v1, axis_u2_f2);
            p2 = Vector3.Dot(v2, axis_u2_f2);

            r = e.X * Math.Abs(Vector3.Dot(u0, axis_u2_f2)) +
            e.Y * Math.Abs(Vector3.Dot(u1, axis_u2_f2)) +
            e.Z * Math.Abs(Vector3.Dot(u2, axis_u2_f2));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            // Next, we have 3 face normals from the AABB
            // for these tests we are conceptually checking if the bounding box
            // of the triangle intersects the bounding box of the AABB
            // that is to say, the seperating axis for all tests are axis aligned:
            // axis1: (1, 0, 0), axis2: (0, 1, 0), axis3 (0, 0, 1)
            // TODO: 3 SAT tests
            // Do the SAT given the 3 primary axis of the AABB
            // You already have vectors for this: u0, u1 & u2

            //axis-X
            p0 = Vector3.Dot(u0, v0);
            p1 = Vector3.Dot(u0, v1);
            p2 = Vector3.Dot(u0, v2);

            Vector2 interval_x = new Vector2(Math.Min(Math.Min(p0, p1), p2), Math.Max(Math.Max(p0, p1), p2));

            if (interval_x.X > e.X || interval_x.Y < -e.X) return false;

            p0 = Vector3.Dot(u1, v0);
            p1 = Vector3.Dot(u1, v1);
            p2 = Vector3.Dot(u1, v2);

            Vector2 interval_y = new Vector2(Math.Min(Math.Min(p0, p1), p2), Math.Max(Math.Max(p0, p1), p2));

            if (interval_y.X > e.Y || interval_y.Y < -e.Y) return false;

            p0 = Vector3.Dot(u2, v0);
            p1 = Vector3.Dot(u2, v1);
            p2 = Vector3.Dot(u2, v2);

            Vector2 interval_z = new Vector2(Math.Min(Math.Min(p0, p1), p2), Math.Max(Math.Max(p0, p1), p2));

            if (interval_z.X > e.Z || interval_z.Y < -e.Z) return false;
            // Finally, we have one last axis to test, the face normal of the triangle
            // We can get the normal of the triangle by crossing the first two line segments
            Vector3 triangleNormal = Vector3.Cross(f0, f1);
            //TODO: 1 SAT test
            p0 = Vector3.Dot(v0, triangleNormal);
            p1 = Vector3.Dot(v1, triangleNormal);
            p2 = Vector3.Dot(v2, triangleNormal);

            r = e.X * Math.Abs(Vector3.Dot(u0, triangleNormal)) +
            e.Y * Math.Abs(Vector3.Dot(u1, triangleNormal)) +
            e.Z * Math.Abs(Vector3.Dot(u2, triangleNormal));

            if (Math.Max(-Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            // Passed testing for all 13 seperating axis that exist!
            return true;
        }

        public static bool Intersects(AABB aabb, Triangle triangle)
        {
            return Intersects(triangle, aabb);
        }


        public static bool Intersects(Triangle triangle, Plane plane)
        {
            float p0 = Vector3.Dot(triangle.p0.ToVector(), plane.Normal) - plane.Distance;
            float p1 = Vector3.Dot(triangle.p1.ToVector(), plane.Normal) - plane.Distance;
            float p2 = Vector3.Dot(triangle.p2.ToVector(), plane.Normal) - plane.Distance;

            float epislon = 0.00001f;
            if(p0>epislon&&p1>epislon&&p2>epislon) return false;
            else if(p0<-epislon&&p1<-epislon&&p2<-epislon) return false;

            return true;
        }

        public static bool Intersects(Plane plane, Triangle triangle)
        {
            return Intersects(triangle, plane);
        }

        public static Vector2 GetInterval(Triangle triangle, Vector3 axis)
        {
            // Vector2.X = min
            // Vector2.Y = max.
            float p0 = Vector3.Dot(triangle.p0.ToVector(), axis);
            float p1 = Vector3.Dot(triangle.p1.ToVector(), axis);
            float p2 = Vector3.Dot(triangle.p2.ToVector(), axis);

            return new Vector2(Math.Min(Math.Min(p0,p1),p2),Math.Max(Math.Max(p0,p1),p2));
        }

        private static bool TestAxis(Triangle triangle1, Triangle triangle2, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 axis = Vector3.Cross(a - b, c - d);

            if (axis.LengthSquared() < 0.0001f)
            {
                //axis is zero, try other combination
                Vector3 n = Vector3.Cross(a - b, c - a);
                axis = Vector3.Cross(a - b, n);
                if (axis.LengthSquared() < 0.0001f)
                {
                    //axis still zero, not seperating axis
                    return false;
                }
            }
            Vector3 axisNorm = new Vector3(axis.X, axis.Y, axis.Z);
            axisNorm.Normalize();
            Vector2 i1 = GetInterval(triangle1, axisNorm);
            Vector2 i2 = GetInterval(triangle2, axisNorm);

            if (i1.Y < i2.X /*i1.max < i2.min*/ || i2.Y < i1.X /*i2.max < i1.min*/)
            {
                //intervals overlap on given axis
                return true;
            }
            return false;//no collision
        }

        public static bool Intersects(Triangle triangle1, Triangle triangle2)
        {
            //test 11axis
            //face normal of t1
            if (TestAxis(triangle1, triangle2, triangle1.p1.ToVector(), triangle1.p0.ToVector(), triangle1.p2.ToVector(), triangle1.p1.ToVector()))
            {
                //seperating axis found
                return false;
            }

            //face normal of triangle2
            if (TestAxis(triangle1, triangle2, triangle2.p1.ToVector(), triangle2.p0.ToVector(), triangle2.p2.ToVector(), triangle2.p1.ToVector()))
            {
                //seperating axis found
                return false;
            }
            /*
            //Vector3[] t1Edges = new Vector3[3] { t1.p1.ToVector() - t1.p0.ToVector(),
            //                                     t1.p2.ToVector() - t1.p1.ToVector(),
            //                                     t1.p0.ToVector() - t1.p2.ToVector() };
            */
            //go through each of the edges (0x0,0x1,0x2,1x0,1x,1x2,2x0,2x1,2x2)
            if (TestAxis(triangle1, triangle2, triangle1.p1.ToVector(), triangle1.p0.ToVector(), triangle2.p1.ToVector(), triangle2.p0.ToVector()))
            { //0x0
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p1.ToVector(), triangle1.p0.ToVector(), triangle2.p2.ToVector(), triangle2.p1.ToVector()))
            { //0x1
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p1.ToVector(), triangle1.p0.ToVector(), triangle2.p0.ToVector(), triangle2.p2.ToVector()))
            { //0x2
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p2.ToVector(), triangle1.p1.ToVector(), triangle2.p1.ToVector(), triangle2.p0.ToVector()))
            { //1x0
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p2.ToVector(), triangle1.p1.ToVector(), triangle2.p2.ToVector(), triangle2.p1.ToVector()))
            { //1x1
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p2.ToVector(), triangle1.p1.ToVector(), triangle2.p0.ToVector(), triangle2.p2.ToVector()))
            { //1x2
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p0.ToVector(), triangle1.p2.ToVector(), triangle2.p1.ToVector(), triangle2.p0.ToVector()))
            { //2x0
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p0.ToVector(), triangle1.p2.ToVector(), triangle2.p2.ToVector(), triangle2.p1.ToVector()))
            { //2x1
                return false;
            }
            if (TestAxis(triangle1, triangle2, triangle1.p0.ToVector(), triangle1.p2.ToVector(), triangle2.p0.ToVector(), triangle2.p2.ToVector()))
            { //2x2
                return false;
            }
            //no seperating axis found, no intersection
            return true;
        }


    }
}
