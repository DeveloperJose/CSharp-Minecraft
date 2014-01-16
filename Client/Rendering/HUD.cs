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
        void Raytrace(SpriteBatch s, Vector3 initial, Vector3 final)
        {
            double dx = Math.Abs(final.X - initial.X);
            double dy = Math.Abs(final.Y - initial.Y);
            double dz = Math.Abs(final.Z - initial.Z);

            int x = (int)(Math.Floor(initial.X));
            int y = (int)(Math.Floor(initial.Y));
            int z = (int)(Math.Floor(initial.Z));

            double dt_dx = 1.0 / dx;
            double dt_dy = 1.0 / dy;
            double dt_dz = 1.0 / dz;

            double t = 0;

            int n = 1;
            int x_inc, y_inc, z_inc;
            double t_next_vertical, t_next_horizontal, t_next_z;

            if (dx == 0)
            {
                x_inc = 0;
                t_next_horizontal = dt_dx; // infinity
            }
            else if (final.X > initial.X)
            {
                x_inc = 1;
                n += (int)(Math.Floor(final.X)) - x;
                t_next_horizontal = (Math.Floor(initial.X) + 1 - initial.X) * dt_dx;
            }
            else
            {
                x_inc = -1;
                n += x - (int)(Math.Floor(final.X));
                t_next_horizontal = (initial.X - Math.Floor(initial.X)) * dt_dx;
            }

            if (dy == 0)
            {
                y_inc = 0;
                t_next_vertical = dt_dy; // infinity
            }
            else if (final.Y > initial.Y)
            {
                y_inc = 1;
                n += (int)(Math.Floor(final.Y)) - y;
                t_next_vertical = (Math.Floor(initial.Y) + 1 - initial.Y) * dt_dy;
            }
            else
            {
                y_inc = -1;
                n += y - (int)(Math.Floor(final.Y));
                t_next_vertical = (initial.Y - Math.Floor(initial.Y)) * dt_dy;
            }

            if (dz == 0)
            {
                z_inc = 0;
                t_next_z = dt_dz; // infinity
            }
            else if (final.Z > initial.Z)
            {
                z_inc = 1;
                n += (int)(Math.Floor(final.Z)) - z;
                t_next_z = (Math.Floor(initial.Z) + 1 - initial.Z) * dt_dz;
            }
            else
            {
                z_inc = -1;
                n += z - (int)(Math.Floor(final.Z));
                t_next_z = (initial.Z - Math.Floor(initial.Z)) * dt_dz;
            }
            for (; n > 0; --n)
            {
                Vector3 renderPos = new Vector3(x, y, z);
                Vector3 min = new Vector3(renderPos.X - 0.5f, renderPos.Y - 0.5f, renderPos.Z - 0.5f);
                Vector3 max = new Vector3(renderPos.X + 0.5f, renderPos.Y + 0.5f, renderPos.Z + 0.5f);
                BlockID id = Client.MainWorld[renderPos.ToBlockCoords()];
                if (id != BlockID.None && y > 2)
                {
#if DEBUG
                    BoundingBoxRenderer.Render(new BoundingBox(min, max),
                        Client.Device,
                        Client.MainPlayer.Camera.ViewMatrix,
                        Client.MainPlayer.Camera.ProjectionMatrix,
                        Color.White);
#endif
                    if (id != BlockID.Air)
                    {
                        s.DrawString(Client.Font, "Looking at: " + id, new Vector2(0, 96), Color.White);
#if DEBUG
                        BoundingBoxRenderer.Render(new BoundingBox(min, max),
                            Client.Device,
                            Client.MainPlayer.Camera.ViewMatrix,
                            Client.MainPlayer.Camera.ProjectionMatrix,
                            Color.Red);
                        return;
#endif
                    }
                }

                if (t_next_vertical <= t_next_horizontal && t_next_vertical <= t_next_z)
                {
                    y += y_inc;
                    t = t_next_vertical;
                    t_next_vertical += dt_dy;
                }
                else if (t_next_horizontal <= t_next_vertical && t_next_horizontal <= t_next_z)
                {
                    x += x_inc;
                    t = t_next_horizontal;
                    t_next_horizontal += dt_dx;
                }
                else
                {
                    z += z_inc;
                    t = t_next_z;
                    t_next_z += dt_dz;
                }
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
                e.SpriteBatch.Draw(Client.EmptyTexture, Client.WindowCenter, Color.Red);
                string fps = string.Format("FPS: {0}", frameRate);
                e.SpriteBatch.DrawString(Client.Font, fps, Vector2.Zero, Color.White);

                //string posText = Client.MainPlayer.Camera.Position.ToBlockCoords();
                //e.SpriteBatch.DrawString(Client.Font, posText, new Vector2(0, 14), Color.White);

                //Vector3I underPos = pos; //- Client.MainPlayer.Head;
                //underPos = new Vector3I(underPos.X, underPos.Y, underPos.Z - 1);
                //BlockID b = Client.MainWorld[underPos];
                //string blockUnder = string.Format("Under: {0}", b);
                //e.SpriteBatch.DrawString(Client.Font, blockUnder, new Vector2(0, 28), Color.White);

                MouseState m = Mouse.GetState();
                int mouseX = m.X;
                int mouseY = m.Y;
                Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
                Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

                Matrix world = Matrix.CreateTranslation(0, 0, 0);

                Vector3 nearPoint = Client.Viewport.Unproject(nearsource,
                    Client.MainPlayer.Camera.ProjectionMatrix, Client.MainPlayer.Camera.ViewMatrix, Matrix.Identity);

                Vector3 farPoint = Client.Viewport.Unproject(farsource,
                    Client.MainPlayer.Camera.ProjectionMatrix, Client.MainPlayer.Camera.ViewMatrix, Matrix.Identity);
                // Create a ray from the near clip plane to the far clip plane.
                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();

                // Leaves are at 16,16,5
                e.SpriteBatch.DrawString(Client.Font, "Pos: " + Client.MainPlayer.Camera.Position.ToBlockCoords(), new Vector2(0, 14), Color.White);

                e.SpriteBatch.DrawString(Client.Font, "Target: " +
                    (Client.MainPlayer.Camera.Target).ToBlockCoords(), new Vector2(0, 56), Color.White);

                e.SpriteBatch.DrawString(Client.Font, "Rotation: " + Client.MainPlayer.Camera.Rotation, new Vector2(0, 70), Color.White);
                //farPoint.Normalize();
                //farPoint *= Client.MainPlayer.;
                Vector3 distance = new Vector3I(1, 5, 1).ToRenderCoords();
                Matrix rotationMatrix = Matrix.CreateRotationX(Client.MainPlayer.Camera.Rotation.X) *
                                        Matrix.CreateRotationY(Client.MainPlayer.Camera.Rotation.Y);
                distance = Vector3.Transform(distance, rotationMatrix);
                distance += nearPoint;

                Raytrace(e.SpriteBatch, nearPoint.Center(), distance.Center());
                //Raytrace(e.SpriteBatch, frontOfLeaf.Center(), leaf.Center());
                //Raytrace(e.SpriteBatch, frontFlower.Center(), flower.Center());
                //Vector3 frontOfLeaf = (new Vector3I(16, 11, 5)).ToRenderCoords();
                //Vector3 leaf = new Vector3I(16, 16, 5).ToRenderCoords();
                //Vector3 frontFlower = new Vector3I(15,11,5).ToRenderCoords();
                //Vector3 flower = new Vector3I(15, 16, 5).ToRenderCoords();
            }
            else
            {
                AnchoredText t = new AnchoredText(Client.Font, "PAUSED: " + Client.Version, Client.WindowCenter, TextAnchor.MiddleCenter);
                t.Draw(e.SpriteBatch);
            }
            e.SpriteBatch.End();
        }
    }
}
