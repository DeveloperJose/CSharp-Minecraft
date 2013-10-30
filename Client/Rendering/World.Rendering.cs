using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace HexaClassicClient
{
    public sealed partial class World
    {
        public void Draw(object sender, Draw3DEventArgs e)
        {
            BasicEffect effect = new BasicEffect(e.GraphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = HexaClassicClient.Terrain;
            effect.CurrentTechnique.Passes[0].Apply();
            //effect.World = HexaClassicClient.MainPlayer.FCameraTwo.WorldMatrix;
            effect.World = Matrix.Identity;
            effect.Projection = HexaClassicClient.MainPlayer.Camera.Projection;
            effect.View = HexaClassicClient.MainPlayer.Camera.View;
            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int y = 0; y < Chunks.GetLength(1); y++)
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        //if (gameCamera.frustrum.Contains(Chunks[x, y, z].Box) != ContainmentType.Disjoint)
                        {
                            effect.CurrentTechnique.Passes[0].Apply();
                            Chunks[x, y, z].Draw(e.GraphicsDevice);
                        }
                    }
            effect.End();
        }
    }
}
