using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Client.Rendering;
namespace Client
{
    public static class RenderExtensions
    {
        /// <summary>
        /// Determines if a block should be rendered.
        /// If true then the block is visible.
        /// </summary>
        /// <param name="b">Block to check.</param>
        /// <returns>True if the should be rendered.</returns>
        public static bool Visible(this Block b)
        {
            return (b.ID != (byte)BlockID.Air && b.ID != (byte)BlockID.None);
        }
        /// <summary>
        /// Creates the UV mapping of a block.
        /// </summary>
        /// <param name="b">The block to get the mapping of.</param>
        /// <param name="d">The direction of the block to render.</param>
        /// <returns>UVMap representing the UV mapping of the current block.</returns>
        public static UVMap CreateUVMapping(this Block b, Direction d)
        {
            /*
             * x * 2^y = x << y
             * x / 2^y = x >> y      - Except that / rounds up for negatives, and >> always rounds down
             * x % 2^y = x & (2^y - 1)
             */
            // Same as Y % 256
            // 256 = (2^16) = 1 << 16
            // x % 256 = x & (256 - 1)
            // x % 256 = x & ((1 << 16) - 1)
            float x = ((int)b.TileVector(d).Y & ((1 << 16) - 1)) * Settings.Offset; // U
            // Same as X / 256
            // x / 256 = x >> 16
            //float y = ((int)b.TileVector(d).X >> 16) * Settings.Offset; // V
            float y = ((int)b.TileVector(d).X & ((1 << 16) - 1)) * Settings.Offset;
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
        public static Vector2 TileVector(this Block b, Direction d)
        {
            // The texture atlas is 256x256, we divide it in 16x16 sections. There are 256 textures in total. 16 rows and 16 columns.
            switch ((BlockID)b.ID)
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
                    break;
                case BlockID.Wood:
                    break;
                case BlockID.Sapling:
                    break;
                case BlockID.Admincrete:
                    return new Vector2(1, 1);
                case BlockID.Water:
                    break;
                case BlockID.StillWater:
                    break;
                case BlockID.Lava:
                    break;
                case BlockID.StillLava:
                    break;
                case BlockID.Sand:
                    break;
                case BlockID.Gravel:
                    break;
                case BlockID.GoldOre:
                    break;
                case BlockID.IronOre:
                    break;
                case BlockID.Coal:
                    break;
                case BlockID.Log:
                    break;
                case BlockID.Leaves:
                    break;
                case BlockID.Sponge:
                    break;
                case BlockID.Glass:
                    break;
                case BlockID.Red:
                    break;
                case BlockID.Orange:
                    break;
                case BlockID.Yellow:
                    break;
                case BlockID.Lime:
                    break;
                case BlockID.Green:
                    break;
                case BlockID.Teal:
                    break;
                case BlockID.Aqua:
                    break;
                case BlockID.Cyan:
                    break;
                case BlockID.Blue:
                    break;
                case BlockID.Indigo:
                    break;
                case BlockID.Violet:
                    break;
                case BlockID.Magenta:
                    break;
                case BlockID.Pink:
                    break;
                case BlockID.Black:
                    break;
                case BlockID.Gray:
                    break;
                case BlockID.White:
                    break;
                case BlockID.YellowFlower:
                    break;
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
                    return new Vector2(FirstPersonCamera.Y, FirstPersonCamera.X);
                case BlockID.TNT:
                    break;
                case BlockID.Books:
                    break;
                case BlockID.MossyCobble:
                    break;
                case BlockID.Obsidian:
                    break;
                default:
                    return new Vector2(1, 6);
            }
            return new Vector2(1, 6);
        }
    }
}
