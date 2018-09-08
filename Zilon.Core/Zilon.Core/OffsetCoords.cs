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
            var coords = obj as OffsetCoords;
            return coords != null &&
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
    }
}
