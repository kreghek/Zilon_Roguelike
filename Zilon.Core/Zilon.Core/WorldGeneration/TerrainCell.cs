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
            var otherCell = obj as TerrainCell;

            if (otherCell == null)
            {
                return false;
            }

            return Coords.X == otherCell.Coords.X && Coords.Y == otherCell.Coords.Y;
        }

        public override int GetHashCode()
        {
            return -319820321 + EqualityComparer<OffsetCoords>.Default.GetHashCode(Coords);
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
