using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    ///     Селектор фабрики для генерации карты сектора.
    /// </summary>
    public interface IMapFactorySelector
    {
        /// <summary>
        ///     Возвращает генератор карты.
        /// </summary>
        /// <param name="sectorScheme">
        ///     Схема сектора, на основе которой будет принято решение,
        ///     какой генератор карты использовать.
        /// </param>
        /// <returns> Возвращает фабрику карт для сектора. </returns>
        IMapFactory GetMapFactory(ISectorNode sectorNode);
    }
}