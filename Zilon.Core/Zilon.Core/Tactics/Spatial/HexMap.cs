using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexMap : MapBase
    {
        private readonly IDictionary<SegmentKey, IMapNode[,]> _segmentDict;
        private readonly int _segmentSize;

        public HexMap(int segmentSize)
        {
            if (segmentSize % 2 != 0)
            {
                throw new ArgumentException("Аргумент должен быть чётным", nameof(segmentSize));
            }

            _segmentSize = segmentSize;

            _segmentDict = new Dictionary<SegmentKey, IMapNode[,]>();

            CreateSegment(0, 0);
        }

        /// <summary>Список узлов карты.</summary>
        public override IEnumerable<IMapNode> Nodes
        {
            get
            {
                foreach (var segmentKeyValue in _segmentDict)
                {
                    for (var i = 0; i < _segmentSize; i++)
                    {
                        for (var j = 0; j < _segmentSize; j++)
                        {
                            if (segmentKeyValue.Value[i, j] != null)
                            {
                                yield return segmentKeyValue.Value[i, j];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Создаёт ребро между двумя узлами графа карты.</summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public override void AddEdge(IMapNode node1, IMapNode node2)
        {
            // Эта возможность не нужна. Пока не будет сделан метод удаления ребра.
            // Сейчас ребра есть между всеми соседями в сетке шестиугольников.
        }

        /// <summary>Добавляет новый узел графа.</summary>
        /// <param name="node"></param>
        public override void AddNode(IMapNode node)
        {
            var hexNode = (HexNode)node;
            var offsetX = hexNode.OffsetX;
            var offsetY = hexNode.OffsetY;

            var nodeMatrix = _segmentDict.First().Value;
            nodeMatrix[offsetX, offsetY] = hexNode;
        }

        /// <summary>Возвращает узлы, напрямую соединённые с указанным узлом.</summary>
        /// <param name="node">Опорный узел, относительно которого выбираются соседние узлы.</param>
        /// <returns>Возвращает набор соседних узлов.</returns>
        public override IEnumerable<IMapNode> GetNext(IMapNode node)
        {
            var hexCurrent = (HexNode)node;
            var offsetCoords = new OffsetCoords(hexCurrent.OffsetX, hexCurrent.OffsetY);
            var segmentX = offsetCoords.X / _segmentSize;
            if (offsetCoords.X < 0)
            {
                segmentX--;
            }

            var segmentY = offsetCoords.Y / _segmentSize;
            if (offsetCoords.Y < 0)
            {
                segmentY--;
            }

            var localOffsetX = NormalizeNeighborCoord(offsetCoords.X % _segmentSize);
            var localOffsetY = NormalizeNeighborCoord(offsetCoords.Y % _segmentSize);

            var segmentKey = new SegmentKey(segmentX, segmentY);
            var matrix = _segmentDict[segmentKey];

            var directions = HexHelper.GetOffsetClockwise();
            var currentCubeCoords = HexHelper.ConvertToCube(localOffsetX, localOffsetY);

            for (var i = 0; i < 6; i++)
            {
                var dir = directions[i];
                var neighborLocalCube = new CubeCoords(dir.X + currentCubeCoords.X,
                    dir.Y + currentCubeCoords.Y,
                    dir.Z + currentCubeCoords.Z);

                var neighborLocalOffset = HexHelper.ConvertToOffset(neighborLocalCube);

                var neighborSegmentX = segmentX;
                var neighborSegmentY = segmentY;

                if (neighborLocalOffset.X < 0)
                {
                    neighborSegmentX--;
                }
                else if (neighborLocalOffset.X >= _segmentSize)
                {
                    neighborSegmentX++;
                }

                if (neighborLocalOffset.Y < 0)
                {
                    neighborSegmentY--;
                }
                else if (neighborLocalOffset.Y >= _segmentSize)
                {
                    neighborSegmentY++;
                }

                IMapNode currentNeibour;
                if (neighborSegmentX == segmentX &&
                    neighborSegmentY == segmentY)
                {
                    currentNeibour = matrix[neighborLocalOffset.X, neighborLocalOffset.Y];

                    if (currentNeibour == null)
                    {
                        continue;
                    }

                    yield return currentNeibour;
                }
            }
        }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode">Целевая ячейка.</param>
        /// <param name="actor">Проверяемый актёр.</param>
        /// <returns>true, если указанный узел проходим для актёра. Иначе - false.</returns>
        public override bool IsPositionAvailableFor(IMapNode targetNode, IActor actor)
        {
            if (!base.IsPositionAvailableFor(targetNode, actor))
            {
                return false;
            }

            var hexIsObstacle = CheckNodeIsObstable(targetNode);
            return !hexIsObstacle;
        }

        /// <summary>Удаляет ребро между двумя узлами графа карты.</summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public override void RemoveEdge(IMapNode node1, IMapNode node2)
        {
            // Эта возможность не нужна. Пока не будет сделан метод удаления ребра.
            // Сейчас ребра есть между всеми соседями в сетке шестиугольников.
        }

        public void SaveToFile(string fileName)
        {
            const int cellWidth = 4;

            var matrix = _segmentDict.First().Value;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.Write(" ".PadLeft(cellWidth, ' '));
                for (var x = 0; x < _segmentSize; x++)
                {
                    file.Write($"{x,3}".PadRight(cellWidth, ' '));
                }
                file.WriteLine();

                for (var y = 0; y < _segmentSize; y++)
                {
                    file.Write($"{y,3}".PadLeft(cellWidth, ' '));
                    for (var x = 0; x < _segmentSize; x++)
                    {
                        if (matrix[x, y] != null)
                        {
                            file.Write(" ".PadLeft(cellWidth, ' '));
                        }
                        else
                        {
                            file.Write("x".PadLeft(cellWidth, ' '));
                        }
                    }

                    file.WriteLine();
                }
            }
        }

        private void CreateSegment(int segmentX, int segmentY)
        {
            var matrix = new IMapNode[_segmentSize, _segmentSize];

            var key = new SegmentKey(segmentX, segmentY);
            _segmentDict[key] = matrix;
        }

        private int NormalizeNeighborCoord(int neighborX)
        {
            if (neighborX < 0)
            {
                neighborX += _segmentSize;
            }
            else if (neighborX >= _segmentSize)
            {
                neighborX -= _segmentSize;
            }

            return neighborX;
        }

        private static bool CheckNodeIsObstable(IMapNode targetNode)
        {
            var hex = targetNode as HexNode;
            var hexIsObstacle = hex.IsObstacle;
            return hexIsObstacle;
        }

        private struct SegmentKey
        {
            // ReSharper disable once MemberCanBePrivate.Local
            public readonly int X;

            // ReSharper disable once MemberCanBePrivate.Local
            public readonly int Y;

            public SegmentKey(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is SegmentKey))
                {
                    return false;
                }

                var key = (SegmentKey)obj;
                return X == key.X &&
                       Y == key.Y;
            }

            public override int GetHashCode()
            {
                var hashCode = 1502939027;
                hashCode = hashCode * -1521134295 + X.GetHashCode();
                hashCode = hashCode * -1521134295 + Y.GetHashCode();
                return hashCode;
            }
        }
    }
}
