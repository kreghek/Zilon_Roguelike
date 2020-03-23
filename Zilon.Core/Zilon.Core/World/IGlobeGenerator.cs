using System.Threading.Tasks;

namespace Zilon.Core.World
{
    /// <summary>
    /// Интерфейс генератора игрового мира.
    /// </summary>
    /// <remarks>
    /// Создаёт историю, граф провинций и узлы локаций внутри провинций.
    /// </remarks>
    public interface IGlobeGenerator
    {
        /// <summary>
        /// Создание игрового мира с историей и граф провинций.
        /// </summary>
        /// <returns> Возвращает объект игрового мира. </returns>
        Task<GlobeGenerationResult> CreateGlobeAsync();
    }
}