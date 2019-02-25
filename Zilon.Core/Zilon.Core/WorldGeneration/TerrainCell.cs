namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Ячейка глобального мира.
    /// </summary>
    public class TerrainCell
    {
        public OffsetCoords Coords;

        public override string ToString()
        {
            return $"{Coords}";
        }
    }
}
