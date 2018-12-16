using System.Collections.Generic;

namespace Zilon.Core.MapGenerators
{
    public interface ISectorGeneratorRandomSource
    {
        int RollRoomPositionIndex(int maxPosition);

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
