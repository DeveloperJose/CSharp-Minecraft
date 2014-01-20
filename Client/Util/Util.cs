using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Client.Rendering;
namespace Client
{
    /// <summary>
    /// Provides a set of methods for the rendering BoundingBoxes.
    /// </summary>
    public static class BoundingBoxRenderer
    {
        #region Fields
 
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static int[] indices = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };
 
        static BasicEffect effect;
        static VertexBuffer vertex_Buffer;
 
        #endregion
 
        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use for drawing the lines of the box.</param>
        public static void Render(
            BoundingBox box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
            }
 
            Vector3[] corners = box.GetCorners();
 
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }
 
            vertex_Buffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.WriteOnly);
            vertex_Buffer.SetData<VertexPositionColor>(verts);
            graphicsDevice.SetVertexBuffer(vertex_Buffer);
 
            effect.View = view;
            effect.Projection = projection;
 
            effect.CurrentTechnique.Passes[0].Apply();
 
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    verts,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);
            }
        }
    }
    public static class RenderExtensions
    {
        private const float f = 1;
        public static Vector3 Center(this Vector3 v)
        {
            //return new Vector3((float)Math.Floor(v.X + f), (float)Math.Floor(v.Y + f), (float)Math.Floor(v.Z + f));
            return new Vector3((float)Math.Floor(v.X) + f, (float)Math.Floor(v.Y) + f, (float)Math.Floor(v.Z) + f);
        }
        public static float FixedToRenderPixels(this int fixedPoint)
        {
            return (float)(fixedPoint / 32f);
        }
        public static Vector3I ToBlockCoords(this Vector3 v)
        {
            return new Vector3I((int)v.X, (int)v.Z, (int)v.Y) / 2;
        }
        public static bool Solid(this BlockID b)
        {
            if (Transparent(b)) return false;
            switch (b)
            {
                case BlockID.Air:
                case BlockID.None:
                    return false;
                default:
                    return true;
            }
        }
        /// <summary>
        /// Determines if a block should be rendered.
        /// If true then the block is visible.
        /// </summary>
        /// <param name="b">Block to check.</param>
        /// <returns>True if the should be rendered.</returns>
        public static bool Visible(this BlockID b)
        {
            /*
             * To properly render transparent blocks other blocks must treat them as invisible.
             * That way we can see through it.
             */
            if (Transparent(b)) return false;
            switch (b)
            {
                case BlockID.Air:
                case BlockID.None:
                    return false;
                default:
                    return true;
            }
        }
        public static bool Transparent(this BlockID b)
        {
            switch (b)
            {
                case BlockID.Leaves:
                case BlockID.RedFlower:
                case BlockID.YellowFlower:
                case BlockID.RedMushroom:
                case BlockID.BrownMushroom:
                case BlockID.Glass:
                case BlockID.Water:
                case BlockID.StillWater:
                case BlockID.Lava:
                case BlockID.StillLava:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Creates the UV mapping of a block.
        /// </summary>
        /// <param name="b">The block to get the mapping of.</param>
        /// <param name="d">The direction of the block to render.</param>
        /// <returns>UVMap representing the UV mapping of the current block.</returns>
        internal static UVMap? CreateUVMapping(this BlockID b, Direction d)
        {
            /*
             * Cheat Sheet
             * x * 2^y = x << y
             * x / 2^y = x >> y      - Except that / rounds up for negatives, and >> always rounds down
             * x % 2^y = x & (2^y - 1)
             */
            // Same as X % 256
            // 256 = (2^16) = 1 << 16
            // x % 256 = x & (256 - 1)
            // x % 256 = x & ((1 << 16) - 1)
            Vector2? posOrNull = b.TileVector(d);
            if (posOrNull == null)
                return null;

            Vector2 tile = posOrNull.GetValueOrDefault();
            float x = ((int)tile.Y & ((1 << 16) - 1)) * Settings.Offset; // U
            float y = ((int)tile.X & ((1 << 16) - 1)) * Settings.Offset;

            return new UVMap(
                new Vector2(x + Settings.Offset, y),
                new Vector2(x, y),
                new Vector2(x + Settings.Offset, y + Settings.Offset),
                new Vector2(x, y + Settings.Offset)
            );
        }
        /// <summary>
        /// Gets the location of the texture of the selected block for the selected direction.
        /// </summary>
        /// <param name="b">Block to get the texture of.</param>
        /// <param name="d">Direction of the face to get the texture of.</param>
        /// <returns>Vector2 representing the location of the texture in the texture atlas.</returns>
        internal static Vector2? TileVector(this BlockID b, Direction d)
        {
            // The texture atlas is 256x256, we divide it in 16x16 sections. There are 256 textures in total. 16 rows and 16 columns.
            switch (b)
            {
                case BlockID.Stone:
                    return new Vector2(0, 1);
                case BlockID.Grass:
                    switch (d)
                    {
                        case Direction.Left:
                        case Direction.Right:
                        case Direction.Front:
                        case Direction.Back:
                            return new Vector2(0, 3);
                        case Direction.Top:
                            return new Vector2(0, 0);
                        case Direction.Bottom:
                            return new Vector2(0, 2);
                    }
                    break;
                case BlockID.Dirt:
                    return new Vector2(0, 2);
                case BlockID.Cobblestone:
                    return new Vector2(1, 0);
                case BlockID.Wood:
                    return new Vector2(0, 4);
                case BlockID.Sapling:
                    switch (d)
                    {
                        case Direction.Top:
                        case Direction.Bottom:
                            return null;
                        default:
                            return new Vector2(0, 15);
                    }
                case BlockID.Admincrete:
                    return new Vector2(1, 1);
                case BlockID.Water:
                case BlockID.StillWater:
                    return new Vector2(0, 14);
                case BlockID.Lava:
                case BlockID.StillLava:
                    return new Vector2(1, 14);
                case BlockID.Sand:
                    return new Vector2(1, 9);
                case BlockID.Gravel:
                    return new Vector2(1, 3);
                case BlockID.GoldOre:
                    switch (d)
                    {
                        case Direction.Top:
                            return new Vector2(1, 8);
                        case Direction.Bottom:
                            return new Vector2(3, 8);
                        default:
                            return new Vector2(2, 8);
                    }
                case BlockID.IronOre:
                    switch (d)
                    {
                        case Direction.Top:
                            return new Vector2(1, 7);
                        case Direction.Bottom:
                            return new Vector2(3, 7);
                        default:
                            return new Vector2(2, 7);
                    }
                case BlockID.Coal:
                    return new Vector2(2, 2);
                case BlockID.Log:
                    switch (d)
                    {
                        case Direction.Top:
                        case Direction.Bottom:
                            return new Vector2(1, 5);
                        default:
                            return new Vector2(1, 4);
                    }
                case BlockID.Leaves:
                    return new Vector2(1, 6);
                case BlockID.Sponge:
                    return new Vector2(3, 0);
                case BlockID.Glass:
                    return new Vector2(3, 1);
                case BlockID.Red:
                case BlockID.Orange:
                case BlockID.Yellow:
                case BlockID.Lime:
                case BlockID.Green:
                case BlockID.Teal:
                case BlockID.Aqua:
                case BlockID.Cyan:
                case BlockID.Blue:
                case BlockID.Indigo:
                case BlockID.Violet:
                case BlockID.Magenta:
                case BlockID.Pink:
                case BlockID.Black:
                case BlockID.Gray:
                case BlockID.White:
                    return new Vector2(4, (float)(b - 21));
                case BlockID.YellowFlower:
                    return new Vector2(0, 13);
                case BlockID.RedFlower:
                    return new Vector2(0, 12);
                case BlockID.BrownMushroom:
                    break;
                case BlockID.RedMushroom:
                    break;
                case BlockID.Gold:
                    break;
                case BlockID.Iron:
                    break;
                case BlockID.DoubleSlab:
                    break;
                case BlockID.Slab:
                    break;
                case BlockID.Bricks:
                    return new Vector2(6, 0);
                case BlockID.TNT:
                    break;
                case BlockID.Books:
                    break;
                case BlockID.MossyCobble:
                    break;
                case BlockID.Obsidian:
                    break;
            }
            return new Vector2(6, 0);
        }
    }
}
