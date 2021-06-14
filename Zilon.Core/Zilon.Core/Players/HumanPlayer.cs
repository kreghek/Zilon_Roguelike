using System;
using System.Diagnostics.CodeAnalysis;
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
        private ISectorNode GetSectorNode()
        {
            if (Globe is null)
            {
                throw new InvalidOperationException("Globe is not assigned.");
            }

            var sectorNode = Globe.SectorNodes.SingleOrDefault(IsActorInSector);
            if (sectorNode is null)
            {
                throw new InvalidOperationException("There is no sector with the player person.");
            }

            return sectorNode;
        }

        private bool IsActorInSector(ISectorNode node)
        {
            var sector = node.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            return sector.ActorManager.Items.Any(x => x.Person == MainPerson);
        }

        public ISectorNode SectorNode => GetSectorNode();

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public IPerson? MainPerson { get; private set; }

        [ExcludeFromCodeCoverage]
        public IGlobe? Globe { get; private set; }

        [ExcludeFromCodeCoverage]
        public void BindPerson(IGlobe globe, IPerson person)
        {
            Globe = globe;
            MainPerson = person;
        }

        [ExcludeFromCodeCoverage]
        public void Reset()
        {
            Globe = null;
            MainPerson = null;
        }
    }
}