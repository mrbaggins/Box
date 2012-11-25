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
        static int CHUNK_SIZE = 8;
        private Block[,,] blocks;
        private bool isDirty = true;

        private List<VertexPositionColorNormal> vertexList;
        private List<int> indexList;

        public Chunk()
        {
            blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

            /* Old stripey generation
             * 
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
             * */

            blocks[0, 0, 0] = new Block(new Vector3(0.2f), Vector3.Zero, 0);

            this.isDirty = true;

            
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
             * y increases to down
             * z increases away
             * */
            
            Vector3 p1 = new Vector3(x + 0, y + 0, z + 0);
            Vector3 p2 = new Vector3(x + 1, y + 0, z + 0);
            Vector3 p3 = new Vector3(x + 0, y + 1, z + 0);
            Vector3 p4 = new Vector3(x + 1, y + 1, z + 0);
            Vector3 p5 = new Vector3(x + 0, y + 0, z + 1);
            Vector3 p6 = new Vector3(x + 1, y + 0, z + 1);
            Vector3 p7 = new Vector3(x + 0, y + 1, z + 1);
            Vector3 p8 = new Vector3(x + 1, y + 1, z + 1);

            Vector3 normal;

            //For temporary storage of the vertex index until it is added to indexBuffer
            uint v1,v2,v3,v4,v5,v6,v7,v8;


        }

        private uint AddVertexToMesh(Vector3 point, Color color, Vector3 normal)
        {
            //Check if vertex already exists
            //If it does, return the index
            //If it doesnt, add the new vertex, and return the index.


            return 0; //Index of the vertex in the List<>
        }
    }
}
