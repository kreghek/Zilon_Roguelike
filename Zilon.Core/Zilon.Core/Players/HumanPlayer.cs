using System;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer : PlayerBase
    {
        /// <summary>
        /// Текущая провинция группы игрока.
        /// </summary>
        public TerrainCell Terrain { get; set; }

        public ISectorNode SectorNode { get; private set; }

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public HumanPerson MainPerson { get; set; }

        public void BindSectorNode(ISectorNode sectorNode)
        {
            SectorNode = sectorNode;
        }
    }
}
