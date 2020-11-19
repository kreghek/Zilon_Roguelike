using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Find isolated regions in matrix of bool.
    /// </summary>
    internal static class RegionFinder
    {
        /// <summary>
        /// Find all passable (true) regions.
        /// </summary>
        public static IEnumerable<RegionDraft> FindPassableRegionsFor(Matrix<bool> matrix)
        {
            var openNodesEnum = AddAllPassableNodesToOpenList(matrix);
            var openNodes = openNodesEnum.ToList();

            var regions = new List<RegionDraft>();
            while (openNodes.Any())
            {
                var openNode = openNodes.First(x => MapFactoryHelper.IsAvailableFor7(matrix, x));
                var regionCoords = FloodFillRegions(matrix, openNode);
                var region = new RegionDraft(regionCoords.ToArray());

                openNodes.RemoveAll(x => region.Contains(x));

                regions.Add(region);
            }

            return regions;
        }

        private static IEnumerable<OffsetCoords> AddAllPassableNodesToOpenList(Matrix<bool> matrix)
        {
            var openNodes = new List<OffsetCoords>();
            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    if (matrix.Items[x, y])
                    {
                        openNodes.Add(new OffsetCoords(x, y));
                    }
                }
            }

            return openNodes;
        }

        private static IEnumerable<OffsetCoords> FloodFillRegions(Matrix<bool> matrix, OffsetCoords point)
        {
            var regionPoints = HexBinaryFiller.FloodFill(
                matrix,
                point);

            // В регионе должна быть хоть одна точка - стартовая.
            // Потому что заливка начинается с выбора незалитых точек.
            // Если этот метод не будет возращать точки, то будет бесконечный цикл.
            // Это критично, поэтому выбрасываем исключение.
            if (!regionPoints.Any())
            {
                throw new InvalidOperationException("Должна быть залита хотя бы одна точка.");
            }

            return regionPoints;
        }
    }
}