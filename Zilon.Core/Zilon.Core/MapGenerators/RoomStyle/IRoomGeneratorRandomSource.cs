using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Интерфейс источника рандома для генератора карт из комнат.
    /// </summary>
    public interface IRoomGeneratorRandomSource
    {
        /// <summary>
        /// Выбрасывает случайный набор уникальных координат в матрице комнат указаной длины.
        /// </summary>
        /// <param name="roomGridSize"> Размер матрицы комнат. </param>
        /// <param name="roomCount"> Количество комнат в секторе. </param>
        /// <returns> Возвращает массив координат из матрицы комнат. </returns>
        IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount);

        /// <summary>
        /// Выбрасывает случайный размер комнаты минимального размера 2х2.
        /// </summary>
        /// <param name="maxSize"> Максимальный размер комнаты. </param>
        /// <returns> Возвращает размер с произвольными шириной и высотой в диапазоне (2, maxSize). </returns>
        /// <remarks>
        /// Источник рандома возвращает комнаты минимального размера 2х2.
        /// </remarks>
        Size RollRoomSize(int maxSize);

        /// <summary>
        /// Выбирает комнаты, с которыми есть соединение.
        /// </summary>
        /// <param name="currentRoom"> Текущая комната, для которой ищуются соединённые соседи. </param>
        /// <param name="maxNeighbors"> Максимальное количество соединённых соседей. </param>
        /// <param name="availableRooms"> Набор доступных для соединения соседенй. </param>
        /// <returns> Возвращает целевые комнаты для соединения. </returns>
        Room[] RollConnectedRooms(Room currentRoom, int maxNeighbors, IList<Room> availableRooms);

        IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors);
    }
}
