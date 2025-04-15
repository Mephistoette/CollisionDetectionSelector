using System;
using System.Collections.Generic;
using Math_Implementation;
using CollisionDetectionSelector.Primitives;
using OpenTK.Graphics.OpenGL;

namespace CollisionDetectionSelector
{
    class OctreeNode
    {
        public AABB Bounds = null;
        public OctreeNode Parent = null;
        public List<OctreeNode> Children = null;
        public List<OBJ> Contents = null;

        public OctreeNode(Point position, float halfSize, OctreeNode parent)
        {
            Bounds = new AABB(
                new Point(position.X - halfSize, position.Y - halfSize, position.Z - halfSize),
                new Point(position.X + halfSize, position.Y + halfSize, position.Z + halfSize)
            );
            Parent = parent;
            Contents = new List<OBJ>();
            // Keep children null, this is a leaf node until the "Split" function is called
        }

        public void Split(int level)
        {
            // Bounds.Extens.x is the same as y and z, all nodes are square
            // We want each child node to be half as big as this node
            float splitSize = Bounds.Extents.X / 2.0f;

            // Also, the constructor of the Octree node takes in the CENTER
            // position of the new node, therefore we want to create new
            // nodes from the center of this node + or - 1/4 the size of
            // this node. This is similar to BVH, but different in size.
            Vector3[] childPattern = new Vector3[] {
        new Vector3(+1f, -1f, -1f), // Right, Top, Front
        new Vector3(+1f, -1f, +1f), // Right, Top, Back
        new Vector3(+1f, +1f, -1f), // Right, Bottom, Front
        new Vector3(+1f, +1f, +1f), // Right, Bottom, Back
        new Vector3(-1f, -1f, -1f), // Left, Top, Front
        new Vector3(-1f, -1f, +1f), // Left, Top, Back
        new Vector3(-1f, +1f, -1f), // Left, Bottom, Front
        new Vector3(-1f, +1f, +1f), // Left, Bottom, Back
        };

            // Make the actual child nodes
            Children = new List<OctreeNode>();
            Contents = null; // We are no longer a leaf node!
            foreach (Vector3 offset in childPattern)
            {
                // Remember to account for the center of the current node
                Point position = new Point(Bounds.Center.ToVector() + offset * splitSize);
                OctreeNode child = new OctreeNode(position, splitSize, this);
                Children.Add(child);
            }

            // If we have not reached the max split depth yet, 
            // go ahead and recursivley call this method for all children
            if (level > 1)
            {
                foreach (OctreeNode child in Children)
                {
                    child.Split(level - 1);
                }
            }
        }

        public bool Insert(OBJ obj)
        {
            // Remember, the bounding sphere is model space, we need it to
            // be world space! Multiply the position and scale the radius
            Sphere worldSpaceSphere = new Sphere();
            worldSpaceSphere.Position = new Point(
                Matrix4.MultiplyPoint(obj.WorldMatrix, obj.BoundingSphere.Position.ToVector())
            ); // End new point
            float scale = Math.Max(obj.WorldMatrix[0, 0], Math.Max(obj.WorldMatrix[1, 1], obj.WorldMatrix[2, 2]));
            worldSpaceSphere.Radius = obj.BoundingSphere.Radius * scale;

            if (Collisions.Intersects(worldSpaceSphere, Bounds))
            {
                // We knoe the obj intersects this node, if it's a leaf
                // add it to the object list, if not, recurse!
                if (Children != null)
                {
                    foreach (OctreeNode child in Children)
                    {
                        child.Insert(obj);
                    }
                }
                else
                {
                    Contents.Add(obj);
                }
                return true;
            }
            return false;
        }

        public void Remove(OBJ obj)
        {
            if (Children != null)
            {
                foreach (OctreeNode child in Children)
                {
                    child.Remove(obj);
                }
            }
            else
            {
                Contents.Remove(obj);
            }
        }

        public bool Update(OBJ obj)
        {
            if (obj.Children != null)
            {
                foreach (OBJ child in obj.Children)
                {
                    Update(child);
                }
            }
            Remove(obj);
            return Insert(obj);
        }

        public void DebugRender()
        {
            if (debugVisited)
            {
                GL.Color3(0f, 1f, 0f);
            }
            else
            {
                GL.Color3(0f, 0f, 1f);

            }
            Bounds.Render();
            if (Children != null)
            {
                foreach (OctreeNode node in Children)
                {
                    node.DebugRender();
                }
            }
        }

        public OBJ Raycast(Ray ray, out float t)
        {
            debugVisited = true;
            // This is NOT a leaf node, see which child can be raycast against!
            if (Children != null)
            {
                foreach (OctreeNode child in Children)
                {
                    // Does the ray intersect the AABB of the node in question?
                    if (Collisions.Raycast(ray, child.Bounds, out t))
                    {
                        // If so, recursivley call the raycast method!
                        OBJ result = child.Raycast(ray, out t);
                        // If we hit something, return it!
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            // This IS a leaf node! See if any of the contents are hit by the ray.
            // If we made it this far, then the Bounds already intersect the ray,
            // no bounds check for the OctreeNode is needed!
            else if (Contents != null)
            {
                // Loop trough all children
                foreach (OBJ content in Contents)
                {
                    // Return the first child that was hit!
                    if (Collisions.Raycast(ray, content, out t))
                    {
                        return content;
                    }
                }
            }

            t = 0f;
            return null;
        }

        protected bool debugVisited = false;

        public void DebugRenderOnlyVisitedNodes()
        {
            if (!debugVisited)
            {
                return;
            }
            if (Children != null)
            {
                foreach (OctreeNode node in Children)
                {
                    node.DebugRenderOnlyVisitedNodes();
                }
            }
            else
            {
                GL.Color3(0f, 1f, 0f);
                Bounds.Render();
            }
        }

    }


}