using System;
using System.Collections.Generic;
using System.IO;
using Math_Implementation;
using CollisionDetectionSelector.Primitives;
using OpenTK.Graphics.OpenGL;

namespace CollisionDetectionSelector
{
    class OBJLoader
    {
        protected int vertexBuffer = -1;

        protected bool hasNormals = false;
        protected bool hasUvs = false;

        protected int numVerts = 0;
        protected int numNormals = 0;
        protected int numUvs = 0;

        protected string materialPath = null;
        protected Triangle[] collisionMesh = null;
        protected AABB containerAABB = null;
        protected Sphere containerSphere = null;
        protected BVHNode bvhRoot = null;
        public OBJLoader(string path)
        {
            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> texCoord = new List<float>();

            List<uint> vertIndex = new List<uint>();
            List<uint> normIndex = new List<uint>();
            List<uint> uvIndex = new List<uint>();

            List<float> vertexData = new List<float>();
            List<float> normalData = new List<float>();
            List<float> uvData = new List<float>();

            using (TextReader reader = File.OpenText(path))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (string.IsNullOrEmpty(line) || line.Length < 2)
                    {
                        continue;
                    }
                    line.ToLower();
                    string[] content = line.Split(' ');
                    if (content[0] == "usemtl")
                    {
                        materialPath = content[1];
                    }
                    else if (content[0] == "v")
                    {
                        //add vertex
                        //vertices
                        vertices.Add(System.Convert.ToSingle(content[1]));
                        vertices.Add(System.Convert.ToSingle(content[2]));
                        vertices.Add(System.Convert.ToSingle(content[3]));
                    }
                    else if (content[0] == "vt")
                    {
                        // vertex texture
                        texCoord.Add(System.Convert.ToSingle(content[1]));
                        texCoord.Add(System.Convert.ToSingle(content[2]));

                    }
                    else if (content[0] == "vn")
                    {
                        //vertex normal
                        //normals
                        normals.Add(System.Convert.ToSingle(content[1]));
                        normals.Add(System.Convert.ToSingle(content[2]));
                        normals.Add(System.Convert.ToSingle(content[3]));
                    }
                    else if (content[0] == "f")
                    {
                        //face
                        for (int i = 1; i < content.Length; i++)
                        { //loop through values
                            string[] subsplit = content[i].Split('/'); // split based on /
                            if (!string.IsNullOrEmpty(subsplit[0]))
                            {
                                //vertindex
                                vertIndex.Add(System.Convert.ToUInt32(subsplit[0]) - 1);
                            }
                            if (!string.IsNullOrEmpty(subsplit[1]))
                            {
                                //uvindex
                                uvIndex.Add(System.Convert.ToUInt32(subsplit[1]) - 1);
                            }
                            if (!string.IsNullOrEmpty(subsplit[2]))
                            {
                                //normindex
                                normIndex.Add(System.Convert.ToUInt32(subsplit[2]) - 1);
                            }
                        }
                    }
                    else if (content[0] == "s")
                    {
                        //specular
                    }
                    line = reader.ReadLine();
                }

            }

            for (int i = 0; i < vertIndex.Count; i++)
            {
                vertexData.Add(vertices[(int)vertIndex[i] * 3 + 0]);
                vertexData.Add(vertices[(int)vertIndex[i] * 3 + 1]);
                vertexData.Add(vertices[(int)vertIndex[i] * 3 + 2]);
            }
            for (int i = 0; i < normIndex.Count; i++)
            {
                normalData.Add(normals[(int)normIndex[i] * 3 + 0]);
                normalData.Add(normals[(int)normIndex[i] * 3 + 1]);
                normalData.Add(normals[(int)normIndex[i] * 3 + 2]);
            }
            for (int i = 0; i < uvIndex.Count; i++)
            {
                uvData.Add(texCoord[(int)uvIndex[i] * 2 + 0]);
                uvData.Add(texCoord[(int)uvIndex[i] * 2 + 1]);
            }

            collisionMesh = new Triangle[vertexData.Count / 9];
            containerAABB = new AABB(new Point(vertices[0], vertices[1], vertices[2]),
                                        new Point(vertices[0], vertices[1], vertices[2]));

            for (int i = 0; i < vertexData.Count / 3; ++i)
            {
                if (vertexData[i * 3 + 0] < containerAABB.Min.X)
                {
                    containerAABB.Min.X = vertexData[i * 3 + 0];
                }
                if (vertexData[i * 3 + 0] > containerAABB.Max.X)
                {
                    containerAABB.Max.X = vertexData[i * 3 + 0];
                }
                if (vertexData[i * 3 + 1] < containerAABB.Min.Y)
                {
                    containerAABB.Min.Y = vertexData[i * 3 + 1];
                }
                if (vertexData[i * 3 + 1] > containerAABB.Max.Y)
                {
                    containerAABB.Max.Y = vertexData[i * 3 + 1];
                }
                if (vertexData[i * 3 + 2] < containerAABB.Min.Z)
                {
                    containerAABB.Min.Z = vertexData[i * 3 + 2];
                }
                if (vertexData[i * 3 + 2] > containerAABB.Max.Z)
                {
                    containerAABB.Max.Z = vertexData[i * 3 + 2];
                }
            }

            bvhRoot = new BVHNode(containerAABB);

            for (int i = 0; i < collisionMesh.Length; ++i)
            {
                collisionMesh[i] =
                new Triangle(new Point(vertexData[i * 9 + 0], vertexData[i * 9 + 1], vertexData[i * 9 + 2]),
                             new Point(vertexData[i * 9 + 3], vertexData[i * 9 + 4], vertexData[i * 9 + 5]),
                             new Point(vertexData[i * 9 + 6], vertexData[i * 9 + 7], vertexData[i * 9 + 8]));

                bvhRoot.Triangles.Add(collisionMesh[i]);
            }

            //split BVHNode
            bvhRoot.Split();

            Vector3 difference_min = containerAABB.Min.ToVector() - containerAABB.Center.ToVector();
            Vector3 dirrerence_max = containerAABB.Max.ToVector() - containerAABB.Center.ToVector();
            float radius = Math.Max(difference_min.Length(), dirrerence_max.Length());
            containerSphere = new Sphere(containerAABB.Center, radius);

            hasNormals = normalData.Count > 0;
            hasUvs = uvData.Count > 0;

            numVerts = vertexData.Count;
            numUvs = uvData.Count;
            numNormals = normalData.Count;

            List<float> data = new List<float>();//create linear array of data
            data.AddRange(vertexData); //add everything from vertData
            data.AddRange(normalData);//add everything from normData
            data.AddRange(uvData);//add everything from uvData

            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);//bind array
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Count * sizeof(float)), data.ToArray(), BufferUsageHint.StaticDraw);//transfer array to gpu
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);//release array
        }
        public Sphere BoundingSphere
        {
            get
            {
                return containerSphere;
            }
        }
        public AABB BoundingBox
        {
            get
            {
                return containerAABB;
            }
        }

        public int NumCollisionTriangles
        {
            get
            {
                return collisionMesh.Length;
            }
        }

        public Triangle[] CollisionMesh
        {
            get
            {
                return collisionMesh;
            }
        }
        public BVHNode BvhRoot
        {
            get
            {
                return bvhRoot;
            }
        }

        // Non-Recursive public API
        public void RenderBVH()
        {
            RenderBVH(bvhRoot);
        }

        // Recursive private API
        private void RenderBVH(BVHNode node)
        {
            node.AABB.Render();

            if (node.Children != null)
            {
                foreach (BVHNode child in node.Children)
                {
                    RenderBVH(child);
                }
            }
        }
        public void DebugRender()
        {
            containerSphere.Render();
            containerAABB.Render();
            foreach (Triangle trianlge in collisionMesh)
            {
                trianlge.Render();
            }
        }

        public void Destroy()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DeleteBuffer(vertexBuffer);

            vertexBuffer = -1;
        }
        public void Render(bool useNormals = true, bool useTextures = true)
        {
            if (vertexBuffer == -1)
            {
                return;
            }
            if (!hasNormals)
            {
                useNormals = false;
            }
            if (!hasUvs)
            {
                useTextures = false;
            }
            //enable client states , check arguments
            GL.EnableClientState(ArrayCap.VertexArray);
            if (hasNormals)
            {
                GL.EnableClientState(ArrayCap.NormalArray);
            }
            if (hasUvs)
            {
                GL.EnableClientState(ArrayCap.TextureCoordArray);
            }
            //bind array buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            //set pointers
            GL.VertexPointer(3, VertexPointerType.Float, 0, new System.IntPtr(0));
            if (hasNormals)
            {
                GL.NormalPointer(NormalPointerType.Float, 0, new System.IntPtr(numVerts * sizeof(float)));
            }
            if (hasUvs)
            {
                GL.TexCoordPointer(2, TexCoordPointerType.Int, 0, new System.IntPtr((numVerts + numNormals) * sizeof(float)));
            }

            //call GL.DrawArrays, always triangles
            GL.DrawArrays(PrimitiveType.Triangles, 0, numVerts);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            if (hasUvs)
            {
                GL.DisableClientState(ArrayCap.TextureCoordArray);
            }
            if (hasNormals)
            {
                GL.DisableClientState(ArrayCap.NormalArray);
            }
            GL.DisableClientState(ArrayCap.VertexArray);
        }



    }
}
