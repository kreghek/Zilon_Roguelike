using System.Threading.Tasks;
using JetBrains.Annotations;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор секторов разного типа.
    /// </summary>
    public interface ISectorGenerator
    {
        /// <summary> Создаёт сектор подземелья с учётом указанных настроек. </summary>
        /// <param name="sectorScheme"> Схема создания сектора. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        Task<ISector> GenerateDungeonAsync(ISectorSubScheme sectorScheme);

        /// <summary>
        /// Создаёт сектор квартала города.
        /// </summary>
        /// <param name="globe"> Объект мира. </param>
        /// <param name="globeNode"> Узел провинции, на основе которого генерируется сектор. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        /// <remarks>
        /// Нужно будет передавать параметры зданий, наличие персонажей и станков для крафта.
        /// Вместо общей информации об узле.
        /// </remarks>
        Task<ISector> GenerateTownQuarterAsync(Globe globe, GlobeRegionNode globeNode);

        /// <summary>
        /// Создаёт сектор фрагмента дикого окружения.
        /// </summary>
        /// <param name="globe"> Объект мира. </param>
        /// <param name="globeNode"> Узел провинции, на основе которого генерируется сектор. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        /// <remarks>
        /// Нужно будет передавать параметры окружения и количество
        /// и характеристики монстров.
        /// </remarks>
        Task<ISector> GenerateWildAsync(Globe globe, GlobeRegionNode globeNode);
    }
}