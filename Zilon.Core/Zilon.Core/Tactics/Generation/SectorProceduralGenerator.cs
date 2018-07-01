using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class SectorProceduralGenerator
    {
        private const int ROOM_COUNT = 10;
        private const int ROOM_CELL_SIZE = 10;

        private readonly ISectorGeneratorRandomSource _randomSource;

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public void Generate(ISector sector, IMap map, IActor playerActor)
        {
            // Генерируем комнаты в сетке
            var roomGridSize = (int)Math.Ceiling(Math.Log(ROOM_COUNT, 2)) + 1;
            var roomGrid = new Room[roomGridSize, roomGridSize];
            var rooms = new List<Room>();

            for (var i = 0; i < ROOM_COUNT; i++)
            {
                var attemptCounter = 3;
                while (true)
                {
                    _randomSource.RollRoomPosition(roomGridSize, out int roomPositionX, out int roomPositionY);

                    var currentRoom = roomGrid[roomPositionX, roomPositionY];
                    if (currentRoom == null)
                    {
                        var room = new Room();
                        room.PositionX = roomPositionX;
                        room.PositionY = roomPositionY;

                        _randomSource.RollRoomSize(ROOM_CELL_SIZE, out int roomWidth, out int roomHeight);

                        room.Width = roomWidth;
                        room.Height = roomHeight;

                        rooms.Add(room);
                    }
                    else
                    {
                        break;
                    }

                    attemptCounter--;
                    if (attemptCounter <= 0)
                    {
                        throw new Exception("Не удалось выбрать ячейку для комнаты.");
                    }
                }
            }

            // Создаём сетку комнат
            foreach (var room in rooms)
            {
                for (var x = 0; x < room.Width; x++)
                {
                    for (var y = 0; y < room.Height; y++)
                    {
                        var nodeX = x + room.PositionX * ROOM_CELL_SIZE;
                        var nodeY = x + room.PositionY * ROOM_CELL_SIZE;
                        var node = new HexNode(nodeX, nodeY);
                        room.Nodes.Add(node);
                        map.Nodes.Add(node);

                        var neighbors = HexNodeHelper.GetNeighbors(node, room.Nodes);

                        foreach (var neighbor in neighbors)
                        {
                            var currentEdge = GetExistsEdge(map, (HexNode)node, neighbor);

                            if (currentEdge != null)
                            {
                                continue;
                            }

                            AddEdgeToMap(map, (HexNode)node, neighbor);
                        }
                    }
                }
            }

            // Соединяем комнаты
            foreach(var room in rooms)
            {
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var selectedRoom = _randomSource.RollConnectedRoom(room, rooms);

                var startNode = room.Nodes.First();
                var finishNode = selectedRoom.Nodes.First();

                //Строим коридор
                var currentX = startNode.OffsetX;
                var currentY = startNode.OffsetY;
                while (true)
                {
                    if (currentX >= finishNode.OffsetX)
                    {
                        currentX--;
                    }
                    else
                    {
                        currentX++;
                    }

                    if (currentY >= finishNode.OffsetY)
                    {
                        currentY--;
                    }
                    else
                    {
                        currentY++;
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт на карте ребро, соединяющее два узла этой карты.
        /// </summary>
        /// <param name="targetMap"> Целевая карта, для которой нужно создать ребро. </param>
        /// <param name="node"> Исходное ребро карты. </param>
        /// <param name="neighbor"> Соседнее ребро карты, с которым будет соединено исходное. </param>
        private static void AddEdgeToMap(IMap targetMap, HexNode node, HexNode neighbor)
        {
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
        private static Edge GetExistsEdge(IMap map, HexNode node, HexNode neighbor)
        {
            return (Edge)(from edge in map.Edges
                          where edge.Nodes.Contains(node)
                          where edge.Nodes.Contains(neighbor)
                          select edge).SingleOrDefault();
        }
    }
}
