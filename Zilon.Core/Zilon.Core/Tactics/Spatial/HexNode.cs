using Zilon.Core.Common;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexNode : IGraphNode
    {
        public HexNode(int x, int y) : this(new OffsetCoords(x, y))
        {
        }

        public HexNode(OffsetCoords coords)
        {
            OffsetCoords = coords;

            var x = coords.X;
            var y = coords.Y;

            CubeCoords = HexHelper.ConvertToCube(x, y);
        }

        /// <summary>
        ///     Уникальный идентификатор узла в рамках сектора.
        /// </summary>
        /// <remarks>
        ///     Нужен только для отладки.
        /// </remarks>
        [PublicAPI]
        public int Id { get; set; }

        public OffsetCoords OffsetCoords { get; }

        public CubeCoords CubeCoords { get; }

        public override string ToString()
        {
            return $"(X: {OffsetCoords.X}, Y: {OffsetCoords.Y})";
        }
    }
}