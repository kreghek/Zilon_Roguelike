using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public static class IntruderDetectionHelper
    {
        public static IActor GetIntruder(IActor actor, ISectorMap map, IActorManager actorManager)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            if (actorManager is null)
            {
                throw new System.ArgumentNullException(nameof(actorManager));
            }

            var intruders = CheckForIntruders(actor, map, actorManager);

            var orderedIntruders = intruders.OrderBy(x => map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            return nearbyIntruder;
        }

        private static IEnumerable<IActor> CheckForIntruders(IActor actor, ISectorMap map, IActorManager actorManager)
        {
            foreach (var target in actorManager.Items)
            {
                if (target.Person.Fraction == actor.Person.Fraction ||
                    (target.Person.Fraction == Fractions.MilitiaFraction &&
                     actor.Person.Fraction == Fractions.MainPersonFraction) ||
                    (target.Person.Fraction == Fractions.MainPersonFraction &&
                     actor.Person.Fraction == Fractions.MilitiaFraction) ||
                    (target.Person.Fraction == Fractions.InterventionistFraction &&
                     actor.Person.Fraction == Fractions.TroublemakerFraction) ||
                    (target.Person.Fraction == Fractions.TroublemakerFraction &&
                     actor.Person.Fraction == Fractions.InterventionistFraction))
                {
                    continue;
                }

                if (target.Person.CheckIsDead())
                {
                    continue;
                }

                var isVisible = LogicHelper.CheckTargetVisible(map, actor.Node, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                yield return target;
            }
        }
    }
}