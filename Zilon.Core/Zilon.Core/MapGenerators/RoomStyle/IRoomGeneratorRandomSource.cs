using System.Collections.Generic;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Интерфейс источника рандома для генератора карт из комнат.
    /// </summary>
    public interface IRoomGeneratorRandomSource
    {
        /// <summary>
        /// Выбрасывает случайный индекс команты из списка указанной длины.
        /// </summary>
        /// <param name="maxPosition"> Максимальное количество комнат в списке. </param>
        /// <returns> Возвращает случайное число в диапазоне (0, maxPosition). </returns>
        int RollRoomPositionIndex(int maxPosition);

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
