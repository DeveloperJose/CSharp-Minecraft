using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public enum TextAnchor : byte
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    } 
    public sealed class AnchoredText
    {
        private string _text;
        private Vector2 _origin;
        private SpriteFont _font;
        private Vector2 _position;


        public AnchoredText(SpriteFont font, string text, Vector2 position)
            : this(font, text, position, TextAnchor.TopLeft) { }
        public AnchoredText(SpriteFont font, string text, Vector2 position, TextAnchor anchor)
        {
            _font = font;
            _text = text;
            _position = position;
            _origin = Vector2.Zero;

            Vector2 textSize = font.MeasureString(text);

            if (anchor == TextAnchor.TopCenter ||
                anchor == TextAnchor.MiddleCenter ||
                anchor == TextAnchor.BottomCenter)
            {
                _origin.X = textSize.X / 2;
            }
            else if (anchor == TextAnchor.TopRight ||
                     anchor == TextAnchor.MiddleRight ||
                     anchor == TextAnchor.BottomRight)
            {
                _origin.X = textSize.X;
            }

            if (anchor == TextAnchor.MiddleLeft ||
                anchor == TextAnchor.MiddleCenter ||
                anchor == TextAnchor.MiddleRight)
            {
                _origin.Y = textSize.Y / 2;
            }
            else if (anchor == TextAnchor.BottomLeft ||
                     anchor == TextAnchor.BottomCenter ||
                     anchor == TextAnchor.BottomRight)
            {
                _origin.Y = textSize.Y;
            }
        }

        // Make sure the caller of this method calls 
        // Begin and End on the SpriteBatch before and 
        // after calling this method, respectively. 
        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(_font, _text, _position, Color.White, 0f, _origin, 1f, SpriteEffects.None, 0f);
        }
    } 
}
