using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer : IPlayer
    {
        public ISectorNode SectorNode { get; private set; }

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public IPerson MainPerson { get; set; }

        public void BindSectorNode(ISectorNode sectorNode)
        {
            SectorNode = sectorNode;
        }

        public void Reset()
        {
            SectorNode = null;
            MainPerson = null;
        }
    }
}