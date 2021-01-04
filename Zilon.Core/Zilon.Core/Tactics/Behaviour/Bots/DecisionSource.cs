using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class DecisionSource : IDecisionSource
    {
        private readonly IDice _dice;

        [ExcludeFromCodeCoverage]
        public DecisionSource(IDice dice)
        {
            _dice = dice;
        }

        private IGraphNode SelectRandomEnumerableImpl(IEnumerable<IGraphNode> mapNodes)
        {
            var list = mapNodes.ToList();
            var node = _dice.RollFromList(list);
            return node;
        }

        private IGraphNode SelectRandomListImpl(IList<IGraphNode> mapNodesList)
        {
            return SelectRandomEnumerableImpl(mapNodesList);
        }

        [ExcludeFromCodeCoverage]
        public int SelectIdleDuration(int min, int max)
        {
            var roll = _dice.Roll(min, max);
            return roll;
        }

        [ExcludeFromCodeCoverage]
        public IGraphNode SelectTargetRoamingNode(IEnumerable<IGraphNode> mapNodes)
        {
            // Небольшая оптимизация для коллекций, которые поддерживают доступ по индексу,
            // чтобы не обходить всю коллекцию до указанного элемента.
            if (mapNodes is IList<IGraphNode> mapNodesList)
            {
                return SelectRandomListImpl(mapNodesList);
            }

            // Медленный вариант доступа.
            return SelectRandomEnumerableImpl(mapNodes);
        }
    }
}