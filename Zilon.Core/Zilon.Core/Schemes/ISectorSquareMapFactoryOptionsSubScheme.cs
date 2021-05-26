namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема параметров генерации карты в виде квадратной области.
    /// </summary>
    public interface ISectorSquareMapFactoryOptionsSubScheme : ISectorMapFactoryOptionsSubScheme
    {
        /// <summary>
        /// Размер карты.
        /// </summary>
        int Size { get; }
    }

    /// <summary>
    /// The options scheme for open huge maps.
    /// </summary>
    public interface ISectorOpenMapFactoryOptionsSubScheme : ISectorMapFactoryOptionsSubScheme
    {
        /// <summary>
        /// Size (radius) of the map in nodes.
        /// </summary>
        int Size { get; }
    }
}