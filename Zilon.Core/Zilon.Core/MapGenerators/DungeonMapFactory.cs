using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class DungeonMapFactory : IMapFactory
    {
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly RoomGeneratorSettings _settings;

        [ExcludeFromCodeCoverage]
        public DungeonMapFactory([NotNull] ISectorGeneratorRandomSource randomSource) :
            this(randomSource, new RoomGeneratorSettings())
        {
        }

        [ExcludeFromCodeCoverage]
        public DungeonMapFactory([NotNull] ISectorGeneratorRandomSource randomSource,
            [NotNull] RoomGeneratorSettings settings)
        {
            _randomSource = randomSource ?? throw new System.ArgumentNullException(nameof(randomSource));
            _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
        }

        public IMap Create()
        {
            var map = CreateMapInstance();

            var roomGenerator = new RoomGenerator(_randomSource, _settings);

            var edgeHash = new HashSet<string>();

            // Генерируем комнаты в сетке
            var rooms = roomGenerator.GenerateRoomsInGrid();

            // Создаём узлы и рёбра комнат
            roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // разбиваем комнаты на группы по назначению.
            var startRoom = rooms.First();
            var exitRoom = rooms.Last();
            var mainRooms = rooms.Skip(1).Take(rooms.Count - 2).ToArray();

            // Указание регионов карты
            var regionId = 1;
            foreach (var room in rooms)
            {
                var region = new MapRegion(regionId, room.Nodes.ToArray());
                regionId++;
                map.Regions.Add(region);

                if (room == startRoom)
                {
                    map.StartRegion = region;
                    map.StartNodes = region
                        .Nodes
                        .Take(1)
                        .ToArray();
                }

                if (room == exitRoom)
                {
                    map.ExitRegion = region;
                    map.ExitNodes = region
                        .Nodes
                        .Skip(region.Nodes.Count() - 2)
                        .Take(1)
                        .ToArray();
                }
            }

            return map;
        }

        private static IMap CreateMapInstance()
        {
            return new HexMap(200);
        }
    }
}
