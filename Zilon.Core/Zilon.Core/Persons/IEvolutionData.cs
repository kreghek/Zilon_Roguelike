namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс, содержащий данные о развитие персонажа.
    /// </summary>
    public interface IEvolutionData
    {
        /// <summary>
        /// Текущие активные перки.
        /// </summary>
        IPerk[] ActivePerks { get; }

        /// <summary>
        /// Полученные перки.
        /// </summary>
        IPerk[] ArchievedPerks { get; }
    }
}
