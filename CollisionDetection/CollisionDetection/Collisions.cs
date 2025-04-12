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
    }
}
