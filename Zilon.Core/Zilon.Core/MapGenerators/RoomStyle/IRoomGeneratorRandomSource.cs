using System.Collections.Generic;

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
        /// выбрасывает случайный размер комнаты.
        /// </summary>
        /// <param name="maxSize"> Максимальный размер комнаты. </param>
        /// <returns> Возвращает размер с произвольными шириной и высотой в диапазоне (0, maxSize). </returns>
        Size RollRoomSize(int maxSize);

        /// <summary>
        /// Выбирает комнаты, с которыми есть соединение.
        /// </summary>
        /// <param name="room"> Текущая комната. </param>
        /// <param name="maxNeighbors"> Максимальное количество соединённых соседей. </param>
        /// <param name="rooms"> Набор доступных комнат для соединения. </param>
        /// <returns> Возвращает соединённые комнаты. </returns>
        Room[] RollConnectedRooms(Room room, int maxNeighbors, IList<Room> rooms);
    }
}
