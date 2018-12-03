using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class DungeonMapFactory : IMapFactory
    {
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly RoomGeneratorSettings _settings;

        public StringBuilder Log { get; }

        public DungeonMapFactory(ISectorGeneratorRandomSource randomSource) :
            this(randomSource, new RoomGeneratorSettings())
        {
        }

        public DungeonMapFactory(ISectorGeneratorRandomSource randomSource, RoomGeneratorSettings settings)
        {
            _randomSource = randomSource ?? throw new System.ArgumentNullException(nameof(randomSource));
            _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));

            Log = new StringBuilder();
        }

        public IMap Create()
        {
            var map = CreateMapInstance();

            var roomGenerator = new RoomGenerator(_randomSource, _settings, Log);

            Log.Clear();

            var edgeHash = new HashSet<string>();

            // Генерируем комнаты в сетке
            var rooms = roomGenerator.GenerateRoomsInGrid();
            var mainRooms = rooms.Where(x => x != roomGenerator.StartRoom).ToArray();

            // Создаём узлы и рёбра комнат
            roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // Указание регионов карты
            var regionId = 1;
            foreach (var room in rooms)
            {
                var region = new MapRegion(regionId, room.Nodes.ToArray());
                regionId++;
                map.Regions.Add(region);

                if (room == roomGenerator.StartRoom)
                {
                    map.StartRegion = region;
                    map.StartNodes = region
                        .Nodes
                        .Take(1)
                        .ToArray();
                }

                if (room == roomGenerator.ExitRoom)
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
