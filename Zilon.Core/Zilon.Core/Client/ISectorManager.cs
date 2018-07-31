using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Интерфейс, который предоставляет доступ к общей информации о текущем секторе.
    /// </summary>
    public interface ISectorManager
    {
        ISector CurrentSector { get; set; }
    }
}