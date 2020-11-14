using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    public sealed class HumanSectorFowData : ISectorFowData
    {
        private readonly Dictionary<IGraphNode, SectorMapFowNode> _nodes;

        private readonly Dictionary<SectorMapNodeFowState, List<SectorMapFowNode>> _sectorNodeHash;

        public HumanSectorFowData()
        {
            _nodes = new Dictionary<IGraphNode, SectorMapFowNode>();
            _sectorNodeHash = new Dictionary<SectorMapNodeFowState, List<SectorMapFowNode>>
            {
                {SectorMapNodeFowState.TerraIncognita, new List<SectorMapFowNode>()},
                {SectorMapNodeFowState.Explored, new List<SectorMapFowNode>()},
                {SectorMapNodeFowState.Observing, new List<SectorMapFowNode>()}
            };
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
                throw new InvalidOperationException(
                    $"Добавляются узлы тумана войны, перекрывающие существующик узлы {string.Join(",", overlapedNodes.Select(x => x.Node))}.");
            }

            foreach (var node in nodes)
            {
                _nodes[node.Node] = node;

                var targetList = _sectorNodeHash[node.State];
                targetList.Add(node);
            }
        }

        public void ChangeNodeState(SectorMapFowNode node, SectorMapNodeFowState targetState)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var sourceList = _sectorNodeHash[node.State];
            sourceList.Remove(node);

            var targetList = _sectorNodeHash[targetState];
            targetList.Add(node);

            node.ChangeState(targetState);
        }

        public IEnumerable<SectorMapFowNode> GetFowNodeByState(SectorMapNodeFowState targetState)
        {
            var targetList = _sectorNodeHash[targetState];
            return targetList;
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