using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class RoomMapFactory : IMapFactory
    {
        private const int RoomMinSize = 2;
        private readonly IRoomGenerator _roomGenerator;

        [ExcludeFromCodeCoverage]
        public RoomMapFactory([NotNull] IRoomGenerator roomGenerator)
        {
            _roomGenerator = roomGenerator;
        }

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        public Task<ISectorMap> CreateAsync(object options)
        {
            var sectorScheme = (ISectorSubScheme)options;

            var map = CreateMapInstance();

            var edgeHash = new HashSet<string>();

            // Генерируем случайные координаты комнат
            var transitions = CreateTransitions(sectorScheme);

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
                var region = new MapRegion(regionIdCounter, room.Nodes.Cast<IMapNode>().ToArray());
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

        private static IEnumerable<RoomTransition> CreateTransitions(ISectorSubScheme sectorScheme)
        {
            if (sectorScheme.TransSectorSids == null)
            {
                return new[] { RoomTransition.CreateGlobalExit() };
            }

            return sectorScheme.TransSectorSids.Select(sid => new RoomTransition(sid));
        }

        private static ISectorMap CreateMapInstance()
        {
            return new SectorHexMap();
        }
    }
}
