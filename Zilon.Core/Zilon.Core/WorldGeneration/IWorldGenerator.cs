using System.Threading.Tasks;

using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Интерфейс генератора игрового мира.
    /// </summary>
    /// <remarks>
    /// Создаёт историю, граф провинций и узлы локаций внутри провинций.
    /// </remarks>
    public interface IWorldGenerator
    {
        /// <summary>
        /// Создание игрового мира с историей и граф провинций.
        /// </summary>
        /// <returns> Возвращает объект игрового мира. </returns>
        Task<GlobeGenerationResult> GenerateGlobeAsync();

        /// <summary>
        /// Создание 
        /// </summary>
        /// <param name="globe"> Объект игрового мира, для которого создаётся локация. </param>
        /// <param name="cell"> Провинция игрового мира из указанного выше <see cref="Globe"/>,
        /// для которого создаётся локация. </param>
        /// <returns> Возвращает граф локация для провинции. </returns>
        Task<GlobeRegion> GenerateRegionAsync(Globe globe, TerrainCell cell);
    }
}