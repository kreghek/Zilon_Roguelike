using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core
{
    public struct CubeCoords : IEquatable<CubeCoords>
    {
        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public CubeCoords(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"(X: {X}, Y: {Y}, Z: {Z})";
        }

        public static CubeCoords operator *(CubeCoords v, int q)
        {
            return new CubeCoords(v.X * q, v.Y * q, v.Z * q);
        }

        public static CubeCoords operator *(int q, CubeCoords v)
        {
            return new CubeCoords(v.X * q, v.Y * q, v.Z * q);
        }

        public static bool operator ==(CubeCoords left, CubeCoords right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CubeCoords left, CubeCoords right)
        {
            return !(left == right);
        }

        public static CubeCoords operator +(CubeCoords v, CubeCoords v2)
        {
            return new CubeCoords(v.X + v2.X, v.Y + v2.Y, v.Z + v2.Z);
        }

        public int DistanceTo(CubeCoords cubeCoords)
        {
            var a = this;
            var b = cubeCoords;
            var distance1 = Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
            var distance = Math.Max(distance1, Math.Abs(a.Z - b.Z));

            return distance;
        }

        public override bool Equals(object obj)
        {
            return obj is CubeCoords coords && Equals(coords);
        }

        public bool Equals(CubeCoords other)
        {
            return (X == other.X) &&
                   (Y == other.Y) &&
                   (Z == other.Z);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -307843816;
                hashCode = (hashCode * -1521134295) + X.GetHashCode();
                hashCode = (hashCode * -1521134295) + Y.GetHashCode();
                hashCode = (hashCode * -1521134295) + Z.GetHashCode();
                return hashCode;
            }
        }

        public static CubeCoords Multiply(CubeCoords left, CubeCoords right)
        {
            return new CubeCoords(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static CubeCoords Add(CubeCoords left, CubeCoords right)
        {
            return left + right;
        }
    }
}