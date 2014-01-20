using Microsoft.Xna.Framework;
namespace Client
{
    public sealed partial class Chunk
    {
        public static readonly Vector3I Size = new Vector3I(32, 32, 32);

        private World Parent;
        private Vector3I ChunkPosition;
        private byte[, ,] Blocks;
        private Mesh ChunkMesh { get; set; }

        public BoundingBox Box;
        public bool Visible { get { return Client.MainPlayer.Camera.BoundingFrustum.Intersects(Box); } }
        public bool UpdateNeeded { get; set; }

        public bool InBounds(int x, int y, int z)
        {
            bool checkX = x < 0 || x >= Size.X;
            bool checkY = y < 0 || y >= Size.Y;
            bool checkZ = z < 0 || z >= Size.Z;
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
                    Parent[x + ChunkPosition.X, y + ChunkPosition.Y, z + ChunkPosition.Z] = value; // It's outside this chunk.
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
        public Chunk(World parent, Vector3I pos)
        {
            Parent = parent;
            Blocks = new byte[Size.X, Size.Y, Size.Z];
            ChunkPosition = pos;

            // Box drawing
            Vector3 posRender = pos.ToRenderCoords();
            Vector3 chunkSize = Size.ToRenderCoords();
            Box = new BoundingBox(posRender, posRender + chunkSize);

            ChunkMesh = new Mesh()
            {
                Vertices =
                    new[] { new Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture() }
            };
            UpdateNeeded = true;
        }

        public void Update()
        {
            if (!Visible) return;
            if (UpdateNeeded)
            {
                ChunkMesh = CreateMesh();
                UpdateNeeded = false;
            }
        }
    }
}
