using System;

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

        /// <summary>
        /// Указывает, что один из активных перков считается прокачанным.
        /// </summary>
        /// <param name="perk"> Активный перк, который следует считать достигнутым. </param>
        void ActivePerkArchieved(IPerk perk);

        /// <summary>
        /// Выстреливает, когда среди достигнутых появляется новый перк.
        /// </summary>
        event EventHandler<PerkEventArgs> PerkArchieved;
    }
}
