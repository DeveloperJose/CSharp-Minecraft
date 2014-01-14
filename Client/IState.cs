using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public interface IState
    {
        void Init();
        void Stop();
    }
    public sealed class MainState : IState
    {
        public void Init()
        {
            Client.OnDraw2D += Client_OnDraw2D;
            Client.OnUpdate += Client_OnUpdate;
        }
        public void Stop()
        {
            Client.OnDraw2D -= Client_OnDraw2D;
            Client.OnUpdate -= Client_OnUpdate;
        }
        private static readonly Vector2 Down = new Vector2(0, 18);
        private void Client_OnDraw2D(object sender, Draw2DEventArgs e)
        {
            e.SpriteBatch.Begin();

            e.SpriteBatch.Draw(Client.EmptyTexture,
                    new Rectangle(0, 0, Client.Viewport.Width, Client.Viewport.Height),
                    Color.CornflowerBlue);

            AnchoredText t = new AnchoredText(Client.Font, "Press Enter to Start",
                new Vector2(Client.Viewport.Width / 2, 36), TextAnchor.MiddleCenter);
            t.Draw(e.SpriteBatch);

            string[] lines = new string[]{
                "WASD - Movement            Q - Up",  
                "R    - Respawn             E - Down",
                "F    - Render Distance     Space - Jump"
            };
            for (int i = 0; i < lines.Length; i++)
            {
                new AnchoredText(Client.Font, lines[i].PadRight(40),
                    Client.WindowCenter + (Down * i),
                    TextAnchor.MiddleCenter
                    ).Draw(e.SpriteBatch);
            }

            e.SpriteBatch.End();
        }
        private void Client_OnUpdate(object sender, UpdateEventArgs e)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                Client.CurrentState = new PlayingState();
            }
        }
    }
    public sealed class PlayingState : IState
    {
        private static bool Loaded;

        public void Init()
        {
            // TODO: Allow external plugins.
            PluginManager.Init();

            Client.OnDraw2D += Client_OnDraw2D;

            // Temp. Level generation.
            Client.MainWorld = new World(32, 32, 32);
            for (int x = 0; x < Client.MainWorld.Length; x++)
                for (int y = 0; y < Client.MainWorld.Width; y++)
                    for (int z = 0; z < Client.MainWorld.Height; z++)
                    {
                        if (x == 17 && y == 16 && z == 5)
                            Client.MainWorld[x, y, z] = BlockID.Bricks;
                        else if (x == 16 && y == 16 && z == 5)
                            Client.MainWorld[x, y, z] = BlockID.Leaves;
                        else if (z == 0)
                            Client.MainWorld[x, y, z] = BlockID.Admincrete;
                        else if (z == 1)
                            Client.MainWorld[x, y, z] = BlockID.Stone;
                        else if (z == 2)
                            Client.MainWorld[x, y, z] = BlockID.Dirt;
                        else if (z == 3)
                            Client.MainWorld[x, y, z] = BlockID.Grass;
                        else
                            Client.MainWorld[x, y, z] = BlockID.Air;

                    }
            Client.MainPlayer = new Player();

            Loaded = true;
            Client.Paused = false;
        }

        void Client_OnDraw2D(object sender, Draw2DEventArgs e)
        {
            if (!Loaded)
            {
                e.SpriteBatch.Begin();
                e.SpriteBatch.Draw(Client.EmptyTexture,
                    new Rectangle(0, 0, Client.Viewport.Width, Client.Viewport.Height),
                    new Color(Color.CornflowerBlue, 75));

                e.SpriteBatch.DrawString(Client.Font,
                    "Loading world...",
                    Client.WindowCenter - (Client.Font.MeasureString("Loading world...") / 2),
                    Color.White);
                e.SpriteBatch.End();
            }
            else if (Client.Paused)
            {
                e.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

                // Draw rectangle over screen
                e.SpriteBatch.Draw(Client.EmptyTexture,
                    new Rectangle(0, 0, Client.Viewport.Width, Client.Viewport.Height),
                    new Color(Color.Blue, 25)); // 50% transparency 

                e.SpriteBatch.End();
            }
            else
            {

            }
        }
        public void Stop()
        {

        }

    }
}
