#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace HexaClassicClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public sealed partial class HexaClassicClient : Game
    {
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
        /// The window for the current game.
        /// </summary>
        public static GameWindow GameWindow { get; private set; }
        public HexaClassicClient()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Viewport = GraphicsDevice.Viewport;
            GameWindow = Window;
            IsMouseVisible = false;
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
            MainPlayer = new Player();

            // Temp. Level generation.
            MainWorld = new World(32, 32, 32);
            for (int x = 0; x < MainWorld.Length; x++)
                for (int y = 0; y < MainWorld.Width; y++)
                    for (int z = 0; z < MainWorld.Height; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            MainWorld[x, y, z] = new Block(BlockID.Bricks);
                        else if (x == 16 && y == 16 && z == 5)
                            MainWorld[x, y, z] = new Block(BlockID.Bricks);
                        else if (z == 0)
                            MainWorld[x, y, z] = new Block(BlockID.Admincrete);
                        else if (z == 1)
                            MainWorld[x, y, z] = new Block(BlockID.Stone);
                        else if (z == 2)
                            MainWorld[x, y, z] = new Block(BlockID.Dirt);
                        else if (z == 3)
                            MainWorld[x, y, z] = new Block(BlockID.Grass);
                        else
                            MainWorld[x, y, z] = new Block(BlockID.Air);

                    }
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

            RaiseLoadEvent(Content);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            RaiseUpdateEvent(gameTime);
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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
            base.Draw(gameTime);
        }
    }
}
