using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace HexaClassicClient
{
    public sealed partial class HexaClassicClient
    {
        // Game events
        public static event EventHandler<ContentEventArgs> OnLoad;
        public static event EventHandler<UpdateEventArgs> OnUpdate;
        public static event EventHandler<Draw2DEventArgs> OnDraw2D;
        public static event EventHandler<Draw3DEventArgs> OnDraw3D;
        public static event EventHandler<ContentEventArgs> OnUnload;

        private void RaiseLoadEvent(ContentManager content)
        {
            var h = OnLoad;
            if (h == null) return;
            h(null, new ContentEventArgs(content));
        }
        private void RaiseUpdateEvent(GameTime gameTime)
        {
            var h = OnUpdate;
            if (h == null) return;
            h(null, new UpdateEventArgs(gameTime));
        }
        private void RaiseDraw2DEvent(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var h = OnDraw2D;
            if (h == null) return;
            h(null, new Draw2DEventArgs(gameTime, spriteBatch));
        }
        private void RaiseDraw3DEvent(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            var h = OnDraw3D;
            if (h == null) return;
            h(null, new Draw3DEventArgs(gameTime, graphicsDevice));
        }
        private void RaiseUnloadEvent(ContentManager content)
        {
            var h = OnUnload;
            if (h == null) return;
            h(null, new ContentEventArgs(content));
        }
    }
    public sealed class ContentEventArgs : EventArgs
    {
        internal ContentEventArgs(ContentManager content)
        {
            Content = content;
        }
        public ContentManager Content { get; private set; }
    }
    public sealed class UpdateEventArgs : EventArgs
    {
        internal UpdateEventArgs(GameTime gameTime)
        {
            GameTime = gameTime;
        }
        public GameTime GameTime { get; private set; }
    }
    public sealed class Draw2DEventArgs : EventArgs
    {
        internal Draw2DEventArgs(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameTime = gameTime;
            SpriteBatch = spriteBatch;
        }
        public GameTime GameTime { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

    }
    public sealed class Draw3DEventArgs : EventArgs
    {
        internal Draw3DEventArgs(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            GameTime = gameTime;
            GraphicsDevice = graphicsDevice;
        }
        public GameTime GameTime { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
    }
}
