namespace Zilon.Core
{
    public struct OffsetCoords : IEquatable<OffsetCoords>
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

        public bool Equals(OffsetCoords other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetCoords coords && Equals(coords);
        }

        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }
    }
}