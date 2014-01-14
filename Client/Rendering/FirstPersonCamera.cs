using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Client.Rendering
{
    /// <summary>
    /// The main player's camera
    /// </summary>
    public sealed class FirstPersonCamera
    {
        #region Debug
        /// <summary>
        /// Debug feature: Controls the texture atlas's X position for Bricks.
        /// That lets me see how each blocks looks like by clicking a button
        /// </summary>
        public static int X = 7;
        /// <summary>
        /// Debug feature: Controls the texture atlas's Y position for Bricks.
        /// That lets me see how each blocks looks like by clicking a button
        /// </summary>
        public static int Y = 0;
        #endregion

        #region Constants
        /// <summary>
        /// The mouse movement speed
        /// </summary>
        public const float MouseSpeed = 0.5f;
        /// <summary>
        /// The camera's movement speed
        /// </summary>
        public const float MovementSpeed = 15f;
        /// <summary>
        /// The nearest distance the camera will use
        /// </summary>
        public const float NearDistance = 0.05f;
        /// <summary>
        /// The furthest the camera can see
        /// </summary>
        public const float FarDistance = 100f;
        #endregion

        #region Player Variables
        /// <summary>
        /// Are we jumping right now?
        /// </summary>
        public bool IsJumping { get; set; }
        #endregion

        #region Camera Variables
        /// <summary>
        /// Our current position.
        /// </summary>
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
        private Vector3 _position;

        /// <summary>
        /// Our current rotation
        /// </summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateLookAt();
            }
        }
        private Vector3 _rotation;

        public Matrix ProjectionMatrix { get; set; }
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(Position, Target, Vector3.Up);
            }
        }
        private Vector3 Target { get; set; }
        #endregion

        #region Mouse
        private Vector3 MouseRotationBuffer;

        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }
        #endregion

        #region Keyboard
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        #endregion

        /// <summary>
        /// Creates a new First person view camera.
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="rotation">Starting rotation</param>
        public FirstPersonCamera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;

            //Setup the projection matrix
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                Client.Viewport.AspectRatio, 
                NearDistance, 
                FarDistance);

            PrevMouseState = Mouse.GetState();
            PrevKeyboardState = Keyboard.GetState();
            IsJumping = false;
        }

        /// <summary>
        /// Updates the camera's looking vector.
        /// </summary>
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
        private readonly float Gravity = -5f;
        private readonly float Height = 0.62f;
        /// <summary>
        /// Update the camera's physics
        /// </summary>
        /// <param name="gameTime">GameTime from the main game.</param>
        public void Update(GameTime gameTime)
        {
            //Delta time
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CurrentMouseState = Mouse.GetState();
            CurrentKeyboardState = Keyboard.GetState();

            Vector3 moveVector = Vector3.Zero;

            if (CurrentKeyboardState.IsKeyUp(Keys.Escape) && PrevKeyboardState.IsKeyDown(Keys.Escape))
                Client.InputAllowed = !Client.InputAllowed;

            if (Client.InputAllowed)
            {
                if (CurrentKeyboardState.IsKeyDown(Keys.W))
                    moveVector.Z = 1; //cameraSpeed * dt;
                if (CurrentKeyboardState.IsKeyDown(Keys.S))
                    moveVector.Z = -1; //cameraSpeed * dt;
                if (CurrentKeyboardState.IsKeyDown(Keys.A))
                    moveVector.X = 1;// cameraSpeed* dt;
                if (CurrentKeyboardState.IsKeyDown(Keys.D))
                    moveVector.X = -1;//cameraSpeed * dt;
                if (CurrentKeyboardState.IsKeyDown(Keys.R))
                    MoveTo(Client.MainWorld.Spawn, Vector3.Zero);
                if (CurrentKeyboardState.IsKeyDown(Keys.Q))
                    moveVector.Y = 1;
                if (CurrentKeyboardState.IsKeyDown(Keys.E))
                    moveVector.Y = -1;
                if (CurrentKeyboardState.IsKeyUp(Keys.NumPad2) && PrevKeyboardState.IsKeyDown(Keys.NumPad2))
                    X++;
                if (CurrentKeyboardState.IsKeyUp(Keys.NumPad5) && PrevKeyboardState.IsKeyDown(Keys.NumPad5))
                    Y++;
                if (CurrentKeyboardState.IsKeyUp(Keys.NumPad1) && PrevKeyboardState.IsKeyDown(Keys.NumPad1))
                    X--;
                if (CurrentKeyboardState.IsKeyUp(Keys.NumPad4) && PrevKeyboardState.IsKeyDown(Keys.NumPad4))
                    Y--;
                if (CurrentKeyboardState.IsKeyDown(Keys.Space) && !IsJumping)
                {
                    moveVector.Y = 12;
                    IsJumping = true;
                }
                if (CurrentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    Client.MainWorld[Vector3I.Zero] = Client.MainWorld[Vector3I.Zero];
                }
            }

            
            // Gravity
            //UpdateGravity(gameTime);
            
            if (moveVector != Vector3.Zero) // If we moved
            {
                // We must normalize the vector (make it of unit length (1))
                //moveVector.Normalize();
                // Now add in our camera speed and delta time
                moveVector *= MovementSpeed * dt;
                //if (IsJumping)
                //{
                //    moveVector.Y += 10;
                //}
                //This is for checking movement parameters
                Vector3 newLoc = PreviewMove(moveVector);
                Vector3I location = new Vector3I(newLoc) / 2;

                if (Client.MainWorld.InBounds(location))
                    Move(moveVector); // Now we move the camera using that movement vector
            }

            // Now try applying gravity
            Vector3 gravityVector = Vector3.Zero;
            gravityVector.Y += Gravity;
            gravityVector *= dt;

            Vector3 gravLoc = PreviewMove(gravityVector);
            Vector3I worldLoc = new Vector3I(gravLoc) / 2;

            if (Client.MainWorld.InBounds(worldLoc))
            {
                if (Client.MainWorld[worldLoc].Solid)
                {
                    if (IsJumping)
                        IsJumping = false;
                }
                else
                {
                    Move(gravityVector);
                }
            }
            if (Client.InputAllowed)
            {
                //Change in mouse position
                //x and y
                float deltaX;
                float deltaY;

                //Handle mouse movement
                if (CurrentMouseState != PrevMouseState)
                {
                    //Get the change in mouse position
                    deltaX = CurrentMouseState.X - (Client.Viewport.Width / 2);
                    deltaY = CurrentMouseState.Y - (Client.Viewport.Height / 2);

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
            }
            PrevMouseState = CurrentMouseState;
            PrevKeyboardState = CurrentKeyboardState;
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
