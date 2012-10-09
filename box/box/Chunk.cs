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

        public Chunk()
        {
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int k = 0;k < CHUNK_SIZE; k++)
                    {
                        blocks[i,j,k] = new Block(new Vector3(1.0f), new Vector3(i, j, k), k % 3);
                    }
                }
            }

        }


        public void Update(GameTime gametime)
        {

        }
        public void Draw(GameTime gametime)
        {

        }


    }
}
