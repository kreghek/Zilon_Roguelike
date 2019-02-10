using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

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

        /// <summary> Текущий уровень сектора подземелья. Используется только в подземельях. </summary>
        int SectorLevel { get; set; }

        /// <summary>
        /// Создаёт и присваивает сектор для текущего узла локации.
        /// </summary>
        Task CreateSectorAsync();
    }
}