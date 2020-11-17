using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Generate map using cellular automaton algorithm.
    /// </summary>
    internal class CellularAutomatonGenerator
    {
        private const int DEATH_LIMIT = 4;
        private const int BIRTH_LIMIT = 6;

        private readonly IDice _dice;

        public CellularAutomatonGenerator(IDice dice)
        {
            _dice = dice;
        }

        public IEnumerable<RegionDraft> Generate(ref Matrix<bool> matrix, int fillProbability, int totalIterations)
        {
            InitiateMatrix(matrix, fillProbability);

            matrix = SimulateCellularAutomaton(matrix, totalIterations);

            var resizedMatrix = MapFactoryHelper.ResizeMatrixTo7(matrix);

            var matrixWithMargins = resizedMatrix.CreateMatrixWithMargins(2, 2);

            var draftRegions = RegionFinder.FindPassableRegionsFor(matrixWithMargins).ToArray();

            matrix = matrixWithMargins;

            return draftRegions;
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

                if (nX >= 0 && nY >= 0 && nX < matrix.Width && nY < matrix.Height && matrix.Items[nX, nY])
                {
                    aliveCount++;
                }
            }

            return aliveCount;
        }

        private static Matrix<bool> DoSimulationStep(Matrix<bool> matrix)
        {
            var newCellMap = new Matrix<bool>(matrix.Width, matrix.Height);

            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    var aliveCount = CountAliveNeighbours(matrix, x, y);

                    if (matrix.Items[x, y])
                    {
                        newCellMap[x, y] = aliveCount >= DEATH_LIMIT;
                    }
                    else
                    {
                        //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                        newCellMap[x, y] = aliveCount > BIRTH_LIMIT;
                    }
                }
            }

            return newCellMap;
        }

        private void InitiateMatrix(Matrix<bool> matrix, int fillProbability)
        {
            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    var blockRoll = _dice.Roll(100);
                    if (blockRoll < fillProbability)
                    {
                        matrix.Items[x, y] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Simulate live during <see cref="totalIterations" />.
        /// </summary>
        private static Matrix<bool> SimulateCellularAutomaton(Matrix<bool> matrix, int totalIterations)
        {
            for (var i = 0; i < totalIterations; i++)
            {
                var newMap = DoSimulationStep(matrix);
                matrix = new Matrix<bool>(newMap.Items, matrix.Width, matrix.Height);
            }

            return matrix;
        }
    }
}