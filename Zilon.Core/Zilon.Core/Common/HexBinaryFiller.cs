using System;
using System.Collections.Generic;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с заливкой
    /// в поле шестиугольников.
    /// </summary>
    public static class HexBinaryFiller
    {
        /// <summary>
        /// Выполняет заливку области в поле шестиугольников.
        /// </summary>
        /// <param name="matrix"> Поле шестиугольников. Будут заливаться ячейки со сзначением <b>true</b>. </param>
        /// <param name="point"> Точка, с которой начинается заливка. Должна указывать на ячейку со значением <b>true</b>. </param>
        /// <returns> Возвращает точки, которые были залиты. </returns>
        public static IEnumerable<OffsetCoords> FloodFill(Matrix<bool> matrix, OffsetCoords point)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            var snapshotCellmap = (bool[,])matrix.Items.Clone();

            var regionPoints = new List<OffsetCoords>();

            var openPoints = new Stack<OffsetCoords>();
            openPoints.Push(point);

            while (openPoints.Count > 0)
            {
                var currentCell = openPoints.Pop();

                var isInBound = IsInBounds(currentCell, matrix.Width, matrix.Height);

                if (!isInBound)
                {
                    // Если текущая точка указывает за край карты, то не пытаемся её заливать.
                    // Пропускаем.
                    continue;
                }

                if (!snapshotCellmap[currentCell.X, currentCell.Y])
                {
                    // Заливаем только живые клетки.
                    // Мертвые клетки являются границей, они не попадают в заливку.
                    continue;
                }

                regionPoints.Add(currentCell);
                snapshotCellmap[currentCell.X, currentCell.Y] = false;

                var cubeCoords = HexHelper.ConvertToCube(currentCell);
                var clockwiseOffsets = HexHelper.GetOffsetClockwise();

                foreach (var offset in clockwiseOffsets)
                {
                    var neighbourCubeCoords = cubeCoords + offset;

                    var neighbourCoords = HexHelper.ConvertToOffset(neighbourCubeCoords);

                    if (!openPoints.Contains(neighbourCoords))
                    {
                        openPoints.Push(neighbourCoords);
                    }
                }
            }

            return regionPoints;
        }

        /// <summary>
        /// Выполняет заливку области в поле шестиугольников с учётом размера в 7 узлов.
        /// </summary>
        /// <param name="matrix"> Поле шестиугольников. Будут заливаться ячейки со сзначением <b>true</b>. </param>
        /// <param name="point"> Точка, с которой начинается заливка. Должна указывать на ячейку со значением <b>true</b>. </param>
        /// <returns> Возвращает точки, которые были залиты. </returns>
        public static IEnumerable<OffsetCoords> FloodFill7(Matrix<bool> matrix, OffsetCoords point)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            var snapshotCellmap = (bool[,])matrix.Items.Clone();

            var regionPoints = new List<OffsetCoords>();

            var openPoints = new Stack<OffsetCoords>();
            openPoints.Push(point);

            while (openPoints.Count > 0)
            {
                var currentCell = openPoints.Pop();

                var isInBound = IsInBounds(currentCell, matrix.Width, matrix.Height);

                if (!isInBound)
                {
                    // Если текущая точка указывает за край карты, то не пытаемся её заливать.
                    // Пропускаем.
                    continue;
                }

                if (!snapshotCellmap[currentCell.X, currentCell.Y])
                {
                    // Заливаем только живые клетки.
                    // Мертвые клетки являются границей, они не попадают в заливку.
                    continue;
                }

                regionPoints.Add(currentCell);
                snapshotCellmap[currentCell.X, currentCell.Y] = false;

                var cubeCoords = HexHelper.ConvertToCube(currentCell);
                var clockwiseOffsets = HexHelper.GetOffsetClockwise();

                foreach (var offset in clockwiseOffsets)
                {
                    var neighbourCubeCoords = cubeCoords + offset;

                    var neighbourCoords = HexHelper.ConvertToOffset(neighbourCubeCoords);

                    if (!openPoints.Contains(neighbourCoords))
                    {
                        var isAvailbleFor7 = CheckAvailableFor7(neighbourCoords, matrix);
                        if (isAvailbleFor7)
                        {
                            openPoints.Push(neighbourCoords);
                        }
                    }
                }
            }

            return regionPoints;
        }

        private static bool CheckAvailableFor7(OffsetCoords testCoords, Matrix<bool> matrix)
        {
            var cubeCoords = HexHelper.ConvertToCube(testCoords);
            var clockwiseOffsets = HexHelper.GetOffsetClockwise();
            foreach (var offset in clockwiseOffsets)
            {
                var neighbourCubeCoords = cubeCoords + offset;

                var neighbourCoords = HexHelper.ConvertToOffset(neighbourCubeCoords);

                try
                {
                    if (!matrix.Items[neighbourCoords.X, neighbourCoords.Y])
                    {
                        return false;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsInBounds(OffsetCoords coords, int width, int height)
        {
            if (!ValueInRange(value: coords.X, min: 0, max: width - 1))
            {
                return false;
            }

            if (!ValueInRange(value: coords.Y, min: 0, max: height - 1))
            {
                return false;
            }

            return true;
        }

        private static bool ValueInRange(int value, int min, int max)
        {
            return min <= value && value <= max;
        }
    }
}
