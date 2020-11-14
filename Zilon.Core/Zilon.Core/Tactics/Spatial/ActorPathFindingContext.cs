using System;
using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация контекста поиска пути.
    /// </summary>
    public class ActorPathFindingContext : IAstarContext
    {
        private readonly ISectorMap _map;

        public ActorPathFindingContext(IActor actor, ISectorMap map, IGraphNode targetNode)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
        }

        public IActor Actor { get; }

        public IGraphNode TargetNode { get; }

        public int GetDistanceBetween(IGraphNode current, IGraphNode target)
        {
            return _map.DistanceBetween(current, target);
        }

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
        private IEnumerable<IGraphNode> GetAvailableNeighbors(IGraphNode current, IMap map)
        {
            var neighbors = map.GetNext(current);

            foreach (var testedNeighbor in neighbors)
            {
                var isNotAvailable = !IsNodeAvailableForActor(map, testedNeighbor);
                if (isNotAvailable)
                {
                    continue;
                }

                yield return testedNeighbor;
            }
        }

        private bool IsNodeAvailableForActor(IMap map, IGraphNode testedNeighbor)
        {
            var actorSize = Actor.Person.PhysicalSize;
            if (actorSize == PhysicalSize.Size1)
            {
                return IsNodeAvailableForSmallActor(map, testedNeighbor);
            }

            if (actorSize == PhysicalSize.Size7)
            {
                return IsNodeAvailableForNormalActor(map, testedNeighbor);
            }

            throw new InvalidOperationException($"Размер {actorSize} не обрабатывается.");
        }

        private bool IsNodeAvailableForSmallActor(IMap map, IGraphNode testedNeighbor)
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

        private bool IsNodeAvailableForNormalActor(IMap map, IGraphNode testedNeighbor)
        {
            if (TargetNode == testedNeighbor)
            {
                return true;
            }

            var centerIsAvailble = map.IsPositionAvailableFor(testedNeighbor, Actor);
            var borderIsAvailable = BorderIsAvailabe(testedNeighbor, Actor, map);

            if (centerIsAvailble && borderIsAvailable)
            {
                return true;
            }

            return false;
        }

        private static bool BorderIsAvailabe(IGraphNode testedNeighbor, IActor actor, IMap map)
        {
            var borders = map.GetNext(testedNeighbor);
            foreach (var node in borders)
            {
                var isNodeAvailable = map.IsPositionAvailableFor(node, actor);
                if (!isNodeAvailable)
                {
                    return false;
                }
            }

            return true;
        }
    }
}