using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class Settings
    {
        /// <summary>
        /// This is the offset of the Texture atlas.
        /// This is the same as 1 / 16
        /// </summary>
        public const float Offset = 1f / (float)(1 << 4);
    }
}
