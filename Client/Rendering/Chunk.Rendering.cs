using System.Collections.Generic;
using System.Linq;
using Client.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{

    public sealed partial class Chunk
    {
        public void Draw(GraphicsDevice device)
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

        // Normal vectors for each face (needed for lighting / display)
        private static readonly Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f);
        private static readonly Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f);
        private static readonly Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
        private static readonly Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
        private static readonly Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
        private static readonly Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

        public Mesh CreateMesh()
        {
            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    for (int z = 0; z < Size.Z; z++)
                    {
                        Vector3I Position = new Vector3I(x, y, z);

                        if (this[Position].Visible()
                            || this[Position].Transparent()) // We still want to render them to see them.
                        {
                            Vector3 size = new Vector3(1, 1, 1);

                            Vector3 realPos = (Position + ChunkPosition).ToRenderCoords();
                            //Vector3 realPos = new Vector3(Position.X, Position.Y, Position.Z) + ChunkPosition; // Array position
                            Vector3 cubePos = new Vector3(realPos.X, realPos.Y, realPos.Z); // View position

                            // Top face
                            Vector3 topLeftFront = cubePos + new Vector3(-1.0f, 1.0f, -1.0f) * size;
                            Vector3 topLeftBack = cubePos + new Vector3(-1.0f, 1.0f, 1.0f) * size;
                            Vector3 topRightFront = cubePos + new Vector3(1.0f, 1.0f, -1.0f) * size;
                            Vector3 topRightBack = cubePos + new Vector3(1.0f, 1.0f, 1.0f) * size;

                            // Calculate the cubePos of the vertices on the bottom face.
                            Vector3 btmLeftFront = cubePos + new Vector3(-1.0f, -1.0f, -1.0f) * size;
                            Vector3 btmLeftBack = cubePos + new Vector3(-1.0f, -1.0f, 1.0f) * size;
                            Vector3 btmRightFront = cubePos + new Vector3(1.0f, -1.0f, -1.0f) * size;
                            Vector3 btmRightBack = cubePos + new Vector3(1.0f, -1.0f, 1.0f) * size;

                            /* Start of Vertices */
                            if (!this[x, y, z + 1].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Top);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the TOP face.
                                    vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalTop, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(topRightBack, normalTop, uv.TopRight));
                                    vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalTop, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalTop, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(topRightFront, normalTop, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightBack, normalTop, uv.TopRight));
                                }
                            }
                            //if (!this[x, y - 1, z).Visible)
                            if (!this[x, y, z - 1].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Bottom);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the BOTTOM face.
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalBottom, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBottom, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBottom, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalBottom, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBottom, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalBottom, uv.TopRight));
                                }
                            }
                            //if (!this[x, y, z + 1).Visible)
                            if (!this[x, y + 1, z].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Back);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the BACK face.
                                    vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalBack, uv.TopRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightBack, normalBack, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBack, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalBack, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightBack, normalBack, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalBack, uv.BottomLeft));
                                }
                            }
                            //if (!this[x, y, z - 1).Visible)
                            if (!this[x, y - 1, z].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Front);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the FRONT face.
                                    vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalFront, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalFront, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(topRightFront, normalFront, uv.TopRight));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalFront, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalFront, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightFront, normalFront, uv.TopRight));
                                }
                            }
                            if (!this[x - 1, y, z].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Left);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the LEFT face.
                                    vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalLeft, uv.TopRight));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalLeft, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftFront, normalLeft, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(topLeftBack, normalLeft, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmLeftBack, normalLeft, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(topLeftFront, normalLeft, uv.TopRight));
                                }
                            }
                            if (!this[x + 1, y, z].Visible())
                            {
                                UVMap? uvMap = this[x, y, z].CreateUVMapping(Direction.Right);
                                if (uvMap != null)
                                {
                                    UVMap uv = uvMap.GetValueOrDefault();
                                    // Add the vertices for the RIGHT face. 
                                    vertices.Add(new VertexPositionNormalTexture(topRightFront, normalRight, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightFront, normalRight, uv.BottomLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalRight, uv.BottomRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightBack, normalRight, uv.TopRight));
                                    vertices.Add(new VertexPositionNormalTexture(topRightFront, normalRight, uv.TopLeft));
                                    vertices.Add(new VertexPositionNormalTexture(btmRightBack, normalRight, uv.BottomRight));
                                }
                            }
                            /* End of Vertices */
                        }
                    }
            Mesh ret = new Mesh()
            {
                Vertices = vertices.ToArray()
            };
            vertices.Clear();
            return ret;
        }
    }
}
