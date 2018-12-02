using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class RoomGenerator
    {
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly RoomGeneratorSettings _settings;

        public StringBuilder Log { get; }

        /// <summary>
        /// Стартовая комната. Отсюда игрок будет начинать.
        /// </summary>
        public Room StartRoom { get; private set; }

        /// <summary>
        /// Комната с выходом из сектора.
        /// </summary>
        public Room ExitRoom { get; private set; }

        public RoomGenerator(ISectorGeneratorRandomSource randomSource,
            RoomGeneratorSettings settings,
            StringBuilder log)
        {
            _randomSource = randomSource;
            _settings = settings;

            Log = log;
        }

        public List<Room> GenerateRoomsInGrid()
        {
            var roomGridSize = (int)Math.Ceiling(Math.Log(_settings.RoomCount, 2)) + 1;
            var roomGrid = new RoomMatrix(roomGridSize);

            var rooms = new List<Room>();
            for (var i = 0; i < _settings.RoomCount; i++)
            {
                var rolledUncheckedPosition = _randomSource.RollRoomPosition(roomGridSize - 1);
                var rolledPosition = GenerationHelper.GetFreeCell(roomGrid, rolledUncheckedPosition);

                if (rolledPosition != null)
                {
                    var room = new Room
                    {
                        PositionX = rolledPosition.X,
                        PositionY = rolledPosition.Y
                    };

                    roomGrid.SetRoom(rolledPosition.X, rolledPosition.Y, room);

                    var rolledSize = _randomSource.RollRoomSize(_settings.RoomCellSize - 2);

                    room.Width = rolledSize.Width + 2;
                    room.Height = rolledSize.Height + 2;

                    rooms.Add(room);

                    if (StartRoom == null)
                    {
                        StartRoom = room;
                    }

                    Log.AppendLine($"Выбрана комната в ячейке {rolledPosition} размером {rolledSize}.");
                }
                else
                {
                    throw new InvalidOperationException("Не найдено свободной ячейки для комнаты.");
                }
            }

            ExitRoom = rooms.Last();

            return rooms;
        }

        public void CreateRoomNodes(IMap map, List<Room> rooms, HashSet<string> edgeHash)
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
                    var nodeX = x + room.PositionX * _settings.RoomCellSize;
                    var nodeY = y + room.PositionY * _settings.RoomCellSize;
                    var node = new HexNode(nodeX, nodeY);
                    room.Nodes.Add(node);
                    map.Nodes.Add(node);

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
                map.Nodes.Add(node);
            }

            var isExists = IsExistsEdge(edgeHash, node, currentNode);

            if (!isExists)
            {
                AddEdgeToMap(map, edgeHash, currentNode, node);
            }

            return node;
        }

        public void BuildRoomCorridors(IMap map, List<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var availableRooms = rooms.Where(x => x != room).ToArray();

                var selectedRooms = _randomSource.RollConnectedRooms(room,
                    _settings.MaxNeighbors,
                    _settings.NeighborProbably,
                    availableRooms);

                if (selectedRooms == null || !selectedRooms.Any())
                {
                    //Значит текущая комната тупиковая
                    Log.AppendLine($"Для комнаты {room} нет соседей (тупик).");
                    continue;
                }

                Log.AppendLine($"Для комнаты {room} выбраны соседи ");
                foreach (var selectedRoom in selectedRooms)
                {
                    Log.Append(selectedRoom + " ");
                }

                foreach (var selectedRoom in selectedRooms)
                {
                    ConnectRoomsWithCorridor(map, edgeHash, room, selectedRoom);
                }
            }
        }

        private void ConnectRoomsWithCorridor(IMap map, HashSet<string> edgeHash, Room room, Room selectedRoom)
        {
            var currentNode = room.Nodes.First();
            var targetNode = selectedRoom.Nodes.First();

            var points = CubeCoordsHelper.CubeDrawLine(currentNode.CubeCoords, targetNode.CubeCoords);

            foreach (var point in points)
            {
                var offsetCoords = HexHelper.ConvertToOffset(point);

                var node = CreateCorridorNode(map, edgeHash, currentNode, offsetCoords.X, offsetCoords.Y);
                currentNode = node;
            }
        }
    }
}
