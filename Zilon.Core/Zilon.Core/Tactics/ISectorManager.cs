using System.Threading.Tasks;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс, который предоставляет доступ к общей информации о текущем секторе.
    /// Главное назначение - создавать сектор в зависимости от локации на глобальной карте и
    /// хранить созданный сервис для предоставления его другим сервисам.
    /// </summary>
    public interface ISectorManager
    {
        /// <summary>
        /// Текущий сектор.
        /// </summary>
        ISector CurrentSector { get; }

        /// <summary>
        /// Создаёт и присваивает сектор для текущего узла локации.
        /// </summary>
        Task CreateSectorAsync();
    }
}