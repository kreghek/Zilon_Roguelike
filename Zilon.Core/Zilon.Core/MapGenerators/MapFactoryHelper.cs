using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Common;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Общий вспомогательный класс для фабрик карты.
    /// </summary>
    public static class MapFactoryHelper
    {
        /// <summary>
        /// Создание переходов на основе схемы.
        /// </summary>
        /// <param name="sectorNode"> Схема сектора. </param>
        /// <returns> Набор объектов переходов. </returns>
        public static IEnumerable<RoomTransition> CreateTransitions(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            if (sectorNode.State != SectorNodeState.SchemeKnown)
            {
                throw new ArgumentException("Узел сектора должен быть материализован", nameof(sectorNode));
            }

            var next = sectorNode.Biome.GetNext(sectorNode);

            return next.Select(node => new RoomTransition(node as ISectorNode));
        }

        public static Matrix<bool> ResizeMatrixTo7(Matrix<bool> matrix)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            var resizedMatrix = matrix.CreateMatrixWithMargins(1, 1);
            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    if (matrix[x, y])
                    {
                        var neighbors = HexHelper.GetNeighbors(x + 1, y + 1);
                        foreach (var neightbor in neighbors)
                        {
                            var resizedX = neightbor.X;
                            var resizedY = neightbor.Y;
                            resizedMatrix[resizedX, resizedY] = true;
                        }
                    }
                }
            }

            return resizedMatrix;
        }

        public static bool IsAvailableFor7(Matrix<bool> matrix, OffsetCoords coords)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (!IsAvailableFor(matrix, coords))
            {
                return false;
            }

            var neighbors = HexHelper.GetNeighbors(coords.X, coords.Y);
            foreach (var neightbor in neighbors)
            {
                if (!matrix.IsIn(neightbor.X, neightbor.Y))
                {
                    return false;
                }

                if (!matrix[neightbor.X, neightbor.Y])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAvailableFor(Matrix<bool> matrix, OffsetCoords coords)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (!matrix[coords.X, coords.Y])
            {
                return false;
            }

            return true;
        }
    }
}