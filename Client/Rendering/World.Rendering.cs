using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public sealed partial class World
    {
        public void Draw(object sender, Draw3DEventArgs e)
        {
            // Transparency
            AlphaTestEffect effect = new AlphaTestEffect(e.GraphicsDevice);
            effect.Texture = Client.TerrainTexture;
            //effect.ReferenceAlpha = 1;
            if (DebugSettings.RenderWireframe)
                effect.Texture = Client.EmptyTexture;
            
            effect.World = Matrix.Identity;
            effect.Projection = Client.MainPlayer.Camera.ProjectionMatrix;
            effect.View = Client.MainPlayer.Camera.ViewMatrix;
            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int y = 0; y < Chunks.GetLength(1); y++)
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        if(Client.MainPlayer.Camera.BoundingFrustum.Intersects(Chunks[x,y,z].Box))
                        {
                            effect.CurrentTechnique.Passes[0].Apply();
                            Chunks[x, y, z].Draw(e.GraphicsDevice);
                        }
                    }
        }
    }
}
