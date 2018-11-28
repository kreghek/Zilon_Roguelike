using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics.Spatial;

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

        [ExcludeFromCodeCoverage]
        public int SelectIdleDuration(int min, int max)
        {
            var roll = _dice.Roll(min, max);
            return roll;
        }

        [ExcludeFromCodeCoverage]
        public IMapNode SelectTargetRoamingNode(IEnumerable<IMapNode> mapNodes)
        {
            var roll = _dice.Roll(mapNodes.Count()) - 1;

            // Небольшая оптимизация для коллекций, которые поддерживают доступ по индексу,
            // чтобы не обходить всю коллекцию до указанного элемента.
            if (mapNodes is IList<IMapNode> mapNodesList)
            {
                return mapNodesList[roll];
            }

            // Медленный вариант доступа.
            return mapNodes.ElementAt(roll);
        }
    }
}
