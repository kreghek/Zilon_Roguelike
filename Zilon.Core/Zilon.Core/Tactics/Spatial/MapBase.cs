using System.Collections.Generic;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация карты.
    /// </summary>
    public abstract class MapBase: IMap
    {

        public List<IMapNode> Nodes { get; }
        public List<IEdge> Edges { get; }

        protected MapBase()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();
        }

        public bool IsPositionAvailableFor(IMapNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(IMapNode node, Actor actor)
        {
            // Ещё нет блокировки ячейки
        }

        public void HoldNode(IMapNode node, Actor actor)
        {
            // Ещё нет блокировки ячейки
        }
    }
}
