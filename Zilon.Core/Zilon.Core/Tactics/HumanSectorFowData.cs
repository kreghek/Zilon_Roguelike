using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    public sealed class HumanSectorFowData : ISectorFowData
    {
        private readonly Dictionary<IGraphNode, SectorMapFowNode> _nodes;

        public HumanSectorFowData()
        {
            _nodes = new Dictionary<IGraphNode, SectorMapFowNode>();
        }

        public IEnumerable<SectorMapFowNode> Nodes => _nodes.Values;

        public void AddNodes(IEnumerable<SectorMapFowNode> nodes)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var overlapedNodes = nodes.Where(x => Nodes.Contains(x));
            if (overlapedNodes.Any())
            {
                throw new InvalidOperationException($"Добавляются узлы тумана войны, перекрывающие существующик узлы {string.Join(",", overlapedNodes.Select(x => x.Node))}.");
            }

            foreach (var node in nodes)
            {
                _nodes[node.Node] = node;
            }
        }

        public SectorMapFowNode GetNode(IGraphNode node)
        {
            if (_nodes.TryGetValue(node, out var sectorMapFowNode))
            {
                return sectorMapFowNode;
            }

            return null;
        }
    }
}
