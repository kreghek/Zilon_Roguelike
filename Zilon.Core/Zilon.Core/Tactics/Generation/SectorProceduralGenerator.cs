using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class SectorProceduralGenerator
    {
        private const int ROOM_COUNT = 10;
        private const int ROOM_CELL_SIZE = 10;
        private const int MaxNeighbors = 1;
        private const int NeighborProbably = 100;
        private readonly ISectorGeneratorRandomSource _randomSource;

        public StringBuilder Log { get; set; }

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource;

            Log = new StringBuilder();
        }

        public void Generate(ISector sector, IMap map)
        {
            Log.Clear();

            var roomGridSize = (int)Math.Ceiling(Math.Log(ROOM_COUNT, 2)) + 1;
            var roomGrid = new Room[roomGridSize, roomGridSize];
            var rooms = new List<Room>();

            // Генерируем комнаты в сетке
            var edgeHash = new HashSet<string>();

            GenerateRoomsInGrid(roomGridSize, roomGrid, rooms);

            // Создаём узлы и рёбра комнат
            CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            BuildRoomCorridors(map, rooms, edgeHash);

            sector.StartNodes = rooms.First().Nodes.Cast<IMapNode>().ToArray();
        }

        private void BuildRoomCorridors(IMap map, List<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var availableRooms = rooms.Where(x => x != room).ToArray();

                var selectedRooms = _randomSource.RollConnectedRooms(room,
                    MaxNeighbors,
                    NeighborProbably,
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

        private static void ConnectRoomsWithCorridor(IMap map, HashSet<string> edgeHash, Room room, Room selectedRoom)
        {
            var currentNode = room.Nodes.First();
            var targetNode = selectedRoom.Nodes.First();

            //Строим коридор
            var currentX = currentNode.OffsetX;
            var currentY = currentNode.OffsetY;
            while (currentNode != targetNode)
            {
                if (currentX >= targetNode.OffsetX)
                {
                    currentX--;
                }
                else
                {
                    currentX++;
                }

                if (currentY >= targetNode.OffsetY)
                {
                    currentY--;
                }
                else
                {
                    currentY++;
                }

                var node = CreateCorridorNode(map, edgeHash, currentNode, currentX, currentY);

                currentNode = node;
            }
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

        private static void CreateRoomNodes(IMap map, List<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room);
            }
        }

        private static void CreateOneRoomNodes(IMap map, HashSet<string> edgeHash, Room room)
        {
            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + room.PositionX * ROOM_CELL_SIZE;
                    var nodeY = y + room.PositionY * ROOM_CELL_SIZE;
                    var node = new HexNode(nodeX, nodeY);
                    room.Nodes.Add(node);
                    map.Nodes.Add(node);

                    var neighbors = HexNodeHelper.GetNeighbors(node, room.Nodes);

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

        private void GenerateRoomsInGrid(int roomGridSize, Room[,] roomGrid, List<Room> rooms)
        {
            for (var i = 0; i < ROOM_COUNT; i++)
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

                    roomGrid[rolledPosition.X, rolledPosition.Y] = room;

                    var rolledSize = _randomSource.RollRoomSize(ROOM_CELL_SIZE - 2);

                    room.Width = rolledSize.Width + 2;
                    room.Height = rolledSize.Height + 2;

                    rooms.Add(room);

                    Log.AppendLine($"Выбрана комната в ячейке {rolledPosition} размером {rolledSize}.");
                }
                else
                {
                    throw new InvalidOperationException("Не найдено свободной ячейки для комнаты.");
                }
            }
        }

        /// <summary>
        /// Создаёт на карте ребро, соединяющее два узла этой карты.
        /// </summary>
        /// <param name="targetMap"> Целевая карта, для которой нужно создать ребро. </param>
        /// <param name="node"> Исходное ребро карты. </param>
        /// <param name="neighbor"> Соседнее ребро карты, с которым будет соединено исходное. </param>
        private static void AddEdgeToMap(IMap targetMap, HashSet<string> edgeHash, HexNode node, HexNode neighbor)
        {
            var hashKey1 = $"{node}-{neighbor}";
            edgeHash.Add(hashKey1);

            var edge = new Edge(node, neighbor);
            targetMap.Edges.Add(edge);
        }

        /// <summary>
        /// Возвращает ребро, соединяющее указанные узлы.
        /// </summary>
        /// <param name="map"> Карта, в которой проверяются ребра. </param>
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
    }
}
