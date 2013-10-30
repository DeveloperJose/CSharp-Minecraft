using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace JClient
{
    public sealed class HUD : IListener
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public void Init()
        {
            JClient.OnUpdate += Update;
            JClient.OnDraw2D += Draw2D;
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

            string fps = string.Format("FPS: {0}", frameRate);
            e.SpriteBatch.DrawString(JClient.Font, fps, Vector2.Zero, Color.White);

            //string pos = string.Format("X:{0},Y:{1},Z:{2}", JClient.MainPlayer.Camera.Position.X, JClient.MainPlayer.Camera.Position.Y, JClient.MainPlayer.Camera.Position.Z);
            //DrawString(e.SpriteBatch, pos, 1);

            //Vector3I blockPos = new Vector3I((int)JClient.MainPlayer.Camera.Position.X - 16, (int)(JClient.MainPlayer.Camera.Position.Z - 16), (int)JClient.MainPlayer.Camera.Position.Y - 16);
            //Vector3 realPos = JClient.MainPlayer.Camera.Position;
            //Vector3 cubePos = new Vector3(realPos.X, realPos.Y, realPos.Z);
            //Vector3I finalPos = new Vector3I((int)cubePos.X, (int)cubePos.Z, (int)cubePos.Y + 5);
            //Block b = JClient.MainWorld[finalPos];
            //string blockUnder = string.Format("Under: ({0},{1},{2}) - {3}", finalPos.X, finalPos.Y, finalPos.Z, (BlockID)b.ID);
            //DrawString(e.SpriteBatch, blockUnder, 2);
            e.SpriteBatch.End();
        }
    }
}
