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
        public ISectorNode SectorNode { get => Globe.SectorNodes.Where(node => node.Sector.ActorManager.Items.Any(x => x.Person == MainPerson)).Single(); }

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