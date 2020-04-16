using System;
using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class FowContext : IFowContext
    {
        private readonly ISector _sector;

        public FowContext(ISector sector)
        {
            _sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            return _sector.Map.GetNext(node);
        }

        public bool IsTargetVisible(IGraphNode baseNode, IGraphNode targetNode)
        {
            return _sector.Map.TargetIsOnLine(baseNode, targetNode);
        }
    }
}
