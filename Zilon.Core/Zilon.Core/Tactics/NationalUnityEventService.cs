using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    public class NationalUnityEventService
    {
        private readonly IPersonFactory _personFactory;
        private readonly IDice _dice;
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;

        public NationalUnityEventService(IPersonFactory personFactory, IDice dice, IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            _personFactory = personFactory;
            _dice = dice;
            _actorTaskSource = actorTaskSource;
        }

        public IGlobe Globe { get; set; }

        public void RollAndCreateUnityGroupIntoSector(ISector sector)
        {
            if (Globe is null)
            {
                return;
            }

            // Interventionists spawns first.
            // If globe has no interventionists then spawn they.
            // Next randomly spawns interventionists or militia.
            var interventionistsCount = Globe.SectorNodes.Select(x => x.Sector)
                .SelectMany(x => x.ActorManager.Items)
                .Where(x => x.Person.Fraction == Fractions.InterventionistFraction)
                .Count();

            if (interventionistsCount <= 0)
            {
                var count = _dice.Roll(2, 5);
                for (var i = 0; i < count; i++)
                {
                    var person = _personFactory.Create("human-person", Fractions.InterventionistFraction);
                    var nodes = sector.Map.Nodes.ToArray();
                    var startNode = _dice.RollFromList(nodes);

                    var actor = new Actor(person, _actorTaskSource, startNode);
                    sector.ActorManager.Add(actor);
                }
            }
        }

        public bool RollEventIsRaised()
        {
            return _dice.Roll(100) < 50;
        }
    }
}