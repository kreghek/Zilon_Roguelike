using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Фабрика карты на основе клеточного автомата.
    /// </summary>
    public sealed class CellularAutomatonMapFactory : IMapFactory
    {
        private readonly int SIMULATION_COUNT = 3;
        private const int DEATH_LIMIT = 4;
        private const int BIRTH_LIMIT = 6;

        private readonly IDice _dice;

        //TODO Эти параметры вынести в схему сектора
        private int _mapWidth = 25;
        private int _mapHeight = 25;
        private int _chanceToStartAlive = 45; // Процент, что на старте клетка будет живой.

        /// <summary>
        /// Конструктор фабрики.
        /// </summary>
        /// <param name="dice"> Кость для рандома. </param>
        public CellularAutomatonMapFactory(IDice dice)
        {
            _dice = dice;
        }

        public Task<ISectorMap> CreateAsync(object options)
        {
            var sectorScheme = (ISectorSubScheme)options;
            var transitions = CreateTransitions(sectorScheme);

            var cellMap = new bool[_mapWidth, _mapHeight];

            // Случайное заполнение
            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    var blockRoll = _dice.Roll(100);
                    if (blockRoll < _chanceToStartAlive)
                    {
                        cellMap[x, y] = true;
                    }
                }
            }

            // Несколько шагов симуляции
            for (var i = 0; i < SIMULATION_COUNT; i++)
            {
                var newMap = DoSimulationStep(cellMap);
                cellMap = newMap;
            }

            var draftRegions = MakeConnectedRegions(cellMap);

            // Создание графа карты сектора на основе карты клеточного автомата.
            ISectorMap map = new SectorHexMap();

            var regionIdCounter = 1;
            foreach (var draftRegion in draftRegions)
            {
                var regionNodeList = new List<IMapNode>();

                foreach (var coord in draftRegion.Coords)
                {
                    var node = new HexNode(coord.X, coord.Y);
                    map.AddNode(node);

                    regionNodeList.Add(node);
                }

                var region = new MapRegion(regionIdCounter, regionNodeList.ToArray());

                map.Regions.Add(region);

                regionIdCounter++;
            }

            // Добавляем узлы каридоров.
            CreateCorridors(cellMap, draftRegions, map);

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
                for (var i = 0; i < transitionArray.Length; i++)
                {
                    // +1, потому что первый регион уже занят под стартовый.
                    var transitionRegion = regionOrderedBySize[i + 1];

                    var transition = transitionArray[i];

                    var transitionNode = transitionRegion.Nodes.First();

                    map.Transitions.Add(transitionNode, transition);

                    if (transition.SectorSid == null)
                    {
                        transitionRegion.IsOut = true;
                    }
                }
            }

            return Task.FromResult(map);
        }

        private void CreateCorridors(bool[,] cellMap, RegionDraft[] draftRegions, ISectorMap map)
        {
            var regionNodeCoords = draftRegions.SelectMany(x => x.Coords);

            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
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

        private RegionDraft[] MakeConnectedRegions(bool[,] cellMap)
        {
            // Формирование регионов.
            // Регионы, кроме дальнейшего размещения игровых предметов,
            // в рамках этой генерации будут служить для обнаружения
            // изолированных регионов.
            // В секторе не должно быть изолированых регионов, поэтому
            // дальше все регионы объединяются в единый граф.
            var openNodes = new List<OffsetCoords>();
            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    if (cellMap[x, y])
                    {
                        openNodes.Add(new OffsetCoords(x, y));
                    }
                }
            }

            // Разбиваем все проходимые (true) клетки на регионы
            // через заливку.
            var regions = new List<RegionDraft>();
            while (openNodes.Any())
            {
                var openNode = openNodes.First();
                var regionCoords = FloodFillRegions(cellMap, openNode);
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
                OffsetCoords currentOpenRegionCoord = null;
                OffsetCoords currentUnitedRegionCoord = null;
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
                    var openCubeCoord = HexHelper.ConvertToCube(currentOpenRegionCoord);
                    var unitedCubeCoord = HexHelper.ConvertToCube(currentUnitedRegionCoord);

                    var line = CubeCoordsHelper.CubeDrawLine(openCubeCoord, unitedCubeCoord);
                    foreach (var lineItem in line)
                    {
                        var offsetCoords = HexHelper.ConvertToOffset(lineItem);
                        cellMap[offsetCoords.X, offsetCoords.Y] = true;
                    }

                    openRegions.Remove(nearbyOpenRegion);
                    unitedRegions.Add(nearbyOpenRegion);
                }
            }

            return regions.ToArray();
        }

        private bool[,] DoSimulationStep(bool[,] oldCellMap)
        {
            var newCellMap = new bool[_mapWidth, _mapHeight];

            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    var aliveCount = CountAliveNeighbours(oldCellMap, x, y);

                    if (oldCellMap[x, y])
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

        private int CountAliveNeighbours(bool[,] oldCellMap, int x, int y)
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

                if (nX >= 0 && nY >= 0 && nX < _mapWidth && nY < _mapHeight)
                {
                    if (oldCellMap[nX, nY])
                    {
                        aliveCount++;
                    }
                }
            }

            return aliveCount;
        }

        private IEnumerable<OffsetCoords> FloodFillRegions(bool[,] cellMap, OffsetCoords point)
        {
            var snapshotCellmap = (bool[,])cellMap.Clone();

            var regionPoints = new List<OffsetCoords>();

            var pixels = new Stack<OffsetCoords>();
            pixels.Push(point);

            while (pixels.Count > 0)
            {
                var currentCell = pixels.Pop();

                var isInBound = IsInBounds(currentCell);

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

                    pixels.Push(neighbourCoords);
                }
            }

            return regionPoints;
        }

        private bool IsInBounds(OffsetCoords a)
        {
            return a.X < _mapWidth && a.X >= 0 &&
                                    a.Y < _mapHeight && a.Y >= 0;
        }

        private sealed class RegionDraft
        {
            public RegionDraft(OffsetCoords[] coords)
            {
                Coords = coords ?? throw new ArgumentNullException(nameof(coords));
            }

            public OffsetCoords[] Coords { get; }
        }

        private static IEnumerable<RoomTransition> CreateTransitions(ISectorSubScheme sectorScheme)
        {
            if (sectorScheme.TransSectorSids == null)
            {
                return new[] { RoomTransition.CreateGlobalExit() };
            }

            return sectorScheme.TransSectorSids.Select(sid => new RoomTransition(sid));
        }
    }
}
