#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public sealed partial class Client : Game
    {
        public static GraphicsDevice ClientGraphics { get; set; }
        public static readonly string Version = "v0.1.0";
        public static bool InputAllowed { get; set; }
        private GraphicsDeviceManager Graphics { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        /// <summary>
        /// Dummy texture used to draw things without needing an actually texture.
        /// </summary>
        public static Texture2D EmptyTexture { get; private set; }
        /// <summary>
        /// The Main world.
        /// </summary>
        public static World MainWorld { get; private set; }
        /// <summary>
        /// This is us! The player.
        /// </summary>
        public static Player MainPlayer { get; private set; }
        /// <summary>
        /// The texture atlas for all blocks.
        /// </summary>
        public static Texture2D Terrain { get; private set; }
        /// <summary>
        /// Our main debugging font!
        /// </summary>
        public static SpriteFont Font { get; private set; }
        /// <summary>
        /// The viewport for the current game.
        /// </summary>
        public static Viewport Viewport { get; private set; }
        /// <summary>
        /// The center of the viewport
        /// </summary>
        public static Vector2 WindowCenter
        {
            get
            {
                return new Vector2(Viewport.Bounds.Width / 2, Viewport.Bounds.Height / 2);
            }
        }
        
        /// <summary>
        /// The window for the current game.
        /// </summary>
        public static GameWindow GameWindow { get; private set; }
        /// <summary>
        /// The default character texture
        /// </summary>
        public static Texture2D Char { get; private set; }
        public static SoundEffect Calm1 { get; private set; }
        private SoundEffectInstance Calm1Instance { get; set; }
        private AudioEmitter AudioEmitter { get; set; }
        private AudioListener AudioListener { get; set; }
        private Vector3 ObjectPos { get; set; }
        public Client()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Viewport = GraphicsDevice.Viewport;
            GameWindow = Window;
            IsMouseVisible = false;
            InputAllowed = true;
            Window.Title = "[HexaClassic]HC Client by Gamemakergm - " + Version;
            ClientGraphics = GraphicsDevice;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Allow external plugins.
            PluginManager.Init();
            EmptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            EmptyTexture.SetData<Color>(new Color[] { Color.White });

            // Temp. Level generation.
            MainWorld = new World(32, 32, 32);
            for (int x = 0; x < MainWorld.Length; x++)
                for (int y = 0; y < MainWorld.Width; y++)
                    for (int z = 0; z < MainWorld.Height; z++)
                    {
                        if (x == 17 && y == 16 && z == 5)
                            MainWorld[x, y, z] = BlockID.Bricks;
                        else if (x == 16 && y == 16 && z == 5)
                            MainWorld[x, y, z] = BlockID.Leaves;
                        else if (z == 0)
                            MainWorld[x, y, z] = BlockID.Admincrete;
                        else if (z == 1)
                            MainWorld[x, y, z] = BlockID.Stone;
                        else if (z == 2)
                            MainWorld[x, y, z] = BlockID.Dirt;
                        else if (z == 3)
                            MainWorld[x, y, z] = BlockID.Grass;
                        else
                            MainWorld[x, y, z] = BlockID.Air;

                    }
            MainPlayer = new Player();
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Terrain = Content.Load<Texture2D>("terrain");
            Font = Content.Load<SpriteFont>("MainFont");
            Char = Content.Load<Texture2D>("char");
            RaiseLoadEvent(Content);

            Mob player = new Mob(Char, Vector3I.Zero);

            Calm1 = Content.Load<SoundEffect>("calm1");
            Calm1Instance = Calm1.CreateInstance();
            AudioEmitter = new AudioEmitter();
            AudioListener = new AudioListener();
            ObjectPos = Vector3.Zero;

            Calm1Instance.Apply3D(AudioListener, AudioEmitter);
            Calm1Instance.IsLooped = true;

            Calm1Instance.Volume = 1f; // Full volume.
            Calm1Instance.Play();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            RaiseUnloadEvent(Content);
            Content.Unload();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/

            RaiseUpdateEvent(gameTime);

            // Move the sound in a circle
            ObjectPos = new Vector3(
                (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) / 2,
                0,
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
            //ObjectPos = MainWorld.Spawn;
            AudioEmitter.Position = ObjectPos;
            AudioListener.Position = ObjectPos;
            Calm1Instance.Apply3D(AudioListener, AudioEmitter);

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //// Enables transparency of blocks
            GraphicsDevice.DepthStencilState.DepthBufferWriteEnable = true;
            GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            if (DebugSettings.RenderWireframe)
            {
                GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            }
            // Fixes seams between blocks.
            SamplerState s = new SamplerState()
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                Filter = TextureFilter.Point,

            };
            
            GraphicsDevice.SamplerStates[0] = s;

            RaiseDraw3DEvent(gameTime, GraphicsDevice);
            RaiseDraw2DEvent(gameTime, SpriteBatch); // Draw 2D components over 3D.
            if (!InputAllowed) // Paused
            {
                SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

                // Draw rectangle over screen
                SpriteBatch.Draw(EmptyTexture,
                    new Rectangle(0, 0, Viewport.Width, Viewport.Height),
                    new Color(Color.Blue, 25)); // 50% transparency 


                SpriteBatch.End();
                this.IsMouseVisible = true;
            }
            else
            {
                this.IsMouseVisible = false;
            }
            base.Draw(gameTime);
        }
    }
}
