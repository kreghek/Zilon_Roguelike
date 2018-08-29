using System.Collections.Generic;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация карты.
    /// </summary>
    public abstract class MapBase: IMap
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
            return true;
        }

        public void ReleaseNode(IMapNode node, IPassMapBlocker blocker)
        {
            // Ещё нет блокировки ячейки
        }

        public void HoldNode(IMapNode node, IPassMapBlocker blocker)
        {
            // Ещё нет блокировки ячейки
        }
    }
}
