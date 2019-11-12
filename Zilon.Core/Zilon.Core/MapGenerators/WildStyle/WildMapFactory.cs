using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.WildStyle
{
    /// <summary>
    /// Реализация фабрики для построения дикого сектора.
    /// </summary>
    /// <seealso cref="IMapFactory" />
    public class WildMapFactory : IMapFactory
    {
        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <param name="options">Параметры создания карты.</param>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        public Task<ISectorMap> CreateAsync(object options)
        {
            const int START_ROOM_SIZE = 2;

            var mapSize = (int)options;

            ISectorMap map = new SectorHexMap();
            MapFiller.FillSquareMap(map, mapSize);

            var startRoomLeftBorder = mapSize / 2 - START_ROOM_SIZE / 2;
            var startRoomTopBorder = mapSize / 2 - START_ROOM_SIZE / 2;
            var startRoomRightBorder = startRoomLeftBorder + START_ROOM_SIZE - 1;
            var startRoomBottomBorder = startRoomTopBorder + START_ROOM_SIZE - 1;

            var startNodes = map.Nodes.OfType<HexNode>()
                .Where(node => (startRoomLeftBorder <= node.OffsetX && node.OffsetX <= startRoomRightBorder)
                && (startRoomTopBorder <= node.OffsetY && node.OffsetY <= startRoomBottomBorder))
                .ToArray();
            var startRegion = new MapRegion(1, startNodes)
            {
                IsStart = true,
            };

            var outerNodes = map.Nodes.Where(x => !startNodes.Contains(x)).ToArray();
            var outerRegion = new MapRegion(2, outerNodes)
            {
                IsOut = true,
                ExitNodes = new[] { outerNodes.Last() }
            };

            map.Regions.Add(startRegion);
            map.Regions.Add(outerRegion);

            map.Transitions.Add(outerNodes.Last(), RoomTransition.CreateGlobalExit());

            return Task.FromResult(map);
        }

        /// <summary>
        /// Вспомогательный метод для создания карты дикого сектора без создания экземпляра фабрики.
        /// </summary>
        /// <param name="mapSize"> Размер карты. </param>
        /// <returns> Возвращает объект карты. </returns>
        public static async Task<ISectorMap> CreateAsync(int mapSize)
        {
            var factory = new WildMapFactory();
            return await factory.CreateAsync((object)mapSize).ConfigureAwait(false);
        }
    }
}
