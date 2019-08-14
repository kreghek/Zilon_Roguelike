using System.Collections.Generic;

namespace Zilon.Core
{
    public sealed class OffsetCoords
    {
        public int X { get; }
        public int Y { get; }

        public OffsetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetCoords coords &&
                   X == coords.X &&
                   Y == coords.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            return EqualityComparer<OffsetCoords>.Default.Equals(left, right);
        }

        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }
    }
}
