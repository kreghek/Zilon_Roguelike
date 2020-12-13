using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer : IPlayer
    {
        public ISectorNode SectorNode =>
            Globe.SectorNodes.Single(node => node.Sector.ActorManager.Items.Any(x => x.Person == MainPerson));

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
            var actor = Globe.SectorNodes.Select(x => x.Sector).SelectMany(x => x.ActorManager.Items).SingleOrDefault(x => x.Person == MainPerson);
            if (actor != null)
            {
                if (actor.TaskSource is IHumanActorTaskSource<ISectorTaskSourceContext> humanTaskSource)
                {
                    //TODO Cancel current task waiting
                }
            }

            Globe = null;
            MainPerson = null;
        }
    }
}