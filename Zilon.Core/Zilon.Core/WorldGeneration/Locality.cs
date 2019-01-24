namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public string Name { get; set; }

        public TerrainCell[] Cells { get; set; }
    }
}
