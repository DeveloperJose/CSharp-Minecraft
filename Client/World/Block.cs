using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public struct Block
    {
        public readonly byte ID;
        public readonly bool Solid;
        public Block(BlockID id)
        {
            ID = (byte)id;
            Solid = (id != BlockID.Air && id != BlockID.None && id != BlockID.RedFlower && id != BlockID.YellowFlower && id != BlockID.Sapling);
        }
        public Block(byte id) : this((BlockID)id) { }
    }
}
