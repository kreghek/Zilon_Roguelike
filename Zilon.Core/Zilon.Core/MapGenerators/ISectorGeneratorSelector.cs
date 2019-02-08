using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Селектор генератора сектора в зависимости от локации.
    /// </summary>
    public interface ISectorGeneratorSelector
    {
        /// <summary>
        /// Выбирает генератор сектора в зависимости от локации.
        /// </summary>
        /// <param name="globeNode"> Узел локации на графе провинции на глобальной карте. </param>
        /// <returns> Возвращает генератор сектора. </returns>
        ISectorGenerator GetGenerator(GlobeRegionNode globeNode);
    }
}
