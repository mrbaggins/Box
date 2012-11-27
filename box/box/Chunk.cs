using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Box
{
    public struct VertexPositionColorNormal : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                );

        public VertexPositionColorNormal(Vector3 pos, Color c, Vector3 n)
        {
            Position = pos;
            Color = c;
            Normal = n;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    class Chunk
    {
        static int CHUNK_SIZE = 16;
        private Block[,,] blocks;
        private bool isDirty;

        private List<VertexPositionColorNormal> vertexList;
        private List<int> indexList;

        public List<VertexPositionColorNormal> VertexList
        {
            set { vertexList = value;}
            get { return vertexList; }
        }
        public List<int> IndexList
        {
            set { this.indexList = value;}
            get { return indexList; }
        }

        public Chunk()
        {
            blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            VertexList = new List<VertexPositionColorNormal>();
            IndexList = new List<int>();


            //Old stripey generation
            
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int k = 0;k < CHUNK_SIZE; k++)
                    {
                        blocks[i,j,k] = new Block(new Vector3(0.1f), new Vector3(i, j, k), k % 3 + 1);
                    }
                }
            }
            

            blocks[0, 0, 0] = new Block(new Vector3(0.2f), Vector3.Zero, 0);
            blocks[CHUNK_SIZE-1, CHUNK_SIZE-1, CHUNK_SIZE-1] = new Block(new Vector3(0.2f), new Vector3(CHUNK_SIZE), 0);

            isDirty = true;
            regenerateMesh();

            
        }

        public void regenerateMesh()
        {
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int k = 0; k < CHUNK_SIZE; k++)
                    {
                        if (blocks[i, j, k].isActive == false)
                            continue;
                        //Else
                        CreateBox(i,j,k, blocks[i,j,k].Color);
                    }
                }
            }

            this.isDirty = false;
        }

        private void CreateBox(int x, int y, int z, Color c) 
        {
            /* Points for the 8 corners.
             * (0,0,0) is the front, top left corner
             * x increases to right
             * y increases to Up
             * z increases away
             * */

            bool debug = false;
            
            Vector3 p1 = new Vector3(x + 0, y + 0, z + 0);
            Vector3 p2 = new Vector3(x + 1, y + 0, z + 0);
            Vector3 p3 = new Vector3(x + 0, y - 1, z + 0);
            Vector3 p4 = new Vector3(x + 1, y - 1, z + 0);
            Vector3 p5 = new Vector3(x + 0, y + 0, z + 1);
            Vector3 p6 = new Vector3(x + 1, y + 0, z + 1);
            Vector3 p7 = new Vector3(x + 0, y - 1, z + 1);
            Vector3 p8 = new Vector3(x + 1, y - 1, z + 1);

            Vector3 normal;

            //For temporary storage of the vertex index until it is added to indexBuffer
            int v1,v2,v3,v4,v5,v6,v7,v8;

            //Front of the box
            normal = new Vector3(0.0f, 0.0f, 1.0f);
            
            if (debug)
                c = Color.Red;

            v1 = AddVertexToMesh(p1, c, normal);
            v2 = AddVertexToMesh(p2, c, normal);
            v3 = AddVertexToMesh(p3, c, normal);
            v4 = AddVertexToMesh(p4, c, normal);

            AddTriangleToMesh(v1, v3, v2);
            AddTriangleToMesh(v2, v3, v4);

            //Back side of the box
            normal = new Vector3(0.0f, 0.0f, -1.0f);

            if (debug)
                c = Color.Orange;

            v5 = AddVertexToMesh(p5, c, normal);
            v6 = AddVertexToMesh(p6, c, normal);
            v7 = AddVertexToMesh(p7, c, normal);
            v8 = AddVertexToMesh(p8, c, normal);

            AddTriangleToMesh(v5, v6, v7);
            AddTriangleToMesh(v6, v8, v7);

            //Left
            normal = new Vector3(-1.0f, 0.0f, 0.0f);
            
            if (debug)
                c = Color.Yellow;

            v1 = AddVertexToMesh(p1, c, normal);
            v3 = AddVertexToMesh(p3, c, normal);
            v5 = AddVertexToMesh(p5, c, normal);
            v7 = AddVertexToMesh(p7, c, normal);

            AddTriangleToMesh(v1, v5, v3);
            AddTriangleToMesh(v3, v5, v7);

            //Right
            normal = new Vector3(1.0f, 0.0f, 0.0f);

            if (debug)
                c = Color.Green;

            v2 = AddVertexToMesh(p2, c, normal);
            v4 = AddVertexToMesh(p4, c, normal);
            v6 = AddVertexToMesh(p6, c, normal);
            v8 = AddVertexToMesh(p8, c, normal);

            AddTriangleToMesh(v2, v4, v6);
            AddTriangleToMesh(v4, v8, v6);

            //Top
            normal = new Vector3(0.0f, 1.0f, 0.0f);

            if (debug)
                c = Color.Blue;

            v1 = AddVertexToMesh(p1, c, normal);
            v2 = AddVertexToMesh(p2, c, normal);
            v5 = AddVertexToMesh(p5, c, normal);
            v6 = AddVertexToMesh(p6, c, normal);

            AddTriangleToMesh(v1, v2, v5);
            AddTriangleToMesh(v2, v6, v5);

            //Bottom
            normal = new Vector3(0.0f, -1.0f, 0.0f);

            if (debug)
                c = Color.Purple;

            v3 = AddVertexToMesh(p3, c, normal);
            v4 = AddVertexToMesh(p4, c, normal);
            v7 = AddVertexToMesh(p7, c, normal);
            v8 = AddVertexToMesh(p8, c, normal);

            AddTriangleToMesh(v3, v7, v4);
            AddTriangleToMesh(v4, v7, v8);

        }

        private int AddVertexToMesh(Vector3 point, Color color, Vector3 normal)
        {
            //Make it into a vertex
            VertexPositionColorNormal v = new VertexPositionColorNormal(point, color, normal);

            //Check if vertex already exists
            //if (VertexList.Contains(v))
                //return VertexList.IndexOf(v);
            //else
            {
                VertexList.Add(v);
                return VertexList.Count - 1;
            }
        }

        private void AddTriangleToMesh(int a, int b, int c)
        {
            IndexList.Add(a);
            IndexList.Add(b);
            IndexList.Add(c);
        }
    }
}
