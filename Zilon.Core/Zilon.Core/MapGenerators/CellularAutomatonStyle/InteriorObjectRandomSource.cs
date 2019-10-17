using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    public class InteriorObjectRandomSource : IInteriorObjectRandomSource
    {
        private readonly IDice _dice;

        public InteriorObjectRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public InteriorObjectMeta[] RollInteriorObjects(OffsetCoords[] regionDraftCoords)
        {
            var availableCoords = GetAvailableCoords(regionDraftCoords);
            var openCoords = new List<OffsetCoords>(availableCoords);
            if (!openCoords.Any())
            {
                return new InteriorObjectMeta[0];
            }

            var count = openCoords.Count / 4;

            var rolledCoordsList = new List<InteriorObjectMeta>();
            for (var i = 0; i < count; i++)
            {
                var rolledCoords = _dice.RollFromList(openCoords.ToArray());
                var interior = new InteriorObjectMeta(rolledCoords);
                rolledCoordsList.Add(interior);

                openCoords.Remove(rolledCoords);

                if (!openCoords.Any())
                {
                    break;
                }
            }

            return rolledCoordsList.ToArray();
        }

        private static IEnumerable<OffsetCoords> GetAvailableCoords(OffsetCoords[] regionDraftCoords)
        {
            var coordHash = new HashSet<OffsetCoords>(regionDraftCoords);

            var neighborCubeOffsets = HexHelper.GetOffsetClockwise();
            foreach (var coords in regionDraftCoords)
            {
                var cube = HexHelper.ConvertToCube(coords);

                var isValid = HasAllHeighbors(coordHash, neighborCubeOffsets, cube);

                if (isValid)
                {
                    yield return coords;
                }
            }
        }

        ///<summary>
        /// Для препятсвий выбираются только те узлы, для которых есть все соседи.
        ///</summary> 
        private static bool HasAllHeighbors(HashSet<OffsetCoords> coordHash, CubeCoords[] neighborCubeOffsets, CubeCoords cube)
        {
            foreach (var neighborCubeOffset in neighborCubeOffsets)
            {
                var neighborCube = cube + neighborCubeOffset;
                var neighborOffsetCoords = HexHelper.ConvertToOffset(neighborCube);
                if (!coordHash.Contains(neighborOffsetCoords))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
