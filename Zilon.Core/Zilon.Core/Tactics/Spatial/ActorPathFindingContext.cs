using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация контекста поиска пути.
    /// </summary>
    public class ActorPathFindingContext : IPathFindingContext
    {
        private readonly IMap _map;

        public ActorPathFindingContext(IActor actor, IMap map): this(actor, map, targetNode:null)
        {
        }

        public ActorPathFindingContext(IActor actor, IMap map, IGraphNode targetNode)
        {
            Actor = actor;
            _map = map;
            TargetNode = targetNode;
        }

        public IActor Actor { get; }

        public IGraphNode TargetNode { get; }

        public IEnumerable<IGraphNode> GetNext(IGraphNode current)
        {
            return GetAvailableNeighbors(current, _map);
        }

        /// <summary>
        /// Возвращает доступные соседние узлы карты с учётом обхода соседей по часовой стрелке.
        /// </summary>
        /// <param name="current"> Текущий узел. </param>
        /// <param name="map"> Карта, на которой проводится проверка. </param>
        /// <returns> Возвращает список соседних узлов, соединённых ребрами с текущим. </returns>
        private IGraphNode[] GetAvailableNeighbors(IGraphNode current, IMap map)
        {
            var neighbors = map.GetNext(current);

            var actualNeighbors = new List<IGraphNode>();
            foreach (var testedNeighbor in neighbors)
            {
                if (TargetNode == null)
                {
                    if (!map.IsPositionAvailableFor(testedNeighbor, Actor))
                    {
                        continue;
                    }
                }
                else
                {
                    var isNotAvailable = !IsAvailable(map, testedNeighbor);
                    if (isNotAvailable)
                    {
                        continue;
                    }
                }

                actualNeighbors.Add(testedNeighbor);
            }

            return actualNeighbors.ToArray();
        }

        private bool IsAvailable(IMap map, IGraphNode testedNeighbor)
        {
            if (TargetNode == testedNeighbor)
            {
                return true;
            }

            if (map.IsPositionAvailableFor(testedNeighbor, Actor))
            {
                return true;
            }

            return false;
        }
    }
}
