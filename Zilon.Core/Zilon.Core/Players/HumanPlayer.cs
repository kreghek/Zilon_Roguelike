using System;

using Zilon.Core.Persons;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer : PlayerBase
    {
        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public HumanPerson MainPerson { get; set; }

        /// <summary> 
        /// Текущий идентфиикатор сектора, где сейчас находится группа игрока.
        /// </summary>
        public string SectorSid { get; set; }

        /// <summary>
        /// Текущий идентфиикатор уровня сектора, где сейчас находится группа игрока.
        /// </summary>
        public string SectorLevelSid { get; set; }

        /// <summary>
        /// Событие выстреливает, если зменяется узел группы игрока на глобальной карте.
        /// </summary>
        public event EventHandler GlobeNodeChanged;
    }
}
