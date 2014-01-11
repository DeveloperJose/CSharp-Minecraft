using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Client.Rendering
{
    public sealed class FirstPersonCamera
    {
        public static int X = 0;
        public static int Y = 0;

        public const float MouseSpeed = 0.5f;
        public const float MovementSpeed = 15f;
        public const float NearDistance = 0.05f;
        public const float FarDistance = 100f;

        public bool IsJumping { get; set; }

        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                // Must update.
                UpdateLookAt();
            }
        }
        private Vector3 _rotation;
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateLookAt();
            }
        }
        private Vector3 Target { get; set; }

        private Vector3 MouseRotationBuffer;
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }

        public Matrix ProjectionMatrix { get; set; }

        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(Position, Target, Vector3.Up);
            }
        }

        public FirstPersonCamera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;

            //Setup the projection matrix
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Client.Viewport.AspectRatio, NearDistance, FarDistance);

            PrevMouseState = Mouse.GetState();
            IsJumping = false;
        }

        //Updates the camera's lookAt vector
        private void UpdateLookAt()
        {
            //Calculate a rotation matrix from our camera's rotation, used
            //to orient our look at vector
            Matrix rotationMatrix = Matrix.CreateRotationX(Rotation.X) *
                                  Matrix.CreateRotationY(Rotation.Y);
            //Create the look at offset vector based on the direction our camera is
            //originally looking and our rotation matrix
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            //Finally, build the camera's look at vector by adding
            //our current position and the look at vector offset.
            Target = Position + lookAtOffset;
        }
        private KeyboardState State;
        public void Update(GameTime gameTime)
        {
            //Delta time
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CurrentMouseState = Mouse.GetState();

            Vector3 moveVector = Vector3.Zero;

            KeyboardState newState = Keyboard.GetState();
            if (State == null)
                State = newState;

            if (newState.IsKeyDown(Keys.W))
                moveVector.Z = 1; //cameraSpeed * dt;
            if (newState.IsKeyDown(Keys.S))
                moveVector.Z = -1; //cameraSpeed * dt;
            if (newState.IsKeyDown(Keys.A))
                moveVector.X = 1;// cameraSpeed* dt;
            if (newState.IsKeyDown(Keys.D))
                moveVector.X = -1;//cameraSpeed * dt;
            if (newState.IsKeyDown(Keys.R))
                MoveTo(Client.MainWorld.Spawn, Vector3.Zero);
            if (newState.IsKeyDown(Keys.Q))
                moveVector.Y = 1;
            if (newState.IsKeyDown(Keys.E))
                moveVector.Y = -1;
            if (newState.IsKeyUp(Keys.NumPad2) && State.IsKeyDown(Keys.NumPad2))
                X++;
            if (newState.IsKeyUp(Keys.NumPad5) && State.IsKeyDown(Keys.NumPad5))
                Y++;
            if (newState.IsKeyUp(Keys.NumPad1) && State.IsKeyDown(Keys.NumPad1))
                X--;
            if (newState.IsKeyUp(Keys.NumPad4) && State.IsKeyDown(Keys.NumPad4))
                Y--;
            if (newState.IsKeyDown(Keys.Space) && !IsJumping)
            {
                moveVector.Y += 7;
                IsJumping = true;
            }
            Client.MainWorld[Vector3I.Zero] = Client.MainWorld[Vector3I.Zero];
            //if (newState.IsKeyDown(Keys.Space) && !IsJumping){
            //    moveVector.Y = 5;
            //    IsJumping = true;
            //}
            //Now if our movement vector is not zero

            if (moveVector != Vector3.Zero)
            {
                //We must normalize the vector (make it of unit length (1))
                moveVector.Normalize();
                //Now add in our camera speed and delta time
                moveVector *= MovementSpeed * dt;

                //This is for checking movement parameters
                Vector3 newLoc = PreviewMove(moveVector);
                Vector3I location = new Vector3I(newLoc) / 2;

                if (Client.MainWorld.InBounds(location))
                {
                    //Now we move the camera using that movement vector
                    Move(moveVector);
                }
            }
            
            //Vector3 gravityVector = new Vector3(0, -0.5f, 0);
            
            //Vector3I check = new Vector3I(PreviewMove(gravityVector));
            //if (Client.MainWorld.InBounds(check))
            //{
            //    Vector3I blockPos = (check / 2) - new Vector3I(0, 0, 1);
            //    if (!Client.MainWorld[blockPos].Solid)
            //    {
            //        Move(gravityVector);
            //    }
            //    else
            //    {
            //        if (IsJumping)
            //            IsJumping = false;
            //    }
            //}

            //Change in mouse position
            //x and y
            float deltaX;
            float deltaY;

            //Handle mouse movement
            if (CurrentMouseState != PrevMouseState)
            {
                //Get the change in mouse position
                deltaX = Mouse.GetState().X - (Client.Viewport.Width / 2);
                deltaY = Mouse.GetState().Y - (Client.Viewport.Height / 2);

                //This is used to buffer against use input.
                MouseRotationBuffer.X -= MouseSpeed * deltaX * dt;
                MouseRotationBuffer.Y -= MouseSpeed * deltaY * dt;
                if (MouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    MouseRotationBuffer.Y = MouseRotationBuffer.Y - (MouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (MouseRotationBuffer.Y > MathHelper.ToRadians(90.0f))
                    MouseRotationBuffer.Y = MouseRotationBuffer.Y - (MouseRotationBuffer.Y - MathHelper.ToRadians(90.0f));


                Rotation = new Vector3(-MathHelper.Clamp(MouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f),
                    MathHelper.ToRadians(90.0f)), MathHelper.WrapAngle(MouseRotationBuffer.X), 0);

                deltaX = 0;
                deltaY = 0;
            }

            Mouse.SetPosition(Client.GameWindow.ClientBounds.Width / 2, Client.GameWindow.ClientBounds.Height / 2);

            PrevMouseState = CurrentMouseState;
            State = newState;
        }

        //Sets the camera's position and rotation
        public void MoveTo(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
        //Used to move simulate camera movement
        //without actually moving the camera
        //Good for checking collision before allowing player to move
        public Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotate = Matrix.CreateRotationY(Rotation.Y);
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);
            return Position + movement;
        }

        //Actually moves the camera by the scale factor passed in
        public void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

    }
}
