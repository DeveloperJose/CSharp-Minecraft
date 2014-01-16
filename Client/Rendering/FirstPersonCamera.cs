using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Client.Rendering
{
    /// <summary>
    /// The main player's camera
    /// </summary>
    public sealed class FirstPersonCamera : Camera
    {
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
        #endregion

        /// <summary>
        /// Are we jumping right now?
        /// </summary>
        public bool IsJumping { get; set; }
   
        /// <summary>
        /// The furthest the camera can see
        /// </summary>
        public float FarDistance { get; private set; }
        public float[] Distances = new float[] { 10f, 15f, 30f, 50f, 100f };
        private int DistanceIndex = 0;

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
        public FirstPersonCamera()
        {
            Position = Client.MainWorld.Spawn.ToRenderCoords();
            Rotation = Vector3.Zero;

            FarDistance = 100f;

            //Setup the projection matrix
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Client.Viewport.AspectRatio,
                NearDistance,
                FarDistance);

            PrevMouseState = Mouse.GetState();
            PrevKeyboardState = Keyboard.GetState();
        }


        private readonly float Gravity = -13f;
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
                Client.Paused = !Client.Paused;

            if (!Client.Paused)
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
                    MoveTo(Client.MainWorld.Spawn.ToRenderCoords(), Vector3.Zero);
                if (CurrentKeyboardState.IsKeyDown(Keys.Q))
                    moveVector.Y = 1;
                if (CurrentKeyboardState.IsKeyDown(Keys.E))
                    moveVector.Y = -1;
                if (CurrentKeyboardState.IsKeyDown(Keys.Space) && !IsJumping)
                {
                    moveVector.Y = 12;
                    IsJumping = true;
                }
                if (CurrentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    Client.MainWorld[Vector3I.Zero] = Client.MainWorld[Vector3I.Zero];
                }
                if (CurrentKeyboardState.IsKeyDown(Keys.NumPad3))
                {
                    PluginManager.Reload();
                }
                if (CurrentKeyboardState.IsKeyUp(Keys.F) && PrevKeyboardState.IsKeyDown(Keys.F))
                {
                    DistanceIndex++;
                    if (DistanceIndex >= Distances.Length)
                        DistanceIndex = 0;

                    FarDistance = Distances[DistanceIndex];
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.PiOver4,
                        Client.Viewport.AspectRatio,
                        NearDistance,
                        FarDistance);
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
                Move(moveVector);
                //Vector3 testVector = moveVector;
                //testVector.Normalize();
                //testVector = testVector * (moveVector.Length() + 0.3f);

                //Vector3I testPos = PreviewMove(testVector).ToBlockCoords();
                //Vector3I bodyPos = PreviewMove(testVector - new Vector3(0, Player.Height, 0)).ToBlockCoords();

                //if (!Client.MainWorld[testPos].Solid() || !Client.MainWorld[bodyPos].Solid())
                //{
                //    Vector3I newLoc = PreviewMove(moveVector).ToBlockCoords();
                //    if (Client.MainWorld.InBounds(newLoc))
                //    {
                //        if (!Client.MainWorld[newLoc].Solid())
                //            Move(moveVector); // Now we move the camera using that movement vector
                //    }
                //}

            }

            // Now try applying gravity
            Vector3 gravityVector = Vector3.Zero;
            gravityVector.Y += Gravity;

            gravityVector *= dt;

            // Add the player's eye level.
            Vector3 vectorWithFeet = new Vector3(gravityVector.X, gravityVector.Y - Player.EyeLevel, gravityVector.Z);
            Vector3 gravLoc = PreviewMove(vectorWithFeet);
            Vector3I worldLoc = gravLoc.ToBlockCoords();

            if (Client.MainWorld.InBounds(worldLoc))
            {
                if (Client.MainWorld[worldLoc].Solid())
                {
                    if (IsJumping)
                        IsJumping = false;
                }
                else
                {
                    Move(gravityVector);
                }
            }
            if (!Client.Paused)
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
