using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Client
{
    public class Mob
    {
        public Texture2D Texture { get; private set; }
        public Vector3I Position { get; private set; }
        public Mob(Texture2D texture, Vector3I pos)
        {
            Client.OnDraw3D += OnDraw3D;
            Client.OnDraw2D += new EventHandler<Draw2DEventArgs>(Client_OnDraw2D);

            Texture = texture;
            Position = pos;
        }

        void Client_OnDraw2D(object sender, Draw2DEventArgs e)
        {
            e.SpriteBatch.Begin();

            //e.SpriteBatch.Draw(Texture, new Rectangle(0, 0, 50, 50), Color.Red);

            e.SpriteBatch.End();
        }

        private void OnDraw3D(object sender, Draw3DEventArgs e)
        {
            
        }
    }
}
