using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HexaClassicClient
{
    public struct Block
    {
        public readonly byte ID;
        public readonly bool Visible;
        public Block(BlockID id)
        {
            ID = (byte)id;
            Visible = id != BlockID.Air && id != BlockID.None;
        }
        public Block(byte id) : this((BlockID)id) { }
    }
}
