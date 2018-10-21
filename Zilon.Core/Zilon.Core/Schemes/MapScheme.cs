namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема карты.
    /// </summary>
    /// <remarks>
    /// Карта - это совокупность узлов и ребёр перехода между узлами.
    /// </remarks>
    public class MapScheme : SchemeBase, IMapScheme
    {
        public MapScheme(int fake)
        {
            Fake = fake;
        }

        public int Fake { get; }
    }
}
