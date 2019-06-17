using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators.RoomStyle
{
    internal sealed class TestSnakeRoomGenerator : IRoomGenerator
    {
        private readonly OffsetCoords[] _rolledOffsetCoords;
        private readonly Size _rolledSize;

        public TestSnakeRoomGenerator()
        {
            _rolledOffsetCoords = new[] {
                new OffsetCoords(0, 0),new OffsetCoords(1, 0), new OffsetCoords(2, 0), new OffsetCoords(3, 0),
                new OffsetCoords(3, 1), new OffsetCoords(2, 1), new OffsetCoords(1, 1), new OffsetCoords(0, 1),
                new OffsetCoords(0, 2),new OffsetCoords(1, 2)
            };

            _rolledSize = new Size(3, 3);
        }

        public IEnumerable<Room> GenerateRoomsInGrid(int roomCount,
            int roomMinSize,
            int roomMaxSize,
            IEnumerable<RoomTransition> availableTransitions)
        {
            var rooms = new List<Room>();

            for (var i = 0; i < _rolledOffsetCoords.Length; i++)
            {
                var room = new Room
                {
                    PositionX = _rolledOffsetCoords[i].X,
                    PositionY = _rolledOffsetCoords[i].Y
                };

                var rolledSize = new Size(3, 3);

                room.Width = rolledSize.Width + 2;
                room.Height = rolledSize.Height + 2;

                rooms.Add(room);

            }

            return rooms;
        }

        public void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            var roomArray = rooms.ToArray();
            for (var i = 0; i < roomArray.Length - 1; i++)
            {
                ConnectRoomsWithCorridor(map, edgeHash, roomArray[i], roomArray[i + 1]);
            }
        }

        #region Duplicate
        public void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room);
            }
        }

        private void CreateOneRoomNodes(IMap map, HashSet<string> edgeHash, Room room)
        {
            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + room.PositionX * 20;
                    var nodeY = y + room.PositionY * 20;
                    var node = new HexNode(nodeX, nodeY);
                    room.Nodes.Add(node);
                    map.AddNode(node);

                    var neighbors = HexNodeHelper.GetSpatialNeighbors(node, room.Nodes);

                    foreach (var neighbor in neighbors)
                    {
                        var isExists = IsExistsEdge(edgeHash, node, neighbor);

                        if (!isExists)
                        {
                            AddEdgeToMap(map, edgeHash, node, neighbor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт на карте ребро, соединяющее два узла этой карты.
        /// </summary>
        /// <param name="targetMap"> Целевая карта, для которой нужно создать ребро. </param>
        /// <param name="edgeHash"> Хеш ребер карты. </param>
        /// <param name="node"> Исходное ребро карты. </param>
        /// <param name="neighbor"> Соседнее ребро карты, с которым будет соединено исходное. </param>
        private static void AddEdgeToMap(IMap targetMap, HashSet<string> edgeHash, HexNode node, HexNode neighbor)
        {
            var hashKey1 = $"{node}-{neighbor}";
            edgeHash.Add(hashKey1);

            var edge = new Edge(node, neighbor);
            targetMap.AddEdge(node, neighbor);
        }

        /// <summary>
        /// Возвращает ребро, соединяющее указанные узлы.
        /// </summary>
        /// <param name="edgeHash"> Хеш ребер карты. </param>
        /// <param name="node"> Искомый узел. </param>
        /// <param name="neighbor"> Узел, с которым соединён искомый. </param>
        /// <returns> Ребро или null, если такого ребра нет на карте. </returns>
        private static bool IsExistsEdge(HashSet<string> edgeHash, HexNode node, HexNode neighbor)
        {
            var hashKey1 = $"{node}-{neighbor}";
            if (edgeHash.Contains(hashKey1))
            {
                return true;
            }

            var hashKey2 = $"{neighbor}-{node}";
            return edgeHash.Contains(hashKey2);
        }

        private static HexNode CreateCorridorNode(IMap map, HashSet<string> edgeHash, HexNode currentNode, int currentX, int currentY)
        {
            var node = map.Nodes.OfType<HexNode>()
                                .SingleOrDefault(x => x.OffsetX == currentX && x.OffsetY == currentY);

            if (node == null)
            {
                node = new HexNode(currentX, currentY);
                map.AddNode(node);
            }

            var isExists = IsExistsEdge(edgeHash, node, currentNode);

            if (!isExists)
            {
                AddEdgeToMap(map, edgeHash, currentNode, node);
            }

            return node;
        }

        private void ConnectRoomsWithCorridor(IMap map, HashSet<string> edgeHash, Room room, Room selectedRoom)
        {
            var currentNode = room.Nodes.First();
            var targetNode = selectedRoom.Nodes.First();

            var points = CubeCoordsHelper.CubeDrawLine(currentNode.CubeCoords, targetNode.CubeCoords);

            foreach (var point in points)
            {
                var offsetCoords = HexHelper.ConvertToOffset(point);

                // это происходит, потому что если при нулевом Х для обеих комнат
                // попытаться отрисовать линию коридора, то она будет змейкой заходить за 0.
                // Нужно искать решение получше.
                offsetCoords = new OffsetCoords(offsetCoords.X < 0 ? 0 : offsetCoords.X,
                    offsetCoords.Y < 0 ? 0 : offsetCoords.Y);

                var node = CreateCorridorNode(map, edgeHash, currentNode, offsetCoords.X, offsetCoords.Y);
                currentNode = node;
            }
        }

        #endregion
    }
}
