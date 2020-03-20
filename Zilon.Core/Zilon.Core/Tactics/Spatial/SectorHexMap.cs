using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Graphs;

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
            Transitions = new Dictionary<IGraphNode, SectorTransition>();
        }

        /// <inheritdoc/>
        public Dictionary<IGraphNode, RoomTransition> Transitions { get; }

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
        public bool TargetIsOnLine(IGraphNode currentNode, IGraphNode targetNode)
        {
            if (currentNode is null)
            {
                throw new System.ArgumentNullException(nameof(currentNode));
            }

            if (targetNode is null)
            {
                throw new System.ArgumentNullException(nameof(targetNode));
            }

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
    }
}
