using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JClient
{
    public struct Vector3I
    {
        public static readonly Vector3I Zero = new Vector3I(0, 0, 0);
        public int X, Y, Z;

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
