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
        void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Создаёт узлы комнат на карте.
        /// </summary>
        /// <param name="map"> Карта, в рамках которой происходит генерация. </param>
        /// <param name="rooms"> Комнаты, для которых создаются узлы графа карты. </param>
        /// <param name="edgeHash"> Хэш рёбер. Нужен для оптимизации при создании узлов графа карты. </param>
        void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Генерация комнат.
        /// </summary>
        /// <param name="roomCount"> Количество комнат, которые будут сгенерированы. </param>
        /// <param name="roomMinSize"> Минимальный размер комнаты. </param>
        /// <param name="roomMaxSize"> Максимальный размер комнаты. </param>
        /// <returns> Возвращает набор созданных комнат. </returns>
        IEnumerable<Room> GenerateRoomsInGrid(
            int roomCount,
            int roomMinSize,
            int roomMaxSize,
            IEnumerable<RoomTransition> availableTransitions);
    }
}