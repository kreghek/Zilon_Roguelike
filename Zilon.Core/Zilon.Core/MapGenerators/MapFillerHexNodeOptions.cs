namespace Zilon.Core.MapGenerators
{
    public struct MapFillerHexNodeOptions : System.IEquatable<MapFillerHexNodeOptions>
    {
        public bool IsObstacle { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MapFillerHexNodeOptions options &&
                   Equals(options);
        }

        public bool Equals(MapFillerHexNodeOptions other)
        {
            return IsObstacle == other.IsObstacle;
        }

        public override int GetHashCode()
        {
            return -1199661580 + IsObstacle.GetHashCode();
        }

        public static bool operator ==(MapFillerHexNodeOptions left, MapFillerHexNodeOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MapFillerHexNodeOptions left, MapFillerHexNodeOptions right)
        {
            return !(left == right);
        }
    }
}