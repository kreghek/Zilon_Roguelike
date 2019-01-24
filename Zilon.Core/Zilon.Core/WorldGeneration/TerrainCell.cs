namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Ячейка глобального мира.
    /// </summary>
    public class TerrainCell
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
