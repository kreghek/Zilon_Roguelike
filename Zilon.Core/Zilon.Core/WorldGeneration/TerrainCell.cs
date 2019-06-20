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

        public override string ToString()
        {
            return $"{Coords}";
        }
    }
}
