using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexNode: IGraphNode
    {
        /// <summary>
        /// Уникальный идентификатор узла в рамках сектора.
        /// </summary>
        /// <remarks>
        /// Нужен только для отладки.
        /// </remarks>
        [PublicAPI]
        public int Id { get; set; }

        public int OffsetX { get; }
        public int OffsetY { get; }
        public CubeCoords CubeCoords { get; }

        public OffsetCoords Coords { get; }

        public bool IsObstacle { get; }

        public HexNode(int x, int y, bool isObstacle)
        {
            OffsetX = x;
            OffsetY = y;
            IsObstacle = isObstacle;

            CubeCoords = HexHelper.ConvertToCube(x, y);

            Coords = new OffsetCoords(x, y);
        }

        //TODO Добавить конструктор, принимающий OffsetCoords.
        // На уровне объекта хранить OffsetCoords вместо разрозненных x и y.
        public HexNode(int x, int y): this(x, y, false)
        {
        }

        public override string ToString()
        {
            return $"(X: {OffsetX}, Y: {OffsetY})";
        }
    }
}
