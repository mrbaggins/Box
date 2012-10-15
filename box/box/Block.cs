using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Box
{
    //public struct VertexPositionColorNormal : IVertexType
    //{
    //    public Vector3 Position;
    //    public Color Color;
    //    public Vector3 Normal;

    //    public readonly static VertexDeclaration VertexDeclaration
    //        = new VertexDeclaration(
    //            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
    //            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
    //            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
    //            );

    //    public VertexPositionColorNormal(Vector3 pos, Color c, Vector3 n)
    //    {
    //        Position = pos;
    //        Color = c;
    //        Normal = n;

    //    }

    //    VertexDeclaration IVertexType.VertexDeclaration
    //    {
    //        get { return VertexDeclaration; }
    //    }
    //}

    public class Block
    {
        const int NUM_TRIANGLES = 12;
        const int NUM_VERTICES = 36;
        public enum BlockTypes { Grass, Dirt, Stone };


        // Array of vertex information - contains position, normal and texture data
        private VertexPositionColorNormal[] vertices;

        // The vertex buffer where we load the vertices before drawing the shape
        //private VertexBuffer _shapeBuffer;

        // Lets us check if the data has been constructed or not to improve performance
        private bool _isConstructed = false;
        public void setDirty()
        {
            _isConstructed = false;
        }

        public Vector3 Size { get; set; }
        private Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Texture { get; set; }
        public Color Color { get; set; }

        /// <summary>
        /// Initializes the size and position parameters for this cube.
        /// </summary>
        /// <param name="size">A Vector3 object representing the size in each dimension</param>
        /// <param name="position">A Vector3 object representing the position</param>
        public Block(Vector3 size, Vector3 position, int blockID)
        {
            Size = size;
            this.position = position;
            switch (blockID)
            {
                case 0:
                    Color = new Color(20, 120, 10);
                    break;
                case 1:
                    Color = new Color(128, 100, 0);
                    break;
                case 2:
                    Color = new Color(128, 120, 100);
                    break;
                default:
                    Color = new Color(0, 0, 0);
                    break;
            }
            
        }

        /// <summary>
        /// Writes our list of vertices to the vertex buffer, 
        /// then draws triangles to the device
        /// </summary>
        /// <param name="device"></param>
        public void RenderToDevice(GraphicsDevice device)
        {
            // Build the cube, setting up the _vertices array
            if (_isConstructed == false)
                ConstructCube();

            // Create the shape buffer and dispose of it to prevent out of memory
            using (VertexBuffer buffer = new VertexBuffer(
                device,
                VertexPositionColorNormal.VertexDeclaration,
                NUM_VERTICES,
                BufferUsage.WriteOnly))
            {
                // Load the buffer
                buffer.SetData(vertices);

                // Send the vertex buffer to the device
                device.SetVertexBuffer(buffer);
            }

            // Draw the primitives from the vertex buffer to the device as triangles
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, NUM_TRIANGLES);

            //Can't get indexed to work at the moment because for some reason I can't get the buffers set.
            //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 3, 0, 1);            
        }
        
        private void ConstructCube()
        {
            vertices = new VertexPositionColorNormal[NUM_VERTICES];

            //Values are set from the bottomleftfront corner of a box.

            // Calculate the position of the vertices on the top face.
            Vector3 topLeftFront = position + new Vector3(0.0f, 1.0f, 0.0f) * Size;
            Vector3 topLeftBack = position + new Vector3(0.0f, 1.0f, 1.0f) * Size;
            Vector3 topRightFront = position + new Vector3(1.0f, 1.0f, 0.0f) * Size;
            Vector3 topRightBack = position + new Vector3(1.0f, 1.0f, 1.0f) * Size;

            // Calculate the position of the vertices on the bottom face.
            Vector3 btmLeftFront = position + new Vector3(0.0f, 0.0f, 0.0f) * Size;
            Vector3 btmLeftBack = position + new Vector3(0.0f, 0.0f, 1.0f) * Size;
            Vector3 btmRightFront = position + new Vector3(1.0f, 0.0f, 0.0f) * Size;
            Vector3 btmRightBack = position + new Vector3(1.0f, 0.0f, 1.0f) * Size;

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f) * Size;
            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * Size;
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * Size;
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * Size;
            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * Size;
            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * Size;

            // UV texture coordinates
            Vector2 textureTopLeft = new Vector2(1.0f * Size.X, 0.0f * Size.Y);
            Vector2 textureTopRight = new Vector2(0.0f * Size.X, 0.0f * Size.Y);
            Vector2 textureBottomLeft = new Vector2(1.0f * Size.X, 1.0f * Size.Y);
            Vector2 textureBottomRight = new Vector2(0.0f * Size.X, 1.0f * Size.Y);

            
            // Add the vertices for the FRONT face.
            vertices[0] = new VertexPositionColorNormal(topLeftFront, Color, normalFront);
            vertices[1] = new VertexPositionColorNormal(btmLeftFront, Color, normalFront);
            vertices[2] = new VertexPositionColorNormal(topRightFront, Color, normalFront);
            vertices[3] = new VertexPositionColorNormal(btmLeftFront, Color, normalFront);
            vertices[4] = new VertexPositionColorNormal(btmRightFront, Color, normalFront);
            vertices[5] = new VertexPositionColorNormal(topRightFront, Color, normalFront);

            // Add the vertices for the BACK face.
            vertices[6] = new VertexPositionColorNormal(topLeftBack, Color, normalBack);
            vertices[7] = new VertexPositionColorNormal(topRightBack, Color, normalBack);
            vertices[8] = new VertexPositionColorNormal(btmLeftBack, Color, normalBack);
            vertices[9] = new VertexPositionColorNormal(btmLeftBack, Color, normalBack);
            vertices[10] = new VertexPositionColorNormal(topRightBack, Color, normalBack);
            vertices[11] = new VertexPositionColorNormal(btmRightBack, Color, normalBack);
            
            // Add the vertices for the TOP face.
            vertices[12] = new VertexPositionColorNormal(topLeftFront, Color, normalTop);
            vertices[13] = new VertexPositionColorNormal(topRightBack, Color, normalTop);
            vertices[14] = new VertexPositionColorNormal(topLeftBack, Color, normalTop);
            vertices[15] = new VertexPositionColorNormal(topLeftFront, Color, normalTop);
            vertices[16] = new VertexPositionColorNormal(topRightFront, Color, normalTop);
            vertices[17] = new VertexPositionColorNormal(topRightBack, Color, normalTop);

            // Add the vertices for the BOTTOM face. 
            vertices[18] = new VertexPositionColorNormal(btmLeftFront, Color,normalBottom);
            vertices[19] = new VertexPositionColorNormal(btmLeftBack, Color, normalBottom);
            vertices[20] = new VertexPositionColorNormal(btmRightBack, Color, normalBottom);
            vertices[21] = new VertexPositionColorNormal(btmLeftFront, Color, normalBottom);
            vertices[22] = new VertexPositionColorNormal(btmRightBack, Color, normalBottom);
            vertices[23] = new VertexPositionColorNormal(btmRightFront, Color, normalBottom);

            // Add the vertices for the LEFT face.
            vertices[24] = new VertexPositionColorNormal(topLeftFront, Color, normalLeft);
            vertices[25] = new VertexPositionColorNormal(btmLeftBack, Color, normalLeft);
            vertices[26] = new VertexPositionColorNormal(btmLeftFront, Color, normalLeft);
            vertices[27] = new VertexPositionColorNormal(topLeftBack, Color, normalLeft);
            vertices[28] = new VertexPositionColorNormal(btmLeftBack, Color, normalLeft);
            vertices[29] = new VertexPositionColorNormal(topLeftFront, Color, normalLeft);

            // Add the vertices for the RIGHT face. 
            vertices[30] = new VertexPositionColorNormal(topRightFront, Color, normalRight);
            vertices[31] = new VertexPositionColorNormal(btmRightFront, Color, normalRight);
            vertices[32] = new VertexPositionColorNormal(btmRightBack, Color, normalRight);
            vertices[33] = new VertexPositionColorNormal(topRightBack, Color, normalRight);
            vertices[34] = new VertexPositionColorNormal(topRightFront, Color, normalRight);
            vertices[35] = new VertexPositionColorNormal(btmRightBack, Color, normalRight);
            
            _isConstructed = true;
        }
    }
}
