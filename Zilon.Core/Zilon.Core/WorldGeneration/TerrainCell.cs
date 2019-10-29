using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Ячейка глобального мира.
    /// </summary>
    public class TerrainCell
    {
        public OffsetCoords Coords;

        public override bool Equals(object obj)
        {
            return obj is TerrainCell cell &&
                   EqualityComparer<OffsetCoords>.Default.Equals(Coords, cell.Coords);
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
    }
}
