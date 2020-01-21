using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics
{
    public sealed class HumanSectorFowData : ISectorFowData
    {
        private readonly List<SectorMapFowNode> _nodes;

        public HumanSectorFowData()
        {
            _nodes = new List<SectorMapFowNode>();
        }

        public IEnumerable<SectorMapFowNode> Nodes => _nodes;

        public void AddNodes(IEnumerable<SectorMapFowNode> nodes)
        {
            var overlapedNodes = nodes.Where(x => Nodes.Contains(x));
            if (overlapedNodes.Any())
            {
                throw new InvalidOperationException($"Добавляются узлы тумана войны, перекрывающие существующик узлы {string.Join(",", overlapedNodes.Select(x => x.Node))}.");
            }

            _nodes.AddRange(nodes);
        }
    }
}
