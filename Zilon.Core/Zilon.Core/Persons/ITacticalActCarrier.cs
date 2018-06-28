namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс владельца действий.
    /// </summary>
    public interface ITacticalActCarrier
    {
        /// <summary>
        /// Действие по умолчанию.
        /// </summary>
        ITacticalAct DefaultAct { get; }

        /// <summary>
        /// Набор всех действий.
        /// </summary>
        ITacticalAct[] Acts { get; }
    }
}