using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Генератор карты с комнатами.
    /// </summary>
    public sealed class RoomGenerator : RoomGeneratorBase
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
        public override IEnumerable<Room> GenerateRoomsInGrid(int roomCount,
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

            var rolledRoomSizeList = _randomSource.RollRoomSize(roomMinSize, roomMaxSize, roomCount);

            for (var i = 0; i < roomCount; i++)
            {
                var rolledPosition = roomMatrixCoords[i];

                var room = new Room {PositionX = rolledPosition.X, PositionY = rolledPosition.Y};

                roomGrid.SetRoom(rolledPosition.X, rolledPosition.Y, room);

                var rolledSize = rolledRoomSizeList[i];

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
        public override void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (rooms is null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            if (edgeHash is null)
            {
                throw new ArgumentNullException(nameof(edgeHash));
            }

            var cellSize = RoomHelper.CalcCellSize(rooms);

            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room, cellSize);
            }
        }

        /// <summary>
        /// Соединяет комнаты коридорами.
        /// </summary>
        /// <param name="map">Карта, в рамках которой происходит генерация.</param>
        /// <param name="rooms">Существующие комнаты.</param>
        /// <param name="edgeHash">Хэш рёбер. Нужен для оптимизации при создании узлов графа карты.</param>
        public override void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
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
            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + room.PositionX * cellSize.Width;
                    var nodeY = y + room.PositionY * cellSize.Height;

                    var node = new HexNode(nodeX, nodeY);

                    room.Nodes.Add(node);
                    map.AddNode(node);

                    RoomHelper.AddAllNeighborToMap(map, edgeHash, room, node);
                }
            }

            CreateTransitions(map, room);
        }

        private void CreateTransitions(ISectorMap map, Room room)
        {
            // создаём переходы, если они есть в данной комнате
            if (room.Transitions.Any())
            {
                //TODO Отфильтровать узлы, которые на входах в коридор
                var availableNodes = room.Nodes;
                var openRoomNodes = new List<HexNode>(availableNodes);
                foreach (var transition in room.Transitions)
                {
                    var transitionNode = _randomSource.RollTransitionNode(openRoomNodes);
                    map.Transitions.Add(transitionNode, transition);
                    openRoomNodes.Remove(transitionNode);
                }
            }
        }
    }
}