using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Реализация фабрики карты, онованной на комнатах.
    /// </summary>
    /// <seealso cref="IMapFactory" />
    public class RoomMapFactory : IMapFactory
    {
        private const int RoomMinSize = 2;
        private readonly IRoomGenerator _roomGenerator;

        [ExcludeFromCodeCoverage]
        public RoomMapFactory([NotNull] IRoomGenerator roomGenerator)
        {
            _roomGenerator = roomGenerator ?? throw new System.ArgumentNullException(nameof(roomGenerator));
        }

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        public Task<ISectorMap> CreateAsync(object options)
        {
            if (options is null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            var sectorScheme = (ISectorSubScheme)options;

            var map = CreateMapInstance();

            var edgeHash = new HashSet<string>();

            // Генерируем случайные координаты комнат
            var transitions = MapFactoryHelper.CreateTransitions(sectorScheme);

            var rooms = _roomGenerator.GenerateRoomsInGrid(sectorScheme.RegionCount,
                RoomMinSize,
                sectorScheme.RegionSize,
                transitions);

            // Создаём узлы и рёбра комнат
            _roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            _roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // Указание регионов карты
            var regionIdCounter = 1;
            foreach (var room in rooms)
            {
                var passableRoomNodes = room.Nodes.Where(x => !x.IsObstacle);
                var region = new MapRegion(regionIdCounter, passableRoomNodes.Cast<IMapNode>().ToArray());
                regionIdCounter++;
                map.Regions.Add(region);

                if (room.IsStart)
                {
                    region.IsStart = true;
                    continue;
                }

                if (room.Transitions?.Any() == true)
                {
                    region.ExitNodes = (from regionNode in region.Nodes
                                        where map.Transitions.Keys.Contains(regionNode)
                                        select regionNode).ToArray();

                    continue;
                }
            }

            return Task.FromResult(map);
        }

        private static ISectorMap CreateMapInstance()
        {
            return new SectorHexMap();
        }
    }
}
