using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    using Zilon.Core.Common;
    using Zilon.Core.Persons;
    using Zilon.Core.Players;
    using Zilon.Core.Tactics.Behaviour.Bots;

    public class SectorProceduralGenerator
    {
        private const int ROOM_COUNT = 10;
        private const int ROOM_CELL_SIZE = 10;
        private const int MaxNeighbors = 1;
        private const int NeighborProbably = 100;
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly IPlayer _botPlayer;

        /// <summary>
        /// Стартовая комната. Отсюда игрок будет начинать.
        /// </summary>
        private Room _startRoom;

        /// <summary>
        /// Комната с выходом из сектора.
        /// </summary>
        private Room _exitRoom;

        /// <summary>
        /// Стартовые узлы.
        /// Набор узлов, где могут располагаться актёры игрока
        /// на начало прохождения сектора.
        /// </summary>
        public IMapNode[] StartNodes { get; private set; }

        public List<IActor> MonsterActors { get; }

        public Dictionary<IActor, IPatrolRoute> Patrols { get; }


        public StringBuilder Log { get; set; }

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource,
            IPlayer botPlayer)
        {
            _randomSource = randomSource;
            _botPlayer = botPlayer;
            Log = new StringBuilder();

            MonsterActors = new List<IActor>();
            Patrols = new Dictionary<IActor, IPatrolRoute>();
        }

        public void Generate(ISector sector, IMap map)
        {
            Log.Clear();

            var roomGridSize = (int)Math.Ceiling(Math.Log(ROOM_COUNT, 2)) + 1;
            var roomGrid = new Room[roomGridSize, roomGridSize];
            var edgeHash = new HashSet<string>();

            // Генерируем комнаты в сетке
            var rooms = GenerateRoomsInGrid(roomGridSize, roomGrid);
            var mainRooms = rooms.Where(x => x != _startRoom).ToArray();

            // Создаём узлы и рёбра комнат
            CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            BuildRoomCorridors(map, rooms, edgeHash);

            CreateRoomMonsters(mainRooms);

            SelectStartNodes(_startRoom);

            SelectExitPoints(sector, _exitRoom);
        }

        private void SelectExitPoints(ISector sector, Room exitRoom)
        {
            sector.ExitNodes = new[] { exitRoom.Nodes.Last() };
        }

        private void SelectStartNodes(Room startRoom)
        {
            StartNodes = startRoom.Nodes.Cast<IMapNode>().ToArray();
        }

        private void CreateRoomMonsters(IEnumerable<Room> rooms)
        {
            foreach (var room in rooms)
            {
                var person = new MonsterPerson();
                var startNode = room.Nodes.FirstOrDefault();
                var actor = new Actor(person, _botPlayer, startNode);
                MonsterActors.Add(actor);

                var finishPatrolNode = room.Nodes.Last();
                var patrolRoute = new PatrolRoute(new[] { startNode, finishPatrolNode });
                Patrols[actor] = patrolRoute;
            }
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

            var points = CubeDrawLine(currentNode.CubeCoords, targetNode.CubeCoords);

            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];

                var offsetCoords = HexHelper.ConvertToOffset(point);

                var node = CreateCorridorNode(map, edgeHash, currentNode, offsetCoords.X, offsetCoords.Y);
                currentNode = node;
            }
        }

        private static float Lerp(int a, int b, float t)
        {
            return a + (b - a) * t;
        }

        private static CubeCoords LerpCube(CubeCoords a, CubeCoords b, float t)
        {
            return new CubeCoords((int)Math.Round(Lerp(a.X, b.X, t)),
                (int)Math.Round(Lerp(a.Y, b.Y, t)),
                (int)Math.Round(Lerp(a.Z, b.Z, t)));
        }

        private static CubeCoords[] CubeDrawLine(CubeCoords a, CubeCoords b)
        {
            var n = a.DistanceTo(b);

            var list = new List<CubeCoords>();

            for (var i = 0; i < n; i++)
            {
                var point = LerpCube(a, b, 1.0f / n * i);
                list.Add(point);
            }

            return list.ToArray();
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

        private List<Room> GenerateRoomsInGrid(int roomGridSize, Room[,] roomGrid)
        {
            var rooms = new List<Room>();
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

                    if (_startRoom == null)
                    {
                        _startRoom = room;
                    }

                    Log.AppendLine($"Выбрана комната в ячейке {rolledPosition} размером {rolledSize}.");
                }
                else
                {
                    throw new InvalidOperationException("Не найдено свободной ячейки для комнаты.");
                }
            }

            _exitRoom = rooms.Last();

            return rooms;
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
