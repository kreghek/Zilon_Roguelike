using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс, содержащий данные о развитие персонажа.
    /// </summary>
    public interface IEvolutionData
    {
        /// <summary>
        /// Текущие перки.
        /// </summary>
        IPerk[] Perks { get; }

        /// <summary>
        /// Указывает, что один из активных перков считается прокачанным.
        /// </summary>
        /// <param name="perk"> Активный перк, который следует считать достигнутым. </param>
        void PerkLevelUp(IPerk perk);

        /// <summary>
        /// Выстреливает, когда один из перков повышается на уровень.
        /// </summary>
        event EventHandler<PerkEventArgs> PerkLeveledUp;
    }
}
