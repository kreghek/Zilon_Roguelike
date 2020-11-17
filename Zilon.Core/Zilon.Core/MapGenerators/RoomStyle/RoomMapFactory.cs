using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Graphs;
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

        private static ISectorMap CreateMapInstance()
        {
            return new SectorHexMap();
        }

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        public Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new System.ArgumentNullException(nameof(generationOptions));
            }

            var roomFactoryOptions = generationOptions.OptionsSubScheme as ISectorRoomMapFactoryOptionsSubScheme;

            if (roomFactoryOptions is null)
            {
                throw new System.ArgumentException("Не задана схема генерации в настройках", nameof(generationOptions));
            }

            var map = CreateMapInstance();

            var edgeHash = new HashSet<string>();

            // Генерируем случайные координаты комнат
            var transitions = generationOptions.Transitions;

            var rooms = _roomGenerator.GenerateRoomsInGrid(roomFactoryOptions.RegionCount,
                RoomMinSize,
                roomFactoryOptions.RegionSize,
                transitions);

            // Создаём узлы и рёбра комнат
            _roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            _roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // Указание регионов карты
            var regionIdCounter = 1;
            foreach (var room in rooms)
            {
                var passableRoomNodes = room.Nodes;
                var region = new MapRegion(regionIdCounter, passableRoomNodes.Cast<IGraphNode>().ToArray());
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
                }
            }

            return Task.FromResult(map);
        }
    }
}