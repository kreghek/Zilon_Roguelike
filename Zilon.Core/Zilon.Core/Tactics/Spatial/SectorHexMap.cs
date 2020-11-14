using Zilon.Core.Common;
using Zilon.Core.Graphs;
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
            Transitions = new Dictionary<IGraphNode, RoomTransition>();
        }

        /// <summary>
        ///     Узлы карты, приведённые к <see cref="HexNode" />.
        /// </summary>
        public IEnumerable<HexNode> HexNodes => Nodes.Cast<HexNode>();

        /// <inheritdoc />
        public Dictionary<IGraphNode, RoomTransition> Transitions { get; }

        /// <summary>
        ///     Проверяет, доступен ли целевой узел из стартового узла.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой проверяемый узел.</param>
        /// <returns>
        ///     Возвращает true, если узел доступен. Иначе, false.
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

            HexNode targetHexNode = (HexNode)targetNode;
            HexNode currentHexNode = (HexNode)currentNode;

            CubeCoords[] line = CubeCoordsHelper.CubeDrawLine(currentHexNode.CubeCoords, targetHexNode.CubeCoords);

            for (var i = 1; i < line.Length; i++)
            {
                CubeCoords prevPoint = line[i - 1];
                CubeCoords testPoint = line[i];

                OffsetCoords prevOffsetCoords = HexHelper.ConvertToOffset(prevPoint);
                OffsetCoords testOffsetCoords = HexHelper.ConvertToOffset(testPoint);

                HexNode prevNode = GetByCoords(prevOffsetCoords.X, prevOffsetCoords.Y);

                if (prevNode == null)
                {
                    return false;
                }

                HexNode testNode = GetByCoords(testOffsetCoords.X, testOffsetCoords.Y);

                if (testNode == null)
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