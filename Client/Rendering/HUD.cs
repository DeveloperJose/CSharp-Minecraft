using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public sealed class HUD : IPlugin
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public void Init()
        {
            Client.OnUpdate += Update;
            Client.OnDraw2D += Draw2D;
        }
        public void Stop()
        {
            Client.OnUpdate -= Update;
            Client.OnDraw2D -= Draw2D;
        }
        public void Update(object sender, UpdateEventArgs e)
        {
            elapsedTime += e.GameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }
        public void Draw2D(object sender, Draw2DEventArgs e)
        {
            frameCounter++;

            e.SpriteBatch.Begin();

            if (!Client.Paused)
            {
                // Crosshair
                e.SpriteBatch.Draw(Client.CrosshairTexture,
                    Client.WindowCenter, // Center of screen
                    null, // Source rectangle
                    Color.White, // Color
                    0f, // Rotation
                    new Vector2(Client.CrosshairTexture.Width, Client.CrosshairTexture.Height) / 2, // Image center
                    1f, // Scale
                    SpriteEffects.None,
                    0f // Depth
                    );
                string fps = string.Format("FPS: {0}", frameRate);
                e.SpriteBatch.DrawString(Client.Font, fps, Vector2.Zero, Color.White);

                Vector3I pos = Client.MainPlayer.Camera.Position.ToBlockCoords();
                string posText = string.Format("X: {0} - Y: {1} - Z: {2}", pos.X, pos.Y, pos.Z);
                e.SpriteBatch.DrawString(Client.Font, posText, new Vector2(0, 14), Color.White);

                Vector3I underPos = pos; //- Client.MainPlayer.Head;
                underPos = new Vector3I(underPos.X, underPos.Y, underPos.Z - 1);
                BlockID b = Client.MainWorld[underPos];
                string blockUnder = string.Format("Under: {0}", b);
                e.SpriteBatch.DrawString(Client.Font, blockUnder, new Vector2(0, 28), Color.White);

                for (float x = 0.5f; x < 10f; x += 0.2f)
                {
                    Vector3 targetPoint = Client.MainPlayer.Camera.Position + (Client.MainPlayer.Camera.LookVector * x);
                    Vector3I worldPos = targetPoint.ToBlockCoords();
                    BlockID id = Client.MainWorld[worldPos];
                    if (id != BlockID.None && id != BlockID.Air && targetPoint.Z > 1)
                    {
                        e.SpriteBatch.DrawString(Client.Font, "Pointing= " + id, new Vector2(0, 42), Color.White);
                        break;
                    }
                }
                //Vector3I inFront = new Vector3I(pos) + new Vector3I(0, 1, 0);
                //Block bb = Client.MainWorld[inFront];
                //e.SpriteBatch.DrawString(Client.Font, (BlockID)b.ID + "", new Vector2(0, 42), Color.White);

                var rot = Client.MainPlayer.Camera.Rotation;
                string mouse = string.Format("Rot: X={0}, Y={1}, Z={2}", rot.X, rot.Y, rot.Z);
                e.SpriteBatch.DrawString(Client.Font, mouse, new Vector2(0, 56), Color.White);
            }
            else
            {
                //string text = "PAUSED: " + Client.Version;
                //e.SpriteBatch.DrawString(Client.Font, text,
                //    Client.WindowCenter - (Client.Font.MeasureString(text)),
                //    Color.White);
                AnchoredText t = new AnchoredText(Client.Font, "PAUSED: " + Client.Version, Client.WindowCenter, TextAnchor.MiddleCenter);
                t.Draw(e.SpriteBatch);
            }
            e.SpriteBatch.End();
        }
    }
}
