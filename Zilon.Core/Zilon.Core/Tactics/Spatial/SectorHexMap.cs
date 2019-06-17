using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorHexMap : HexMap, ISectorMap
    {
        private const int SegmentSize = 200;

        public SectorHexMap() : this(SegmentSize)
        {
        }

        public SectorHexMap(int segmentSize) : base(segmentSize)
        {
            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }

        public Dictionary<IMapNode, RoomTransition> Transitions { get; }

        /// <summary>
        /// Узлы карты, приведённые к <see cref="HexNode"/>.
        /// </summary>
        public IEnumerable<HexNode> HexNodes
        {
            get
            {
                return Nodes.Cast<HexNode>();
            }
        }

        /// <summary>
        /// Проверяет, доступен ли целевой узел из стартового узла.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой проверяемый узел.</param>
        /// <returns>
        /// Возвращает true, если узел доступен. Иначе, false.
        /// </returns>
        public bool TargetIsOnLine(IMapNode currentNode, IMapNode targetNode)
        {
            var targetHexNode = (HexNode)targetNode;
            var currentHexNode = (HexNode)currentNode;

            var line = CubeCoordsHelper.CubeDrawLine(currentHexNode.CubeCoords, targetHexNode.CubeCoords);

            for (var i = 1; i < line.Length; i++)
            {
                var prevPoint = line[i - 1];
                var testPoint = line[i];

                var prevNode = HexNodes
                    .SingleOrDefault(x => x.CubeCoords == prevPoint);

                if (prevNode == null)
                {
                    return false;
                }

                var testNode = HexNodes
                    .SingleOrDefault(x => x.CubeCoords == testPoint);

                if (testNode == null)
                {
                    return false;
                }

                if (testNode.IsObstacle)
                {
                    return false;
                }

                var hasNext = GetNext(prevNode).Contains(testNode);
                if (!hasNext)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Distances the between.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targetNode">The target node.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int DistanceBetween(IMapNode currentNode, IMapNode targetNode)
        {
            var actorHexNode = (HexNode)currentNode;
            var containerHexNode = (HexNode)targetNode;

            var actorCoords = actorHexNode.CubeCoords;
            var containerCoords = containerHexNode.CubeCoords;

            var distance = actorCoords.DistanceTo(containerCoords);

            return distance;
        }
    }
}
