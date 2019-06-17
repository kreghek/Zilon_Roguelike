using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Генератор карты с комнатами.
    /// </summary>
    public sealed class RoomGenerator : IRoomGenerator
    {
        private readonly IRoomGeneratorRandomSource _randomSource;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="randomSource"> Источник рандома для генератора. </param>
        [ExcludeFromCodeCoverage]
        public RoomGenerator([NotNull] IRoomGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        }

        /// <summary>
        /// Генерация комнат.
        /// </summary>
        /// <param name="roomCount">Количество комнат, которые будут сгенерированы.</param>
        /// <param name="roomMinSize">Минимальный размер комнаты.</param>
        /// <param name="roomMaxSize">Максимальный размер комнаты.</param>
        /// <param name="availableTransitions"> Информация о переходах из данного сектора. </param>
        /// <returns>
        /// Возвращает набор созданных комнат.
        /// </returns>
        public IEnumerable<Room> GenerateRoomsInGrid(int roomCount,
            int roomMinSize,
            int roomMaxSize,
            IEnumerable<RoomTransition> availableTransitions)
        {
            // На 20 комнат будет матрица 6х6.
            var roomGridSize = (int)Math.Ceiling(Math.Log(roomCount, 2)) + 1;
            var roomGrid = new RoomMatrix(roomGridSize);

            var rooms = new List<Room>(roomCount);

            //Координаты не повторяются и их ровно roomCount.
            // Это гарантирует IroomGeneratorRandomSource
            var roomMatrixCoords = _randomSource.RollRoomMatrixPositions(roomGridSize, roomCount).ToArray();

            var openTransitions = new List<RoomTransition>(availableTransitions);

            var startAssigned = false;

            for (var i = 0; i < roomCount; i++)
            {
                var rolledPosition = roomMatrixCoords[i];

                var room = new Room
                {
                    PositionX = rolledPosition.X,
                    PositionY = rolledPosition.Y
                };

                roomGrid.SetRoom(rolledPosition.X, rolledPosition.Y, room);

                var rolledSize = _randomSource.RollRoomSize(roomMinSize, roomMaxSize);

                room.Width = rolledSize?.Width ?? 0;
                room.Height = rolledSize?.Height ?? 0;

                if (openTransitions.Any())
                {
                    var roomTransitions = _randomSource.RollTransitions(openTransitions);

                    room.Transitions.AddRange(roomTransitions);
                    openTransitions.RemoveAll(transition => roomTransitions.Contains(transition));
                }
                else if (!startAssigned)
                {
                    room.IsStart = true;
                    startAssigned = true;
                }

                rooms.Add(room);
            }

            return rooms;
        }

        /// <summary>
        /// Создаёт узлы комнат на карте.
        /// </summary>
        /// <param name="map">Карта, в рамках которой происходит генерация.</param>
        /// <param name="rooms">Комнаты, для которых создаются узлы графа карты.</param>
        /// <param name="edgeHash">Хэш рёбер. Нужен для оптимизации при создании узлов графа карты.</param>
        public void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            var cellSize = CalcCellSize(rooms);

            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room, cellSize);
            }
        }

        private Size CalcCellSize(IEnumerable<Room> rooms)
        {
            var maxWidth = rooms.Max(x => x.Width);
            var maxHeight = rooms.Max(x => x.Height);
            return new Size(maxWidth, maxHeight);
        }

        /// <summary>
        /// Соединяет комнаты коридорами.
        /// </summary>
        /// <param name="map">Карта, в рамках которой происходит генерация.</param>
        /// <param name="rooms">Существующие комнаты.</param>
        /// <param name="edgeHash">Хэш рёбер. Нужен для оптимизации при создании узлов графа карты.</param>
        public void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            var roomNet = _randomSource.RollRoomNet(rooms, 1);
            foreach (var roomPair in roomNet)
            {
                foreach (var selectedRoom in roomPair.Value)
                {
                    ConnectRoomsWithCorridor(map, roomPair.Key, selectedRoom, edgeHash);
                }
            }
        }

        private void CreateOneRoomNodes(ISectorMap map, HashSet<string> edgeHash, Room room, Size cellSize)
        {
            var interiorObjects = _randomSource.RollInteriorObjects(room.Width, room.Height);

            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + room.PositionX * cellSize.Width;
                    var nodeY = y + room.PositionY * cellSize.Height;

                    var isObstacle = false;
                    var interiorObjectForCoords = interiorObjects
                        .SingleOrDefault(o => o.Coords.CompsEqual(x, y));

                    if (interiorObjectForCoords != null)
                    {
//TODO Сделать так, чтобы укрытия не генерировались на узлах с выходами
// Как вариант - если выбираем узел, как выход, то снимаем флаг укрытия.
// Вообще, нужно поискать алгоритмы, которые бы расставляли укрытия и выходы, оставляя комнату проходимой.
                        isObstacle = true;
                    }

                    var node = new HexNode(nodeX, nodeY, isObstacle);


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

            // создаём переходы, если они есть в данной комнате
            if (room.Transitions.Any())
            {
                //TODO Отфильтровать узлы, которые на входах в коридор
                var availableNodes = room.Nodes.Where(x => !x.IsObstacle);
                var openRoomNodes = new List<HexNode>(availableNodes);
                foreach (var transition in room.Transitions)
                {
                    var transitionNode = _randomSource.RollTransitionNode(openRoomNodes);
                    map.Transitions.Add(transitionNode, transition);
                    openRoomNodes.Remove(transitionNode);
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
