using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexMap : MapBase
    {
        private readonly IList<IEdge> _edges;
        private readonly IDictionary<SegmentKey, IMapNode[,]> _segmentDict;
        private readonly int _segmentSize;

        public HexMap(int segmentSize)
        {
            _edges = new List<IEdge>();

            if (segmentSize % 2 != 0)
            {
                throw new ArgumentException("Аргумент должен быть нечтётным", nameof(segmentSize));
            }

            _segmentSize = segmentSize;

            _segmentDict = new Dictionary<SegmentKey, IMapNode[,]>();

            CreateSegment(0, 0);
        }

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

        public override void AddEdge(IMapNode node1, IMapNode node2)
        {
            //throw new NotImplementedException();
        }

        public override void AddNode(IMapNode node)
        {
            var hexNode = (HexNode)node;
            var offsetX = hexNode.OffsetX;
            var offsetY = hexNode.OffsetY;

            var nodeMatrix = _segmentDict.First().Value;
            nodeMatrix[offsetX, offsetY] = hexNode;
        }

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
                }
                else
                {
                    var segmentMatrix = CreateSegment(neighborSegmentX, neighborSegmentY);
                    var neighborX = NormalizeNeighborCoord(neighborLocalOffset.X);
                    var neighborY = NormalizeNeighborCoord(neighborLocalOffset.Y);

                    currentNeibour = segmentMatrix[neighborX, neighborY];
                }

                if (currentNeibour != null)
                {
                    yield return currentNeibour;
                }
            }

            //var hexCurrent = (HexNode)node;
            //var hexNodes = Nodes.Cast<HexNode>().ToArray();
            //var neighbors = HexNodeHelper.GetSpatialNeighbors(hexCurrent, hexNodes);

            //var currentEdges = from edge in _edges
            //                   where edge.Nodes.Contains(node)
            //                   select edge;
            //var currentEdgeArray = currentEdges.ToArray();

            //var actualNeighbors = new List<IMapNode>();
            //foreach (var testedNeighbor in neighbors)
            //{
            //    var edge = currentEdgeArray.SingleOrDefault(x => x.Nodes.Contains(testedNeighbor));
            //    if (edge == null)
            //    {
            //        continue;
            //    }

            //    yield return testedNeighbor;
            //}
        }

        public override void RemoveEdge(IMapNode node1, IMapNode node2)
        {
            throw new NotImplementedException();
        }

        private IMapNode[,] CreateSegment(int segmentX, int segmentY)
        {
            var matrix = new IMapNode[_segmentSize, _segmentSize];

            var key = new SegmentKey(segmentX, segmentY);
            _segmentDict[key] = matrix;
            return matrix;
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
                if (!(obj is SegmentKey)) return false;

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
