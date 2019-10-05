using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    public sealed class CellularAutomatonMapFactory : IMapFactory
    {
        private readonly IDice _dice;
        private readonly int _simulationCount = 3;
        private int _mapWidth = 100;
        private int _mapHeight = 100;
        private int _chanceToStartAlive = 45; // Процент, что на старте клетка будет живой.
        private int _deathLimit = 4;
        private int _birthLimit = 6;

        public CellularAutomatonMapFactory(IDice dice)
        {
            _dice = dice;
        }

        public Task<ISectorMap> CreateAsync(object options)
        {
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
            for (var i = 0; i < _simulationCount; i++)
            {
                var newMap = DoSimulationStep(cellMap);
                cellMap = newMap;
            }

            // Создание графа карты тектора на основе карты клеточного автомата
            ISectorMap map = new SectorHexMap();

            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    if (cellMap[x, y])
                    {
                        var node = new HexNode(x, y);
                        map.AddNode(node);
                    }
                }
            }

            return Task.FromResult(map);
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
                        if (aliveCount < _deathLimit)
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
                        if (aliveCount > _birthLimit)
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

                if (nX < 0 || nY < 0 || nX >= _mapWidth || nY >= _mapHeight)
                {
                    // Границу считаем живым соседом.
                    // Сделано, чтобы зполнялись углы.
                    aliveCount++;
                }
                else if (oldCellMap[nX, nY])
                {
                    aliveCount++;
                }
            }

            return aliveCount;
        }
    }
}
