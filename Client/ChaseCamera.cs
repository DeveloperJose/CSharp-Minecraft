using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace Client
{
    public sealed class ChaseCamera
    {
        public Keys Left = Keys.A;
        public Keys Right = Keys.D;
        public Keys Up = Keys.W;
        public Keys Down = Keys.S;
        public Keys Jump = Keys.Space;

        public Vector3 Target;
        public Vector3 Position;

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Client.Viewport.AspectRatio, 0.1f, 100f); }
        }
        public Matrix View
        {
            get { return Matrix.CreateLookAt(Position, Target, Vector3.Up); }
        }
        public ChaseCamera(Vector3 pos)
        {
            Reset();
            Position = pos;
            Target = pos;
        }
        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            Vector3 dir = Vector3.Zero;
            if (state.IsKeyDown(Left))
                dir.X = -1;
            if (state.IsKeyDown(Right))
                dir.X = 1;
            if (state.IsKeyDown(Up))
                dir.Z = 1;
            if (state.IsKeyDown(Down))
                dir.Z = -1;
            if (state.IsKeyDown(Keys.E))
                dir.Y = 1;
            if (state.IsKeyDown(Keys.Q))
                dir.Y = -1;

            dir *= 3;
            Position += dir;
            Target = Position;
            //Target = Position;
        }
        public void Reset()
        {
            Target = Vector3.Zero;
            Position = Vector3.Zero;
        }
    }
}
