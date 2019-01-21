using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public sealed class RoomGenerator : IRoomGenerator
    {
        private readonly IRoomGeneratorRandomSource _randomSource;
        private readonly RoomGeneratorSettings _settings;

        [ExcludeFromCodeCoverage]
        public RoomGenerator([NotNull] IRoomGeneratorRandomSource randomSource,
            [NotNull] RoomGeneratorSettings settings)
        {
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        }

        [ExcludeFromCodeCoverage]
        public RoomGenerator(IRoomGeneratorRandomSource randomSource) : this(randomSource, new RoomGeneratorSettings())
        {
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        }

        public List<Room> GenerateRoomsInGrid()
        {
            // На 20 комнат будет матрица 6х6.
            var roomGridSize = (int)Math.Ceiling(Math.Log(_settings.RoomCount, 2)) + 1;
            var roomGrid = new RoomMatrix(roomGridSize);

            var rooms = new List<Room>();

            var roomMatrixCoords = _randomSource.RollRoomMatrixPositions(roomGridSize, _settings.RoomCount).ToArray();

            for (var i = 0; i < _settings.RoomCount; i++)
            {
                var rolledPosition = roomMatrixCoords[i];

                var room = new Room
                {
                    PositionX = rolledPosition.X,
                    PositionY = rolledPosition.Y
                };

                roomGrid.SetRoom(rolledPosition.X, rolledPosition.Y, room);

                var rolledSize = _randomSource.RollRoomSize(_settings.RoomCellSize);

                room.Width = rolledSize.Width;
                room.Height = rolledSize.Height;

                rooms.Add(room);
            }

            return rooms;
        }


        public void CreateRoomNodes(IMap map, List<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room);
            }
        }


        public void BuildRoomCorridors(IMap map, List<Room> rooms, HashSet<string> edgeHash)
        {
            var roomNet = _randomSource.RollRoomNet(rooms, _settings.MaxNeighbors);
            foreach (var roomPair in roomNet)
            {
                foreach (var selectedRoom in roomPair.Value)
                {
                    ConnectRoomsWithCorridor(map, roomPair.Key, selectedRoom, edgeHash);
                }
            }
        }

        private void CreateOneRoomNodes(IMap map, HashSet<string> edgeHash, Room room)
        {
            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + room.PositionX * _settings.RoomCellSize;
                    var nodeY = y + room.PositionY * _settings.RoomCellSize;
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

        private static HexNode CreateCorridorNode(IMap map, HexNode currentNode, int currentX, int currentY, HashSet<string> edgeHash)
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

        private void ConnectRoomsWithCorridor(IMap map, Room room, Room selectedRoom, HashSet<string> edgeHash)
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

                var node = CreateCorridorNode(map, currentNode, offsetCoords.X, offsetCoords.Y, edgeHash);
                currentNode = node;
            }
        }
    }
}
