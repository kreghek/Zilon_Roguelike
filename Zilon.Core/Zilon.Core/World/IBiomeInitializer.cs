using System.Threading.Tasks;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    /// <summary>
    /// Интерфейс сервиса для создания графа биома.
    /// </summary>
    public interface IBiomeInitializer
    {
        /// <summary>
        /// Стартовая инициализация биома.
        /// </summary>
        /// <param name="locationScheme"> Локация, но основе которой нужно проинициализировать биом. </param>
        /// <returns> Возвращает биом с как минимум одним материализованным узлом. </returns>
        Task<IBiome> InitBiomeAsync(ILocationScheme locationScheme);

        /// <summary>
        /// Материализация узла биома.
        /// </summary>
        /// <param name="sectorNode"> Целевой узел. </param>
        Task MaterializeLevelAsync(ISectorNode sectorNode);
    }
}