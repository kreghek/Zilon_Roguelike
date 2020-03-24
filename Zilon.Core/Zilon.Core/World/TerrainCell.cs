using System.Collections.Generic;

namespace Zilon.Core.World
{
    /// <summary>
    /// Ячейка глобального мира.
    /// </summary>
    public struct TerrainCell : System.IEquatable<TerrainCell>
    {
        public OffsetCoords Coords { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TerrainCell cell && Equals(cell);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return -319820321 + EqualityComparer<OffsetCoords>.Default.GetHashCode(Coords);
            }
        }

        public override string ToString()
        {
            return $"{Coords}";
        }

        public static bool operator ==(TerrainCell left, TerrainCell right)
        {
            return EqualityComparer<TerrainCell>.Default.Equals(left, right);
        }

        public static bool operator !=(TerrainCell left, TerrainCell right)
        {
            return !(left == right);
        }

        public bool Equals(TerrainCell other)
        {
            return EqualityComparer<OffsetCoords>.Default.Equals(Coords, other.Coords);
        }
    }
}
