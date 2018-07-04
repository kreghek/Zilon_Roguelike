using System.Collections.Generic;

namespace Zilon.Core.Tactics.Generation
{
    public interface ISectorGeneratorRandomSource
    {
        OffsetCoords RollRoomPosition(int maxPosition);
        Size RollRoomSize(int maxSize);

        /// <summary>
        /// Выбирает комнаты, с которыми есть соединение.
        /// </summary>
        /// <param name="room"> Текущая комната. </param>
        /// <param name="maxNeighbors"> Максимальное количество соединённых соседей. </param>
        /// <param name="p"> Веротяность в процентах, с которой выбирается сосед. 0-100. </param>
        /// <param name="rooms"> Набор доступных комнат для соединения. </param>
        /// <returns> Возвращает соединённые комнаты. </returns>
        Room[] RollConnectedRooms(Room room, int maxNeighbors, int p, IList<Room> rooms);
    }
}
