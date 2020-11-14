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
}