using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Mesh
    {
        public VertexPositionNormalTexture[] Vertices { get; set; }
    }
}
