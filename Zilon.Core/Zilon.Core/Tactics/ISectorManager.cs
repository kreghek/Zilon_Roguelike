using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс, который предоставляет доступ к общей информации о текущем секторе.
    /// Главное назначение - хранить сгенерированный сектор.
    /// </summary>
    public interface ISectorManager
    {
        /// <summary>
        /// Текущий сектор.
        /// </summary>
        ISector CurrentSector { get; }

        /// <summary>
        /// Создаёт текущий сектор по указанному генератору и настройкам.
        /// </summary>
        /// <param name="generator">Генератор сектора.</param>
        /// <param name="scheme">Схема генерации сектора.</param>
        Task CreateSectorAsync(ISectorGenerator generator, ISectorSubScheme scheme);
    }
}