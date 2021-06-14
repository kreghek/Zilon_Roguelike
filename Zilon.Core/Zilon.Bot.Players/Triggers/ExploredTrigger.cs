using System;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class ExploredTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        /// <summary>
        /// Checks situation when all nodes of a map are explored (known) by the person.
        /// </summary>
        /// <returns>True if all nodes are explored. False otherside.</returns>
        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (currentState is null)
            {
                throw new ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new ArgumentNullException(nameof(strategyData));
            }

            var fowModule = actor.Person.GetModuleSafe<IFowData>();
            if (fowModule is null)
            {
                var notObservedNodes = context.Sector.Map.Nodes.Where(x => !strategyData.ObservedNodes.Contains(x));

                var allNodesObserved = !notObservedNodes.Any();
                return allNodesObserved;
            }
            else
            {
                var fowData = fowModule.GetSectorFowData(context.Sector);
                var observingNodes = fowData.GetFowNodeByState(SectorMapNodeFowState.Observing).Select(x => x.Node)
                    .ToArray();
                var exploredNodes = fowData.GetFowNodeByState(SectorMapNodeFowState.Explored).Select(x => x.Node)
                    .ToArray();

                var knownNodes = observingNodes.Union(exploredNodes);

                var notObservedNodes = context.Sector.Map.Nodes.Where(x => !knownNodes.Contains(x));

                var allNodesObserved = !notObservedNodes.Any();
                return allNodesObserved;
            }
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}