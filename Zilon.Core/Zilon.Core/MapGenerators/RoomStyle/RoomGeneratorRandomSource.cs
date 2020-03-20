using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Реализация источника рандома для генератора комнат сектора.
    /// </summary>
    /// <seealso cref="Zilon.Core.MapGenerators.RoomStyle.IRoomGeneratorRandomSource" />
    public class RoomGeneratorRandomSource : IRoomGeneratorRandomSource
    {
        private readonly IDice _dice;
        private readonly IDice _roomSizeDice;

        public RoomGeneratorRandomSource(IDice dice, IDice roomSizeDice)
        {
            _dice = dice;
            _roomSizeDice = roomSizeDice;
        }

        /// <summary>
        /// Выбирает комнаты, с которыми есть соединение.
        /// </summary>
        /// <param name="currentRoom">Текущая комната, для которой ищуются соединённые соседи.</param>
        /// <param name="maxNeighbors">Максимальное количество соединённых соседей.</param>
        /// <param name="availableRooms">Набор доступных для соединения соседенй.</param>
        /// <returns>
        /// Возвращает целевые комнаты для соединения.
        /// </returns>
        [NotNull, ItemNotNull]
        public Room[] RollConnectedRooms(Room currentRoom, int maxNeighbors, IList<Room> availableRooms)
        {
            var openRooms = new List<Room>(availableRooms);
            var selectedRooms = new HashSet<Room>();
            var neighborCount = _dice.Roll(1, maxNeighbors);
            for (var i = 0; i < neighborCount; i++)
            {
                var rolledRoomIndex = _dice.Roll(0, openRooms.Count - 1);
                var selectedRoom = openRooms[rolledRoomIndex];
                selectedRooms.Add(selectedRoom);
                openRooms.Remove(selectedRoom);

                if (!openRooms.Any())
                {
                    break;
                }
            }

            return selectedRooms.ToArray();
        }

        /// <summary>Выбрасывает случаный набор элементов интерьера комнаты.</summary>
        /// <param name="roomWidth">Ширина комнаты.</param>
        /// <param name="roomHeight">Высота комнаты.</param>
        /// <returns>Возвращает набор элементов интерьера комнаты.</returns>
        public RoomInteriorObjectMeta[] RollInteriorObjects(int roomWidth, int roomHeight)
        {
            if (roomWidth <= 2 || roomHeight <= 2)
            {
                return new RoomInteriorObjectMeta[0];
            }

            const int PASS_PADDING = 1;

            var minHorizontalBorder = 0;
            var maxHorizontalBorder = roomWidth - 1;
            var minVerticalBorder = 0;
            var maxVerticalBorder = roomHeight - 1;

            var leftBorder = minHorizontalBorder + PASS_PADDING;
            var topBorder = minVerticalBorder + PASS_PADDING;

            var rightBorder = maxHorizontalBorder - PASS_PADDING;
            var bottomBorder = maxVerticalBorder - PASS_PADDING;

            var list = new List<RoomInteriorObjectMeta>();
            var maxCount = (roomWidth * roomHeight) / 9;
            var count = _dice.Roll(0, maxCount);
            for (var i = 0; i < count; i++)
            {
                var x = _dice.Roll(leftBorder, rightBorder);
                var y = _dice.Roll(topBorder, bottomBorder);

                var same = list.SingleOrDefault(o => o.Coords.CompsEqual(x, y));
                if (same != null)
                {
                    continue;
                }

                var interiorObjectCoords = new OffsetCoords(x, y);
                var interiorObject = new RoomInteriorObjectMeta(interiorObjectCoords);

                list.Add(interiorObject);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Выбрасывает случайный набор уникальных координат в матрице комнат указаной длины.
        /// </summary>
        /// <param name="roomGridSize">Размер матрицы комнат.</param>
        /// <param name="roomCount">Количество комнат в секторе.</param>
        /// <returns>
        /// Возвращает массив координат из матрицы комнат.
        /// </returns>
        public IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount)
        {
            var roomMatrixPosList = new List<OffsetCoords>(roomGridSize * roomGridSize);
            for (var x = 0; x < roomGridSize; x++)
            {
                for (var y = 0; y < roomGridSize; y++)
                {
                    roomMatrixPosList.Add(new OffsetCoords(x, y));
                }
            }

            var rolledCoords = new List<OffsetCoords>(roomCount);
            for (var i = 0; i < roomCount; i++)
            {
                var rolledIndex = _dice.Roll(0, roomMatrixPosList.Count - 1);
                var currentRolledCoords = roomMatrixPosList[rolledIndex];
                rolledCoords.Add(currentRolledCoords);
                roomMatrixPosList.RemoveAt(rolledIndex);
            }

            return rolledCoords;
        }

        /// <summary>
        /// Возвращает матрицу смежности между комнатами (сеть комнат).
        /// </summary>
        /// <param name="rooms">Всё комнаты, которые должны быть соединены в сеть.</param>
        /// <param name="maxNeighbors">Максимальное количество соседей у комнаты.</param>
        /// <returns>
        /// Возвращает словарь, представляющий собой матрицу смежности комнат.
        /// Минимальное число соседей - 1. Максимальное - не превышает указанное в аргументе значение.
        /// </returns>
        public IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors)
        {
            var result = new Dictionary<Room, Room[]>();

            var roomsInGraph = new Dictionary<Room, int>();
            var roomsNotInGraph = new List<Room>(rooms);
            while (roomsNotInGraph.Any())
            {
                var room = roomsNotInGraph.First();
                roomsInGraph.Add(room, 0);
                roomsNotInGraph.Remove(room);
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var availableRooms = roomsInGraph
                    .Where(x => x.Key != room && x.Value < maxNeighbors)
                    .Select(x => x.Key)
                    .ToArray();

                if (!availableRooms.Any())
                {
                    continue;
                }

                var selectedRooms = RollConnectedRooms(room,
                    maxNeighbors,
                    availableRooms);

                if (!selectedRooms.Any())
                {
                    //Значит текущая комната тупиковая
                    continue;
                }

                result.Add(room, selectedRooms);
                foreach (var selectedRoom in selectedRooms)
                {
                    roomsInGraph[selectedRoom]++;
                }
            }

            return result;
        }

        /// <summary>
        /// Выбрасывает случайный размер комнаты.
        /// </summary>
        /// <param name="minSize">Минимальный размер комнаты.</param>
        /// <param name="maxSize">Максимальный размер комнаты.</param>
        /// <returns>
        /// Возвращает размер с произвольными шириной и высотой в диапазоне (minSize, maxSize).
        /// </returns>
        /// <remarks>
        /// Источник рандома возвращает случайный размер комнаты в указанном диапазоне.
        /// </remarks>
        public Size[] RollRoomSize(int minSize, int maxSize, int count)
        {
            var sizeList = new Size[count];

            for (var i = 0; i < count; i++)
            {
                var diffSize = maxSize - minSize;

                var rollDiffWidth = _roomSizeDice.Roll(diffSize);
                var rollDiffHeight = _roomSizeDice.Roll(diffSize);

                var rollWidth = (int)rollDiffWidth + minSize;
                var rollHeight = (int)rollDiffHeight + minSize;

                var size = new Size(rollWidth, rollHeight);

                sizeList[i] = size;
            }

            return sizeList;
        }

        public HexNode RollTransitionNode(IEnumerable<HexNode> openRoomNodes)
        {
            var index = _dice.Roll(0, openRoomNodes.Count() - 1);
            return openRoomNodes.ElementAt(index);
        }

        public IEnumerable<RoomTransition> RollTransitions(IEnumerable<RoomTransition> openTransitions)
        {
            var index = _dice.Roll(0, openTransitions.Count() - 1);
            return new[] { openTransitions.ElementAt(index) };
        }
    }
}
