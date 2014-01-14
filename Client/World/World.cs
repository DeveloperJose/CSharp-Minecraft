using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public sealed partial class World
    {
        public readonly int Length;
        public readonly int Width;
        public readonly int Height;
        private Chunk[, ,] Chunks;
        public BlockID this[Vector3I pos]
        {
            get { return this[pos.X, pos.Y, pos.Z]; }
            set { this[pos.X, pos.Y, pos.Z] = value; }
        }
        public BlockID this[int x, int y, int z]
        {
            get
            {
                //if (!InBounds(x, y, z)) // Not within world bounds.
                //  return new Block(BlockID.Air);
                // Determine what chunk holds this block.
                int chunkX = x / Chunk.SizeX;
                int chunkY = y / Chunk.SizeY;
                int chunkZ = z / Chunk.SizeZ;

                // Check bounds
                if (chunkX < 0 || chunkX >= Chunks.GetLength(0)
                    || chunkY < 0 || chunkY >= Chunks.GetLength(1)
                    || chunkZ < 0 || chunkZ >= Chunks.GetLength(2))
                    return BlockID.None; // Chunk outside of bounds.

                Chunk chunk = Chunks[chunkX, chunkY, chunkZ];
                // This figures out the coordinate of the block relative to chunk.
                int levelX = x % Chunk.SizeX;
                int levelY = y % Chunk.SizeY;
                int levelZ = z % Chunk.SizeZ;
                return chunk[levelX, levelY, levelZ];
            }
            set
            {
                if (!InBounds(x, y, z)) // Not within world bounds.
                    return;
                // first calculate which chunk we are talking about:
                int chunkX = (x / Chunk.SizeX);
                int chunkY = (y / Chunk.SizeY);
                int chunkZ = (z / Chunk.SizeZ);

                // cannot modify chunks that are not within the visible area
                if (chunkX < 0 || chunkX > Chunks.GetLength(0))
                    throw new Exception("Cannot modify world outside visible area");
                if (chunkY < 0 || chunkY > Chunks.GetLength(1))
                    throw new Exception("Cannot modify world outside visible area");
                if (chunkZ < 0 || chunkZ > Chunks.GetLength(2))
                    throw new Exception("Cannot modify world outside visible area");
                Chunk chunk = Chunks[chunkX, chunkY, chunkZ];

                // this figures out the coordinate of the block relative to
                // chunk origin.
                int lx = x % Chunk.SizeX;
                int ly = y % Chunk.SizeY;
                int lz = z % Chunk.SizeZ;

                chunk[lx, ly, lz] = value;
            }
        }
        public World(int length, int width, int height)
        {
            Length = length;
            Width = width;
            Height = height;

            Chunks = new Chunk[Length / Chunk.SizeX, Width / Chunk.SizeY, Height / Chunk.SizeZ];
            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int y = 0; y < Chunks.GetLength(1); y++)
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        Chunks[x, y, z] = new Chunk(this, new Vector3(x * Chunk.SizeX, y * Chunk.SizeY, z * Chunk.SizeZ));
                        //Chunks[x, y, z].UpdateNeeded = true;
                    }
            Client.OnUpdate += Update;
            Client.OnDraw3D += Draw;
        }
        public Vector3I Spawn
        {
            get { return new Vector3I(Length / 2, Width / 2, Height); }
        }
        public void Update(object sender, UpdateEventArgs e)
        {
            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int y = 0; y < Chunks.GetLength(1); y++)
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        Chunks[x, y, z].Update();
                    }
        }
        public bool InBounds(Vector3I pos)
        {
            return InBounds(pos.X, pos.Y, pos.Z);
        }
        public bool InBounds(int x, int y, int z)
        {
            bool checkX = x < 0 || x >= Length;
            bool checkY = y < 0 || y >= Width;
            bool checkZ = z < 0 || z >= Height;
            return !(checkX || checkY || checkZ);
        }
    }
}
