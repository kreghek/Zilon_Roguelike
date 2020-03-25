using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Фабрика карты на основе клеточного автомата.
    /// </summary>
    public sealed class CellularAutomatonMapFactory : IMapFactory
    {
        private const int SIMULATION_COUNT = 2;
        private const int DEATH_LIMIT = 4;
        private const int BIRTH_LIMIT = 6;
        private const int RETRY_LIMIT = 3;

        private readonly IDice _dice;
        private readonly IInteriorObjectRandomSource _interiorObjectRandomSource;

        /// <summary>
        /// Конструктор фабрики.
        /// </summary>
        /// <param name="dice"> Кость для рандома. </param>
        /// <param name="interiorObjectRandomSource"> Источник рандома для элементов интерьера. </param>
        public CellularAutomatonMapFactory(IDice dice, IInteriorObjectRandomSource interiorObjectRandomSource)
        {
            _dice = dice;
            _interiorObjectRandomSource = interiorObjectRandomSource;
        }

        /// <inheritdoc/>
        public Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            var transitions = generationOptions.Transitions;

            var cellularAutomatonOptions = (ISectorCellularAutomataMapFactoryOptionsSubScheme)generationOptions.OptionsSubScheme;
            if (cellularAutomatonOptions == null)
            {
                throw new ArgumentException($"Для {nameof(generationOptions)} не задано {nameof(ISectorSubScheme.MapGeneratorOptions)} равно null.");
            }

            var matrixWidth = cellularAutomatonOptions.MapWidth;
            var matrixHeight = cellularAutomatonOptions.MapHeight;

            var chanceToStartAlive = cellularAutomatonOptions.ChanceToStartAlive;

            var targetRegionDraftCount = transitions.Count() + 1;
            var createResult = CreateInner(
                targetRegionDraftCount,
                chanceToStartAlive,
                matrixWidth,
                matrixHeight);

            var matrix = createResult.Matrix;
            var draftRegions = createResult.Regions;

            var map = CreateSectorMap(matrix, draftRegions, transitions);

            return Task.FromResult(map);
        }

        //TODO Придумать название метода получше
        private CreateMatrixResult CreateInner(int targetRegionDraftCount, int chanceToStartAlive, int matrixWidth, int matrixHeight)
        {
            var matrix = new Matrix<bool>(matrixWidth, matrixHeight);

            for (var retry = 0; retry < RETRY_LIMIT; retry++)
            {
                InitStartAliveMatrix(matrix, chanceToStartAlive);

                // Несколько шагов симуляции
                for (var i = 0; i < SIMULATION_COUNT; i++)
                {
                    var newMap = DoSimulationStep(matrix);
                    matrix = new Matrix<bool>(newMap, matrix.Width, matrix.Height);
                }

                // Растягиваем матрицу на 4, чтобы в единичную ячейку матрицы клеточного автомата
                // мог помещаться персонаж размером в 7 узлов.
                // Отключено, потому что сейчас слишком долгая генерация + туман войны на больших плошадях тормозит
                //const int SCALE_FACTOR = 4;
                //var maxtrixScales = MatrixHelper.CreateScaledMatrix(matrix, SCALE_FACTOR);

                var matrixWithMargins = matrix.CreateMatrixWithVerticalMargins();

                RegionDraft[] draftRegions;
                try
                {
                    draftRegions = MakeUnitedRegions(matrixWithMargins);
                }
                catch (CellularAutomatonException)
                {
                    // Это означает, что при текущих стартовых данных невозможно создать подходящую карту.
                    // Запускаем следующую итерацю.
                    continue;
                }

                // Обрабатываем ситуацию, когда на карте тегионов меньше, чем переходов.
                // На карте должно быть минимум столько регионов, сколько переходов.
                // +1 - это регион старта.
                if (draftRegions.Length < targetRegionDraftCount)
                {
                    try
                    {
                        var splittedDraftRegions = SplitRegionsForTransitions(draftRegions, targetRegionDraftCount);

                        // Разделение успешно выполнено.
                        // Пропускаем карту дальше.
                        var result = new CreateMatrixResult(matrixWithMargins, splittedDraftRegions);
                        return result;
                    }
                    catch (CellularAutomatonException)
                    {
                        // Это означает, что при текущих стартовых данных невозможно создать подходящую карту.
                        // Запускаем следующую итерацю.
                        continue;
                    }
                }
                else
                {
                    var result = new CreateMatrixResult(matrixWithMargins, draftRegions);
                    return result;
                }
            }

            // Если цикл закончился, значит вышел лимит попыток.
            throw new InvalidOperationException("Не удалось создать карту за предельное число попыток.");
        }

        private ISectorMap CreateSectorMap(Matrix<bool> matrix, RegionDraft[] draftRegions, IEnumerable<RoomTransition> transitions)
        {
            // Создание графа карты сектора на основе карты клеточного автомата.
            ISectorMap map = new SectorHexMap();
            var interiorHashset = GenerateInteriorObjects(draftRegions);

            var regionIdCounter = 1;
            foreach (var draftRegion in draftRegions)
            {
                var regionNodeList = new List<IGraphNode>();

                foreach (var coord in draftRegion.Coords)
                {
                    var isObstacle = interiorHashset.ContainsKey(coord);

                    var node = new HexNode(coord.X, coord.Y, isObstacle);
                    map.AddNode(node);

                    regionNodeList.Add(node);
                }

                var region = new MapRegion(regionIdCounter, regionNodeList.ToArray());

                map.Regions.Add(region);

                regionIdCounter++;
            }

            // Добавляем узлы каридоров.
            CreateCorridors(matrix, draftRegions, map);

            // Размещаем переходы и отмечаем стартовую комнату.
            // Общее описание: стараемся размещать переходы в самых маленьких комнатах.
            // Для этого сортируем все комнаты по размеру.
            // Первую занимаем под старт.
            // Последующие - это переходы.

            var regionOrderedBySize = map.Regions.OrderBy(x => x.Nodes.Count()).ToArray();

            if (regionOrderedBySize.Any())
            {
                var startRegion = regionOrderedBySize.First();
                startRegion.IsStart = true;

                var transitionArray = transitions.ToArray();
                // Пропускаем 1, потому что 1 занять стартом.
                var trasitionRegionDrafts = regionOrderedBySize.Skip(1).ToArray();

                Debug.Assert(trasitionRegionDrafts.Length >= transitionArray.Length,
                    "Должно быть достаточно регионов для размещения всех переходов.");

                for (var i = 0; i < transitionArray.Length; i++)
                {
                    var transitionRegion = trasitionRegionDrafts[i];

                    var transition = transitionArray[i];

                    var transitionNode = transitionRegion.Nodes.First();

                    map.Transitions.Add(transitionNode, transition);

                    if (transition.SectorNode == null)
                    {
                        transitionRegion.IsOut = true;
                    }

                    transitionRegion.ExitNodes = (from regionNode in transitionRegion.Nodes
                                                  where map.Transitions.Keys.Contains(regionNode)
                                                  select regionNode).ToArray();
                }
            }

            return map;
        }

        private Dictionary<OffsetCoords, InteriorObjectMeta> GenerateInteriorObjects(RegionDraft[] draftRegions)
        {
            var interiorHashset = new Dictionary<OffsetCoords, InteriorObjectMeta>();
            foreach (var draftRegion in draftRegions)
            {
                var allCoords = draftRegion.Coords;
                var interiorMetas = _interiorObjectRandomSource.RollInteriorObjects(allCoords);

                foreach (var meta in interiorMetas)
                {
                    interiorHashset.Add(meta.Coords, meta);
                }
            }

            return interiorHashset;
        }

        private void InitStartAliveMatrix(Matrix<bool> matrix, int _chanceToStartAlive)
        {
            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    var blockRoll = _dice.Roll(100);
                    if (blockRoll < _chanceToStartAlive)
                    {
                        matrix.Items[x, y] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Метод отделяет от существующих регионов ячйки таким образом,
        /// чтобы суммарно на карте число регионов равнялось числу переходов + 1 (за стартовый).
        /// Сейчас все отщеплённые регионы в первую отщепляются от произвольных.
        /// Уже одноклеточные регионы не участвуют в расщеплении.
        /// </summary>
        /// <param name="draftRegions"> Текущие регионы на карте. </param>
        /// <param name="targetRegionCount"> Целевое число регионов. </param>
        /// <returns> Возвращает новый массив черновиков регионов. </returns>
        private RegionDraft[] SplitRegionsForTransitions(
            [NotNull, ItemNotNull] RegionDraft[] draftRegions,
            int targetRegionCount)
        {
            if (draftRegions == null)
            {
                throw new ArgumentNullException(nameof(draftRegions));
            }

            if (targetRegionCount <= 0)
            {
                throw new ArgumentException("Целевое количество регионов должно быть больше 0.", nameof(targetRegionCount));
            }

            var regionCountDiff = targetRegionCount - draftRegions.Length;
            if (regionCountDiff <= 0)
            {
                return (RegionDraft[])draftRegions.Clone();
            }

            var availableSplitRegions = draftRegions.Where(x => x.Coords.Length > 1);
            var availableCoords = from region in availableSplitRegions
                                  from coord in region.Coords.Skip(1)
                                  select new RegionCoords(coord, region);

            if (availableCoords.Count() < regionCountDiff)
            {
                // Возможна ситуация, когда в принципе клеток меньше,
                // чем требуется регионов.
                // Даже если делать по одной клетки на регион.
                // В этом случае ничего сделать нельзя.
                // Передаём проблему вызывающему коду.
                throw new CellularAutomatonException("Невозможно расщепить регионы на достаточное количество. Клеток меньше, чем требуется.");
            }

            var openRegionCoords = new List<RegionCoords>(availableCoords);
            var usedRegionCoords = new List<RegionCoords>();

            for (var i = 0; i < regionCountDiff; i++)
            {
                var coordRollIndex = _dice.Roll(0, openRegionCoords.Count - 1);
                var regionCoordPair = openRegionCoords[coordRollIndex];
                openRegionCoords.RemoveAt(coordRollIndex);
                usedRegionCoords.Add(regionCoordPair);
            }

            var newDraftRegionList = new List<RegionDraft>();
            var regionGroups = usedRegionCoords.GroupBy(x => x.Region)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());

            foreach (var draftRegion in draftRegions)
            {
                if (regionGroups.TryGetValue(draftRegion, out var splittedRegionCoords))
                {
                    var splittedCoords = splittedRegionCoords.Select(x => x.Coords).ToArray();

                    var newCoordsOfCurrentRegion = draftRegion.Coords
                        .Except(splittedCoords)
                        .ToArray();

                    var recreatedRegionDraft = new RegionDraft(newCoordsOfCurrentRegion);
                    newDraftRegionList.Add(recreatedRegionDraft);

                    foreach (var splittedCoord in splittedCoords)
                    {
                        var newRegionDraft = new RegionDraft(new[] { splittedCoord });
                        newDraftRegionList.Add(newRegionDraft);
                    }
                }
                else
                {
                    newDraftRegionList.Add(draftRegion);
                }
            }

            return newDraftRegionList.ToArray();
        }

        private static void CreateCorridors(Matrix<bool> matrix, RegionDraft[] draftRegions, ISectorMap map)
        {
            var cellMap = matrix.Items;
            var mapWidth = matrix.Width;
            var mapHeight = matrix.Height;

            var regionNodeCoords = draftRegions.SelectMany(x => x.Coords);

            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    if (cellMap[x, y])
                    {
                        var offsetCoord = new OffsetCoords(x, y);

                        if (!regionNodeCoords.Contains(offsetCoord))
                        {
                            var node = new HexNode(x, y);
                            map.AddNode(node);
                        }
                    }
                }
            }
        }

        private static RegionDraft[] MakeUnitedRegions(Matrix<bool> matrix)
        {
            // Формирование регионов.
            // Регионы, кроме дальнейшего размещения игровых предметов,
            // в рамках этой генерации будут служить для обнаружения
            // изолированных регионов.
            // В секторе не должно быть изолированых регионов, поэтому
            // дальше все регионы объединяются в единый граф.

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

            if (!openNodes.Any())
            {
                throw new CellularAutomatonException("Ни одна из клеток не выжила.");
            }

            // Разбиваем все проходимые (true) клетки на регионы
            // через заливку.
            var regions = new List<RegionDraft>();
            while (openNodes.Any())
            {
                var openNode = openNodes.First();
                var regionCoords = FloodFillRegions(matrix, openNode);
                var region = new RegionDraft(regionCoords.ToArray());

                openNodes.RemoveAll(x => region.Coords.Contains(x));

                regions.Add(region);
            }

            // Соединяем все регионы в единый граф.
            var openRegions = new List<RegionDraft>(regions);
            var unitedRegions = new List<RegionDraft>();

            var startRegion = openRegions[0];
            openRegions.RemoveAt(0);
            unitedRegions.Add(startRegion);

            while (openRegions.Any())
            {
                var unitedRegionCoords = unitedRegions.SelectMany(x => x.Coords).ToArray();

                // Ищем две самые ближние точки между объединённым регионом и 
                // и всеми открытыми регионами.

                var currentDistance = int.MaxValue;
                OffsetCoords? currentOpenRegionCoord = null;
                OffsetCoords? currentUnitedRegionCoord = null;
                RegionDraft nearbyOpenRegion = null;

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

                // Если координаты, которые нужно соединить, найдены,
                // то прорываем тоннель.
                if (nearbyOpenRegion != null
                    && currentOpenRegionCoord != null
                     && currentUnitedRegionCoord != null)
                {
                    var openCubeCoord = HexHelper.ConvertToCube(currentOpenRegionCoord.Value);
                    var unitedCubeCoord = HexHelper.ConvertToCube(currentUnitedRegionCoord.Value);

                    var line = CubeCoordsHelper.CubeDrawLine(openCubeCoord, unitedCubeCoord);
                    foreach (var lineItem in line)
                    {
                        var offsetCoords = HexHelper.ConvertToOffset(lineItem);

                        matrix.Items[offsetCoords.X, offsetCoords.Y] = true;
                    }

                    openRegions.Remove(nearbyOpenRegion);
                    unitedRegions.Add(nearbyOpenRegion);
                }
            }

            return regions.ToArray();
        }

        private static bool[,] DoSimulationStep(Matrix<bool> matrix)
        {
            var newCellMap = new bool[matrix.Width, matrix.Height];

            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    var aliveCount = CountAliveNeighbours(matrix, x, y);

                    if (matrix.Items[x, y])
                    {
                        if (aliveCount < DEATH_LIMIT)
                        {
                            newCellMap[x, y] = false;
                        }
                        else
                        {
                            newCellMap[x, y] = true;
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (aliveCount > BIRTH_LIMIT)
                        {
                            newCellMap[x, y] = true;
                        }
                        else
                        {
                            newCellMap[x, y] = false;
                        }
                    }
                }
            }

            return newCellMap;
        }

        private static int CountAliveNeighbours(Matrix<bool> matrix, int x, int y)
        {
            var aliveCount = 0;

            var cubeCoords = HexHelper.ConvertToCube(x, y);
            var offsetsImplicit = HexHelper.GetOffsetClockwise();
            var offsetsDiagonal = HexHelper.GetDiagonalOffsetClockwise();
            var offsets = offsetsImplicit.Union(offsetsDiagonal);
            foreach (var offset in offsets)
            {
                var neighbour = cubeCoords + offset;

                var offsetCoords = HexHelper.ConvertToOffset(neighbour);

                var nX = offsetCoords.X;
                var nY = offsetCoords.Y;

                // Границу мертвым живым соседом.
                // Сделано, чтобы углы не заполнялись.

                if (nX >= 0 && nY >= 0 && nX < matrix.Width && nY < matrix.Height)
                {
                    if (matrix.Items[nX, nY])
                    {
                        aliveCount++;
                    }
                }
            }

            return aliveCount;
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

        private sealed class RegionCoords
        {
            public RegionCoords(OffsetCoords coords, RegionDraft region)
            {
                Coords = coords;
                Region = region ?? throw new ArgumentNullException(nameof(region));
            }

            public OffsetCoords Coords { get; }

            public RegionDraft Region { get; }
        }

        private sealed class CreateMatrixResult
        {
            public CreateMatrixResult(Matrix<bool> matrix, RegionDraft[] regions)
            {
                Matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
                Regions = regions ?? throw new ArgumentNullException(nameof(regions));
            }

            public Matrix<bool> Matrix { get; }
            public RegionDraft[] Regions { get; }
        }
    }
}
