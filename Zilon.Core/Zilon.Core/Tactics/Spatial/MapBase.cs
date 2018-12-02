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
        private readonly IDictionary<IMapNode, IList<IPassMapBlocker>> _nodeBlockers;

        public IList<IMapNode> Nodes { get; }
        public IList<MapRegion> Regions { get; }
        public MapRegion StartRegion { get; set; }
        public MapRegion ExitRegion { get; set; }
        public IMapNode[] StartNodes { get; set; }
        public IMapNode[] ExitNodes { get; set; }

        protected MapBase()
        {
            Nodes = new List<IMapNode>();
            Regions = new List<MapRegion>();

            _nodeBlockers = new Dictionary<IMapNode, IList<IPassMapBlocker>>();
        }

        public bool IsPositionAvailableFor(IMapNode targetNode, IActor actor)
        {
            if (!_nodeBlockers.TryGetValue(targetNode, out IList<IPassMapBlocker> blockers))
            {
                return true;
            }

            if (blockers.All(x => x == actor))
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

        public abstract IEnumerable<IMapNode> GetNext(IMapNode node);

        public void AddEdge(IMapNode node1, IMapNode node2)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdge(IMapNode node1, IMapNode node2)
        {
            throw new NotImplementedException();
        }
    }
}
