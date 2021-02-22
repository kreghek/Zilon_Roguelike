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
            return node.Sector.ActorManager.Items.Any(x => x.Person == MainPerson);
        }

        public ISectorNode SectorNode => GetSectorNode();

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IPerson MainPerson { get; private set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IGlobe Globe { get; private set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void BindPerson(IGlobe globe, IPerson person)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            Globe = globe;
            MainPerson = person;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void Reset()
        {
            Globe = null;
            MainPerson = null;
        }
    }
}