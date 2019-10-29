namespace Zilon.Core.Common
{
    public static class HexHelper
    {
        public static CubeCoords ConvertToCube(int offsetX, int offsetY)
        {
            var x = offsetX - (offsetY - (offsetY & 1)) / 2;
            var z = offsetY;
            var y = -x - z;

            return new CubeCoords(x, y, z);
        }

        public static CubeCoords ConvertToCube(OffsetCoords offsetCoords)
        {
            return ConvertToCube(offsetCoords.X, offsetCoords.Y);
        }

        public static OffsetCoords ConvertToOffset(CubeCoords cube)
        {
            var col = cube.X + (cube.Z - (cube.Z & 1)) / 2;
            var row = cube.Z;
            return new OffsetCoords(col, row);
        }

        public static float[] ConvertToWorld(int offsetX, int offsetY)
        {
            var rowOffset = offsetY % 2 == 0 ? 0 : 0.5f;
            return new[] {
                offsetX + rowOffset,
                offsetY * 3f / 4
            };
        }

        /// <summary>
        /// Возвращает смещения по часовой стрелке.
        /// </summary>
        /// <returns> Массив со смещениями. </returns>
        /// <remarks>
        /// Основано на статье https://www.redblobgames.com/grids/hexagons/.
        /// </remarks>
        public static CubeCoords[] GetOffsetClockwise()
        {
            var offsets = new[]
            {
                new CubeCoords(-1, +1, 0), new CubeCoords(-1, 0, +1), new CubeCoords(0, -1, +1),
                new CubeCoords(+1, -1, 0),new CubeCoords(+1, 0, -1),new CubeCoords(0, +1, -1)
            };

            return offsets;
        }

        /// <summary>
        /// Возвращает смещения диагоналей по часовой стрелке.
        /// </summary>
        /// <returns> Массив со смещениями. </returns>
        /// <remarks>
        /// Основано на статье https://www.redblobgames.com/grids/hexagons/.
        /// </remarks>
        public static CubeCoords[] GetDiagonalOffsetClockwise()
        {
            // Начинаем с верхнего левого.
            var offsets = new[]
            {
                new CubeCoords(-1, +2, -1), new CubeCoords(-2, +1, +1), new CubeCoords(-1, -1, +2),
                new CubeCoords(+1, -2, +1),new CubeCoords(+2, -1, -1),new CubeCoords(+1, +1, -2)
            };

            return offsets;
        }
    }
}
