using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexMap : MapBase
    {
        private readonly IDictionary<SegmentKey, IGraphNode[,]> _segmentDict;
        private readonly int _segmentSize;

        public HexMap(int segmentSize)
        {
            if (segmentSize % 2 != 0)
            {
                throw new ArgumentException("Аргумент должен быть чётным", nameof(segmentSize));
            }

            _segmentSize = segmentSize;

            _segmentDict = new Dictionary<SegmentKey, IGraphNode[,]>();

            CreateSegment(0, 0);
        }

        /// <summary>Список узлов карты.</summary>
        public override IEnumerable<IGraphNode> Nodes
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
        public override void AddEdge(IGraphNode node1, IGraphNode node2)
        {
            // Эта возможность не нужна. Пока не будет сделан метод удаления ребра.
            // Сейчас ребра есть между всеми соседями в сетке шестиугольников.
        }

        /// <summary>Добавляет новый узел графа.</summary>
        /// <param name="node"></param>
        public override void AddNode(IGraphNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var hexNode = (HexNode)node;
            var offsetX = hexNode.OffsetCoords.X;
            var offsetY = hexNode.OffsetCoords.Y;

            var nodeMatrix = _segmentDict.First().Value;

            if (nodeMatrix[offsetX, offsetY] != null)
            {
                // Эта проверка нужна, чтобы отлавливать дубликаты координат,
                // которые могут быть созданы фабриками.
                // Дубликаты так же попадают в регионы и могут быть сложными в отладке.
                // Потому что искажают дальнейшую работу с картой.
                //TODO Рассматреть вариант, когда проверка выполняется  стороне регионов.
                throw new InvalidOperationException($"В координатах {offsetX},{offsetY} уже есть узел графа.");
            }

            nodeMatrix[offsetX, offsetY] = hexNode;
        }

        protected HexNode GetByCoords(int x, int y)
        {
            var segmentKey = new SegmentKey(0, 0);
            var segment = _segmentDict[segmentKey];
            try
            {
                var node = segment[x, y];
                return (HexNode)node;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>Возвращает узлы, напрямую соединённые с указанным узлом.</summary>
        /// <param name="node">Опорный узел, относительно которого выбираются соседние узлы.</param>
        /// <returns>Возвращает набор соседних узлов.</returns>
        public override IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var hexCurrent = (HexNode)node;
            var offsetCoords = hexCurrent.OffsetCoords;
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

            return GetNextFromMatrix(localOffsetX, localOffsetY, segmentX, segmentY, matrix);
        }

        private IEnumerable<IGraphNode> GetNextFromMatrix(int localOffsetX, int localOffsetY, int segmentX, int segmentY, IGraphNode[,] matrix)
        {
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

                IGraphNode currentNeibour;
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
        public override bool IsPositionAvailableFor(IGraphNode targetNode, IActor actor)
        {
            if (targetNode is null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (!base.IsPositionAvailableFor(targetNode, actor))
            {
                return false;
            }

            return true;
        }

        /// <summary>Удаляет ребро между двумя узлами графа карты.</summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public override void RemoveEdge(IGraphNode node1, IGraphNode node2)
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
            //TODO Отказаться от многомерного массива. Вместо этого сделать одномерный и адресацию через смещение.
            var matrix = new IGraphNode[_segmentSize, _segmentSize];

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

        public override bool IsPositionAvailableForContainer(IGraphNode targetNode)
        {
            return true;
        }

        /// <summary>
        /// Distances the between.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targetNode">The target node.</param>
        /// <returns></returns>
        public override int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            if (currentNode is null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (targetNode is null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            var actorHexNode = (HexNode)currentNode;
            var containerHexNode = (HexNode)targetNode;

            var actorCoords = actorHexNode.CubeCoords;
            var containerCoords = containerHexNode.CubeCoords;

            var distance = actorCoords.DistanceTo(containerCoords);

            return distance;
        }

        public override void RemoveNode(IGraphNode node)
        {
            throw new NotImplementedException();
        }

        private struct SegmentKey : IEquatable<SegmentKey>
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
                return obj is SegmentKey key && Equals(key);
            }

            public bool Equals(SegmentKey other)
            {
                return X == other.X &&
                       Y == other.Y;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = 1861411795;
                    hashCode = hashCode * -1521134295 + X.GetHashCode();
                    hashCode = hashCode * -1521134295 + Y.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(SegmentKey left, SegmentKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SegmentKey left, SegmentKey right)
            {
                return !(left == right);
            }
        }
    }
}