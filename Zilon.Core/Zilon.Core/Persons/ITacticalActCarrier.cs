namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс владельца действий.
    /// </summary>
    public interface ITacticalActCarrier
    {
        /// <summary>
        /// Набор всех действий.
        /// </summary>
        ITacticalAct[] Acts { get; set; }
    }
}