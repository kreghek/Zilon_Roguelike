using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Интерфейс фабрики карты.
    /// </summary>
    /// <remarks>
    /// Различные реализации фабрики создают карту согласно различным алгоритмам:
    /// 1. Карта по схеме.
    /// 2. Процедурная генерация.
    /// </remarks>
    public interface IMapFactory
    {
        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <param name="generationOptions"> Настройки генерации карты. </param>
        /// <returns> Возвращает экземпляр карты. </returns>
        [NotNull]
        Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions);
    }
}