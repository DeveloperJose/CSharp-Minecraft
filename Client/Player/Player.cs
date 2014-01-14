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
        //public ChaseCamera Camera;
        public float Height = 0.62f;
        public FirstPersonCamera Camera;
        public Player()
        {
            Client.OnUpdate += Update;
            //Camera = new ChaseCamera(Client.MainWorld.Spawn);
            Camera = new FirstPersonCamera(Client.MainWorld.Spawn.ToRenderCoords(), Vector3.Zero);
        }
        public void Update(object sender, UpdateEventArgs e)
        {
            // Process Input
            //Camera.Update();
            Camera.Update(e.GameTime);
        }
    }
}
