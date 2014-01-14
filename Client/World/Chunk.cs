using Microsoft.Xna.Framework;
namespace Client
{
    public sealed partial class Chunk
    {
        /// <summary>
        /// The width (X) of the chunks.
        /// This is currently the same as 32
        /// 2^5
        /// </summary>
        public const int SizeX = 1 << 5;
        /// <summary>
        /// The length (Y) of the chunks.
        /// This is currently the same as 32
        /// 2^5
        /// </summary>
        public const int SizeY = 1 << 5;
        /// <summary>
        /// The height (Z) of the chunks.
        /// This is currently the same as 32
        /// 2^5
        /// </summary>
        public const int SizeZ = 1 << 5;

        private World Parent;
        private Vector3 ChunkPosition;
        private byte[, ,] Blocks;
        public BoundingBox Box;
        public bool InBounds(int x, int y, int z)
        {
            bool checkX = x < 0 || x >= SizeX;
            bool checkY = y < 0 || y >= SizeY;
            bool checkZ = z < 0 || z >= SizeZ;
            return !(checkX || checkY || checkZ);
        }
        public BlockID this[int x, int y, int z]
        {
            get
            {
                if (!InBounds(x, y, z))
                    return BlockID.Air;
                return (BlockID)Blocks[x, y, z];
            }
            set
            {
                if (!InBounds(x, y, z))
                    Parent[x + (int)ChunkPosition.X, y + (int)ChunkPosition.Y, z + (int)ChunkPosition.Z] = value; // It's outside this chunk.
                Blocks[x, y, z] = (byte)value;
                UpdateNeeded = true;
            }
        }
        public BlockID this[Vector3I pos]
        {
            get
            {
                return this[pos.X, pos.Y, pos.Z];
            }
            set
            {
                this[pos.X, pos.Y, pos.Z] = value;
            }
        }
        private Mesh ChunkMesh;
        public Chunk(World parent, Vector3 pos)
        {
            Parent = parent;
            Blocks = new byte[SizeX, SizeY, SizeZ];
            ChunkPosition = pos;
            Box = new BoundingBox(pos, new Vector3(pos.X + SizeX, pos.Y + SizeY, pos.Z + SizeZ));
        }
        public bool Visible = true;
        public bool UpdateNeeded = true;
        public void Update()
        {
            if (UpdateNeeded)
            {
                ChunkMesh = CreateMesh();
                UpdateNeeded = false;
            }
        }
    }
}
