using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Базовая реализация источника рандома при работе с элементами интерьера.
    /// </summary>
    public class InteriorObjectRandomSource : IInteriorObjectRandomSource
    {
        private readonly IDice _dice;

        /// <summary>
        /// Кнструктор.
        /// </summary>
        /// <param name="dice"> Кость, используемая, как источник рандома. </param>
        public InteriorObjectRandomSource(IDice dice)
        {
            _dice = dice;
        }

        /// <summary>
        /// Случайный выбор координат для размещения элемента интерьера.
        /// </summary>
        /// <param name="regionDraftCoords"> Координаты региона, среди которых можно выбирать позиции элементов интерьера. </param>
        /// <returns> Возвращает набор метаданных об элементах интерьера. </returns>
        public InteriorObjectMeta[] RollInteriorObjects(OffsetCoords[] regionDraftCoords)
        {
            if (regionDraftCoords is null)
            {
                throw new System.ArgumentNullException(nameof(regionDraftCoords));
            }

           // var availableCoords = GetAvailableCoords(regionDraftCoords);
            var openCoords = new List<OffsetCoords>(regionDraftCoords);
            if (!openCoords.Any())
            {
                return System.Array.Empty<InteriorObjectMeta>();
            }

            var count = openCoords.Count / 4;

            var resultMetaList = new List<InteriorObjectMeta>();
            for (var i = 0; i < count; i++)
            {
                // Выполняем 3 попытки на размещение элемента декора.
                // 2 из них могут быть неудачными (например, элемент
                // декора перекрывает проход к какой-либо доступной ячейке).
                for (var tryIndex = 0; tryIndex < 3; tryIndex++)
                {
                    var isValid = TryRollInteriorCoord(openCoords, regionDraftCoords, out var rolledCoord);

                    // Вне зависимости от корректности rolledCoord
                    // убираем его из открытых координат.

                    // В случае если он перекрывал одну из доступных ячеек.
                    // Позже такая координата может стать корректной (например,
                    // когда будет полукольцо из препятсвий).
                    // Но мы пренебрегаем этим в целях производительности.
                    openCoords.Remove(rolledCoord);

                    if (isValid)
                    {
                        var interiorMeta = new InteriorObjectMeta(rolledCoord);
                        resultMetaList.Add(interiorMeta);
                        break;
                    }

                    if (!openCoords.Any())
                    {
                        break;
                    }
                }

                if (!openCoords.Any())
                {
                    break;
                }
            }

            return resultMetaList.ToArray();
        }

        private bool TryRollInteriorCoord(IList<OffsetCoords> openCoords,
            IEnumerable<OffsetCoords> passableRegionCoords,
            out OffsetCoords rolledCoords)
        {
            var supposedRolledCoords = _dice.RollFromList(openCoords);

            // Проверяем, что элемент декора не перекрывает проход.
            var isNotBlockPass = CheckMapPassable(passableRegionCoords, supposedRolledCoords);
            if (!isNotBlockPass)
            {
                rolledCoords = supposedRolledCoords;
                return false;
            }

            rolledCoords = supposedRolledCoords;
            return true;
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

        private bool CheckMapPassable(IEnumerable<OffsetCoords> currentCoords, OffsetCoords targetCoords)
        {
            var matrix = new Matrix<bool>(1000, 1000);
            foreach (var coords in currentCoords)
            {
                var x = coords.X;
                var y = coords.Y;
                matrix.Items[x, y] = true;
            }

            // Закрываем проверяемый узел
            matrix.Items[targetCoords.X, targetCoords.Y] = false;

            var startPoint = currentCoords.First();
            var floodPoints = HexBinaryFiller.FloodFill(matrix, startPoint);

            foreach (var point in floodPoints)
            {
                matrix.Items[point.X, point.Y] = false;
            }

            foreach (var node in currentCoords)
            {
                var x = node.X;
                var y = node.Y;
                if (matrix.Items[x, y])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
