using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    public class NationalUnityEventService
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;
        private readonly IDice _dice;
        private readonly IPersonFactory _personFactory;
        private readonly IUserTimeProvider _userTimeProvider;

        public NationalUnityEventService(
            IPersonFactory personFactory,
            IDice dice,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource,
            IUserTimeProvider userTimeProvider)
        {
            _personFactory = personFactory;
            _dice = dice;
            _actorTaskSource = actorTaskSource;
            _userTimeProvider = userTimeProvider;
        }

        public IGlobe Globe { get; set; }

        public void RollAndCreateUnityGroupIntoSector(ISector sector)
        {
            if (Globe is null)
            {
                return;
            }

            if (sector.ActorManager.Items.Any())
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
            else
            {
                var faction = _dice.RollFromList(new[]
                {
                    Fractions.InterventionistFraction, Fractions.MilitiaFraction, Fractions.TroublemakerFraction
                });

                var count = _dice.Roll(2, 5);
                for (var i = 0; i < count; i++)
                {
                    var person = _personFactory.Create("human-person", faction);
                    var nodes = sector.Map.Nodes.ToArray();
                    var startNode = _dice.RollFromList(nodes);

                    var actor = new Actor(person, _actorTaskSource, startNode);
                    sector.ActorManager.Add(actor);
                }
            }
        }

        public bool RollEventIsRaised()
        {
            // The day of national unity.
            var targetData = new DateTime(DateTime.Now.Year, 11, 4);

            var max = _userTimeProvider.GetCurrentTime();
            var min = targetData;

            if (min > max)
            {
                var temp = min;
                min = max;
                max = temp;
            }

            var dateDistance = (max - min).TotalDays;
            const double EVENT_BONUS_DURATION = 10d;
            const double EVENT_BONUS_MAX_PROBABILITY = 10;
            const double EVENT_BONUS_MIN_PROBABILITY = 50;
            const double EVENT_BONUS_PROBABILITY_DIFF = EVENT_BONUS_MIN_PROBABILITY - EVENT_BONUS_MAX_PROBABILITY;
            var dateDistanceNormalized = Math.Min(dateDistance, EVENT_BONUS_DURATION);
            var eventRaiseRollValue = EVENT_BONUS_MAX_PROBABILITY +
                                      ((EVENT_BONUS_PROBABILITY_DIFF * dateDistanceNormalized) / EVENT_BONUS_DURATION);

            return _dice.Roll(100) > eventRaiseRollValue;
        }
    }
}