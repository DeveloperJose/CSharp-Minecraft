using System.Collections.Generic;
using JClient.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace JClient
{
    public sealed partial class Chunk
    {
        public void Draw(GraphicsDevice device)
        {
            if (Visible)
            {
                if (ChunkMesh.Vertices.Length > 0)
                {
                    using (VertexBuffer buffer = new VertexBuffer(
                                                   device,
                                                   VertexPositionNormalTexture.VertexDeclaration,
                                                   ChunkMesh.Vertices.Length, // Vertices
                                                   BufferUsage.WriteOnly))
                    {
                        // Load the buffer
                        buffer.SetData(ChunkMesh.Vertices);
                        // Send the vertex buffer to the device
                        device.SetVertexBuffer(buffer);
                        // Draw the primitives from the vertex buffer to the device as triangles
                        device.DrawPrimitives(PrimitiveType.TriangleList, 0, ChunkMesh.Vertices.Length / 3);
                    }
                }
            }
        }
        public Mesh CreateMesh()
        {
            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();

            for (int x = 0; x < SizeX; x++)
                for (int y = 0; y < SizeY; y++)
                    for (int z = 0; z < SizeZ; z++)
                    {
                        Vector3I Position = new Vector3I(x, y, z);

                        Vector3 realPos = new Vector3(Position.X, Position.Y, Position.Z) + ChunkPosition;
                        Vector3 cubePos = new Vector3(realPos.X + (realPos.X - 16), realPos.Z + (realPos.Z - 16), realPos.Y + (realPos.Y - 16));
                        if (this[Position].Visible)
                        {
                            Vector3 topLeftFront = cubePos + new Vector3(-1.0f, 1.0f, -1.0f);
                            Vector3 topLeftBack = cubePos + new Vector3(-1.0f, 1.0f, 1.0f);
                            Vector3 topRightFront = cubePos + new Vector3(1.0f, 1.0f, -1.0f);
                            Vector3 topRightBack = cubePos + new Vector3(1.0f, 1.0f, 1.0f);

                            // Calculate the cubePos of the vertices on the bottom face.
                            Vector3 btmLeftFront = cubePos + new Vector3(-1.0f, -1.0f, -1.0f);
                            Vector3 btmLeftBack = cubePos + new Vector3(-1.0f, -1.0f, 1.0f);
                            Vector3 btmRightFront = cubePos + new Vector3(1.0f, -1.0f, -1.0f);
                            Vector3 btmRightBack = cubePos + new Vector3(1.0f, -1.0f, 1.0f);

                            // Normal vectors for each face (needed for lighting / display)
                            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f);
                            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f);
                            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
                            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
                            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
                            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

                            if (!this[x, y, z + 1].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Top);
                                // Add the vertices for the TOP face.
                                vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalTop, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(topRightBack, normalTop, uv.TopRight));
                                vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalTop, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalTop, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(topRightFront, normalTop, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightBack, normalTop, uv.TopRight));
                            }
                            //if (!this[x, y - 1, z).Visible)
                            if (!this[x, y, z - 1].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Bottom);
                                // Add the vertices for the BOTTOM face.
                                vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalBottom, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBottom, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBottom, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalBottom, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBottom, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalBottom, uv.TopRight));
                            }
                            //if (!this[x, y, z + 1).Visible)
                            if (!this[x, y + 1, z].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Back);
                                // Add the vertices for the BACK face.
                                vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalBack, uv.TopRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightBack, normalBack, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBack, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBack, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightBack, normalBack, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBack, uv.BottomLeft));
                            }
                            //if (!this[x, y, z - 1).Visible)
                            if (!this[x, y - 1, z].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Front);
                                // Add the vertices for the FRONT face.
                                vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalFront, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalFront, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(topRightFront, normalFront, uv.TopRight));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalFront, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalFront, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightFront, normalFront, uv.TopRight));
                            }
                            if (!this[x - 1, y, z].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Left);
                                // Add the vertices for the LEFT face.
                                vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalLeft, uv.TopRight));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalLeft, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalLeft, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalLeft, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalLeft, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalLeft, uv.TopRight));
                            }
                            if (!this[x + 1, y, z].Visible)
                            {
                                UVMap uv = this[x, y, z].CreateUVMapping(Direction.Right);
                                // Add the vertices for the RIGHT face. 
                                vertices.Add(new VertexPositionNormalTexture(topRightFront, normalRight, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalRight, uv.BottomLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalRight, uv.BottomRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightBack, normalRight, uv.TopRight));
                                vertices.Add(new VertexPositionNormalTexture(topRightFront, normalRight, uv.TopLeft));
                                vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalRight, uv.BottomRight));
                            }
                        }
                    }
            Mesh ret = new Mesh()
            {
                Vertices = vertices.ToArray(),
            };
            vertices.Clear();
            return ret;
        }
    }
}
