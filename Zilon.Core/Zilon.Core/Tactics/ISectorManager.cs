namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс, который предоставляет доступ к общей информации о текущем секторе.
    /// </summary>
    public interface ISectorManager
    {
        ISector CurrentSector { get; }

        void CreateSector();
    }
}