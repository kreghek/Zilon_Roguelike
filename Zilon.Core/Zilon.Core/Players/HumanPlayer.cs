using System;
using System.Linq;

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
        public ISectorNode SectorNode => GetSectorNode();

        private ISectorNode GetSectorNode()
        {
            var sectorNode = Globe.SectorNodes.SingleOrDefault(IsActorInSector);
            if (sectorNode is null)
            {
                throw new InvalidOperationException($"There is no sector with the player person.");
            }

            return sectorNode;
        }

        private bool IsActorInSector(ISectorNode node)
        {
            return node.Sector.ActorManager.Items.Any(x => x.Person == MainPerson);
        }

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public IPerson MainPerson { get; private set; }

        public IGlobe Globe { get; private set; }

        public void BindPerson(IGlobe globe, IPerson person)
        {
            if (globe is null)
            {
                throw new System.ArgumentNullException(nameof(globe));
            }

            if (person is null)
            {
                throw new System.ArgumentNullException(nameof(person));
            }

            Globe = globe;
            MainPerson = person;
        }

        public void Reset()
        {
            Globe = null;
            MainPerson = null;
        }
    }
}