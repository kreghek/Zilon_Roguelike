using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core
{
    public struct OffsetCoords : IEquatable<OffsetCoords>
    {
        public int X { get; }
        public int Y { get; }

        [ExcludeFromCodeCoverage]
        public OffsetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 1861411795;
                hashCode = (hashCode * -1521134295) + X.GetHashCode();
                hashCode = (hashCode * -1521134295) + Y.GetHashCode();
                return hashCode;
            }
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(OffsetCoords other)
        {
            return X == other.X && Y == other.Y;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return obj is OffsetCoords coords && Equals(coords);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            return left.Equals(right);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }
    }
}