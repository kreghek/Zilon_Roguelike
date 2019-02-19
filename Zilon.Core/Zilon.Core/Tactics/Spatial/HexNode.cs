using JetBrains.Annotations;

using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexNode: IMapNode
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

        public HexNode(int x, int y)
        {
            OffsetX = x;
            OffsetY = y;
            CubeCoords = HexHelper.ConvertToCube(x, y);
        }

        public override string ToString()
        {
            return $"(X: {OffsetX}, Y: {OffsetY})";
        }
    }
}
