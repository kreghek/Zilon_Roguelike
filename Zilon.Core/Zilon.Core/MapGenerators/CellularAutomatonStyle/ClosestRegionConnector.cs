using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Connect closest map regions each with other.
    /// </summary>
    internal static class ClosestRegionConnector
    {
        public static void Connect(Matrix<bool> matrix, IEnumerable<RegionDraft> regions)
        {
            // Соединяем все регионы в единый граф.
            var openRegions = new List<RegionDraft>(regions);
            var unitedRegions = new List<RegionDraft>();

            var startRegion = openRegions[0];
            openRegions.RemoveAt(0);
            unitedRegions.Add(startRegion);

            while (openRegions.Any())
            {
                ConnectOpenRegionsToUnited(matrix, openRegions, unitedRegions);
            }
        }

        private static void ConnectOpenRegionsToUnited(Matrix<bool> matrix, List<RegionDraft> openRegions,
            List<RegionDraft> unitedRegions)
        {
            var unitedRegionCoords = unitedRegions.SelectMany(x => x.Coords).ToArray();

            // Ищем две самые ближние точки между объединённым регионом и 
            // и всеми открытыми регионами.

            FindClosestNodesBetweenOpenAndUnited(
                openRegions,
                unitedRegionCoords,
                out OffsetCoords? currentOpenRegionCoord,
                out OffsetCoords? currentUnitedRegionCoord,
                out RegionDraft nearbyOpenRegion);

            // Если координаты, которые нужно соединить, найдены,
            // то прорываем тоннель.
            if (nearbyOpenRegion != null
                && currentOpenRegionCoord != null
                && currentUnitedRegionCoord != null)
            {
                var openCubeCoord = HexHelper.ConvertToCube(currentOpenRegionCoord.Value);
                var unitedCubeCoord = HexHelper.ConvertToCube(currentUnitedRegionCoord.Value);

                DrawLineBetweenNodes(matrix, openCubeCoord, unitedCubeCoord);

                openRegions.Remove(nearbyOpenRegion);
                unitedRegions.Add(nearbyOpenRegion);
            }
        }

        private static void FindClosestNodesBetweenOpenAndUnited(List<RegionDraft> openRegions,
            OffsetCoords[] unitedRegionCoords, out OffsetCoords? currentOpenRegionCoord,
            out OffsetCoords? currentUnitedRegionCoord, out RegionDraft nearbyOpenRegion)
        {
            var currentDistance = int.MaxValue;
            currentOpenRegionCoord = null;
            currentUnitedRegionCoord = null;
            nearbyOpenRegion = null;
            foreach (var currentOpenRegion in openRegions)
            {
                foreach (var openRegionCoord in currentOpenRegion.Coords)
                {
                    var openCubeCoord = HexHelper.ConvertToCube(openRegionCoord);

                    foreach (var unitedRegionCoord in unitedRegionCoords)
                    {
                        var unitedCubeCoord = HexHelper.ConvertToCube(unitedRegionCoord);
                        var distance = openCubeCoord.DistanceTo(unitedCubeCoord);

                        if (distance < currentDistance)
                        {
                            currentDistance = distance;
                            currentOpenRegionCoord = openRegionCoord;
                            currentUnitedRegionCoord = unitedRegionCoord;
                            nearbyOpenRegion = currentOpenRegion;
                        }
                    }
                }
            }
        }

        private static void DrawLineBetweenNodes(Matrix<bool> matrix, CubeCoords openCubeCoord,
            CubeCoords unitedCubeCoord)
        {
            var line = CubeCoordsHelper.CubeDrawLine(openCubeCoord, unitedCubeCoord);
            foreach (var lineItem in line)
            {
                var offsetCoords = HexHelper.ConvertToOffset(lineItem);
                matrix[offsetCoords.X, offsetCoords.Y] = true;

                // Коридоры должны быть размером в Size7.
                // Поэтому вокруг каждой точки прорываем соседей.

                var neighborCoords = HexHelper.GetNeighbors(offsetCoords.X, offsetCoords.Y);
                foreach (var coords in neighborCoords)
                {
                    matrix[coords.X, coords.Y] = true;
                }
            }
        }
    }
}