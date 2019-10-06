using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var cellularAutomatonOptions = (ISectorCellularAutomataMapFactoryOptionsSubScheme)sectorScheme.MapGeneratorOptions;

            var mapWidth = cellularAutomatonOptions.MapWidth;
            var mapHeight = cellularAutomatonOptions.MapHeight;

            var _chanceToStartAlive = cellularAutomatonOptions.ChanceToStartAlive;

            var mapData = new MapData
            {
                Matrix = new bool[mapWidth, mapHeight],
                Width = mapWidth,
                Height = mapHeight
            };

            // Случайное заполнение
            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    var blockRoll = _dice.Roll(100);
                    if (blockRoll < _chanceToStartAlive)
                    {
                        mapData.Matrix[x, y] = true;
                    }
                }
            }

            // Несколько шагов симуляции
            for (var i = 0; i < SIMULATION_COUNT; i++)
            {
                var newMap = DoSimulationStep(mapData);
                mapData.Matrix = newMap;
            }

            var draftRegions = MakeUnitedRegions(mapData);

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
            CreateCorridors(mapData, draftRegions, map);

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

        private void CreateCorridors(MapData mapData, RegionDraft[] draftRegions, ISectorMap map)
        {
            var cellMap = mapData.Matrix;
            var mapWidth = mapData.Width;
            var mapHeight = mapData.Height;

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

        private RegionDraft[] MakeUnitedRegions(MapData mapData)
        {
            // Формирование регионов.
            // Регионы, кроме дальнейшего размещения игровых предметов,
            // в рамках этой генерации будут служить для обнаружения
            // изолированных регионов.
            // В секторе не должно быть изолированых регионов, поэтому
            // дальше все регионы объединяются в единый граф.
            var openNodes = new List<OffsetCoords>();
            for (var x = 0; x < mapData.Width; x++)
            {
                for (var y = 0; y < mapData.Height; y++)
                {
                    if (mapData.Matrix[x, y])
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
                var regionCoords = FloodFillRegions(mapData, openNode);
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
                        mapData.Matrix[offsetCoords.X, offsetCoords.Y] = true;
                    }

                    openRegions.Remove(nearbyOpenRegion);
                    unitedRegions.Add(nearbyOpenRegion);
                }
            }

            return regions.ToArray();
        }

        private bool[,] DoSimulationStep(MapData mapData)
        {
            var newCellMap = new bool[mapData.Width, mapData.Height];

            for (var x = 0; x < mapData.Width; x++)
            {
                for (var y = 0; y < mapData.Height; y++)
                {
                    var aliveCount = CountAliveNeighbours(mapData, x, y);

                    if (mapData.Matrix[x, y])
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

        private int CountAliveNeighbours(MapData mapData, int x, int y)
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

                if (nX >= 0 && nY >= 0 && nX < mapData.Width && nY < mapData.Height)
                {
                    if (mapData.Matrix[nX, nY])
                    {
                        aliveCount++;
                    }
                }
            }

            return aliveCount;
        }

        private static IEnumerable<OffsetCoords> FloodFillRegions(MapData mapData, OffsetCoords point)
        {
            var snapshotCellmap = (bool[,])mapData.Matrix.Clone();

            var regionPoints = HexBinaryFiller.FloodFill(
                snapshotCellmap,
                mapData.Width,
                mapData.Height,
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
