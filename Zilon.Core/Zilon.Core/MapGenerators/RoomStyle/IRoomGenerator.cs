using System.Collections.Generic;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Генератор комнат.
    /// </summary>
    public interface IRoomGenerator
    {
        /// <summary>
        /// Соединяет комнаты коридорами.
        /// </summary>
        /// <param name="map"> Карта, в рамках которой происходит генерация. </param>
        /// <param name="rooms"> Существующие комнаты. </param>
        /// <param name="edgeHash"> Хэш рёбер. Нужен для оптимизации при создании узлов графа карты. </param>
        void BuildRoomCorridors(IMap map, List<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Создаёт узлы комнат на карте.
        /// </summary>
        /// <param name="map"> Карта, в рамках которой происходит генерация. </param>
        /// <param name="rooms"> Комнаты, для которых создаются узлы графа карты. </param>
        /// <param name="edgeHash"> Хэш рёбер. Нужен для оптимизации при создании узлов графа карты. </param>
        void CreateRoomNodes(IMap map, List<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Генерация комнат.
        /// </summary>
        /// <returns> Возвращает набор козданных комнат. </returns>
        List<Room> GenerateRoomsInGrid();
    }
}