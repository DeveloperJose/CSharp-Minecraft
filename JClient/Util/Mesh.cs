using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
namespace JClient
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Mesh
    {
        public VertexPositionNormalTexture[] Vertices { get; set; }
    }
}
