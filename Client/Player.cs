using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HexaClassicClient.Rendering;
namespace HexaClassicClient
{
    public sealed class Player
    {
        public Player()
        {
            HexaClassicClient.OnUpdate += Update;
            Camera = new FirstPersonCamera(Vector3.Zero, Vector3.Zero);
        }
        public FirstPersonCamera Camera { get; set; }
        public void Update(object sender, UpdateEventArgs e)
        {
            Camera.Update(e.GameTime);
        }
    }
}
