using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Client
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
        public Vector3I(Vector3 v)
        {
            X = (int)v.X;
            Y = (int)v.Z; // Reverse
            Z = (int)v.Y;
        }
        public static Vector3I operator +(Vector3I a, Vector3I b)
        {
            return new Vector3I(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3I operator -(Vector3I a, Vector3I b)
        {
            return new Vector3I(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3I operator /(Vector3I a, int scalar)
        {
            return new Vector3I(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }
        public static implicit operator Vector3(Vector3I v)
        {
            return new Vector3(v.X, v.Z, v.Y); // Reverse Y and Z
        }
    }
}
