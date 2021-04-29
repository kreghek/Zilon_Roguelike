using System;

namespace Zilon.Core
{
    public struct OffsetCoords : IEquatable<OffsetCoords>
    {
        public int X { get; }
        public int Y { get; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public OffsetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool Equals(OffsetCoords other)
        {
            return X == other.X && Y == other.Y;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return obj is OffsetCoords coords && Equals(coords);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            return left.Equals(right);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }
    }
}