using System;
using System.Collections.Generic;
using System.Linq;

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
            return FloodFillInner(matrix, point, (nextNeighbour) => true);
        }

        /// <summary>
        /// Выполняет заливку области в поле шестиугольников с учётом размера в 7 узлов.
        /// </summary>
        /// <param name="matrix"> Поле шестиугольников. Будут заливаться ячейки со сзначением <b>true</b>. </param>
        /// <param name="point"> Точка, с которой начинается заливка. Должна указывать на ячейку со значением <b>true</b>. </param>
        /// <returns> Возвращает точки, которые были залиты. </returns>
        public static IEnumerable<OffsetCoords> FloodFill7(Matrix<bool> matrix, OffsetCoords point)
        {
            return FloodFillInner(
                matrix,
                point,
                (nextCoords) => CheckAvailableFor7(nextCoords, matrix));
        }

        /// <param name="availabilityDelegate"> In - coords on next, neightbor nodes to check if it next fron current node. </param>
        /// <returns></returns>
        public static IEnumerable<OffsetCoords> FloodFillInner(Matrix<bool> matrix, OffsetCoords point, Func<OffsetCoords, bool> availabilityDelegate)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (availabilityDelegate is null)
            {
                throw new ArgumentNullException(nameof(availabilityDelegate));
            }

            var snapshotCellMap = (bool[,])matrix.Items.Clone();

            var regionPoints = new List<OffsetCoords>();

            var openPoints = new HashSet<OffsetCoords>
            {
                point
            };

            while (openPoints.Count > 0)
            {
                var currentCell = openPoints.First();
                openPoints.Remove(currentCell);

                var isInBound = IsInBounds(currentCell, matrix.Width, matrix.Height);

                if (!isInBound)
                {
                    // Если текущая точка указывает за край карты, то не пытаемся её заливать.
                    // Пропускаем.
                    continue;
                }

                if (!snapshotCellMap[currentCell.X, currentCell.Y])
                {
                    // Заливаем только живые клетки.
                    // Мертвые клетки являются границей, они не попадают в заливку.
                    continue;
                }

                regionPoints.Add(currentCell);
                snapshotCellMap[currentCell.X, currentCell.Y] = false;

                var cubeCoords = HexHelper.ConvertToCube(currentCell);
                var clockwiseOffsets = HexHelper.GetOffsetClockwise();

                foreach (var offset in clockwiseOffsets)
                {
                    var neighbourCubeCoords = cubeCoords + offset;

                    var neighbourCoords = HexHelper.ConvertToOffset(neighbourCubeCoords);

                    if (!openPoints.Contains(neighbourCoords))
                    {
                        var isAvailable = availabilityDelegate(neighbourCoords);
                        if (isAvailable)
                        {
                            openPoints.Add(neighbourCoords);
                        }
                    }
                }
            }

            return regionPoints;
        }

        private static bool CheckAvailableFor7(OffsetCoords testCoords, Matrix<bool> matrix)
        {
            if (!matrix[testCoords.X, testCoords.Y])
            {
                return false;
            }

            var neighbours = HexHelper.GetNeighbors(testCoords.X, testCoords.Y);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.X >= matrix.Width || neighbour.Y >= matrix.Height)
                {
                    return false;
                }

                if (!matrix.Items[neighbour.X, neighbour.Y])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsInBounds(OffsetCoords coords, int width, int height)
        {
            // ReSharper disable once ArgumentsStyleNamedExpression
            // ReSharper disable once ArgumentsStyleLiteral
            // ReSharper disable once ArgumentsStyleOther
            // There used named argument to improve readable of code.
            if (!ValueInRange(value: coords.X, min: 0, max: width - 1))
            {
                return false;
            }

            // ReSharper disable once ArgumentsStyleNamedExpression
            // ReSharper disable once ArgumentsStyleLiteral
            // ReSharper disable once ArgumentsStyleOther
            // There used named argument to improve readable of code.
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