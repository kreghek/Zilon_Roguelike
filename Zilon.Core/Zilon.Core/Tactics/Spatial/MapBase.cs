using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация карты.
    /// </summary>
    public abstract class MapBase : IMap
    {
        private readonly IDictionary<IMapNode, IList<IPassMapBlocker>> _nodeBlockers =
            new Dictionary<IMapNode, IList<IPassMapBlocker>>();


        public IList<IMapNode> Nodes { get; }
        public IList<IEdge> Edges { get; }

        protected MapBase()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();

            _nodeBlockers = new Dictionary<IMapNode, IList<IPassMapBlocker>>();
        }

        public bool IsPositionAvailableFor(IMapNode targetNode, IActor actor)
        {
            if (!_nodeBlockers.TryGetValue(targetNode, out IList<IPassMapBlocker> blockers))
            {
                return true;
            }

            if (!blockers.Any(x=>x != actor))
            {
                return true;
            }

            return false;
        }

        public void ReleaseNode(IMapNode node, IPassMapBlocker blocker)
        {
            if (!_nodeBlockers.TryGetValue(node, out IList<IPassMapBlocker> blockers))
            {
                throw new InvalidOperationException($"Попытка освободить узел {node}, который не заблокирован.");
            }

            if (!blockers.Contains(blocker))
            {
                throw new InvalidOperationException($"Попытка освободить узел {node}, который не заблокирован блокировщиком {blocker}.");
            }

            blockers.Remove(blocker);
        }

        public void HoldNode(IMapNode node, IPassMapBlocker blocker)
        {
            if (!_nodeBlockers.TryGetValue(node, out IList<IPassMapBlocker> blockers))
            {
                blockers = new List<IPassMapBlocker>(1);
                _nodeBlockers.Add(node, blockers);
            }

            blockers.Add(blocker);
        }
    }
}
