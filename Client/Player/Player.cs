using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Client.Rendering;
namespace Client
{
    public sealed class Player
    {
        public static readonly float EyeLevel = (51).FixedToRenderPixels(); // Fixed-point 51
        public float ClickDistance = (160).FixedToRenderPixels(); //FixedPoint.Create(160);
        public FirstPersonCamera Camera;
        public Player()
        {
            Camera = new FirstPersonCamera();
            Client.OnUpdate += Update;
        }
        public void Update(object sender, UpdateEventArgs e)
        {
            Camera.Update(e.GameTime);
        }
    }
}
