using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class DungeonMapFactory : IMapFactory
    {
        private readonly ISectorGeneratorRandomSource _randomSource;

        public DungeonMapFactory(ISectorGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public IMap Create()
        {
            var map = CreateMapInstance();

            var roomGenerator = new RoomGenerator(_randomSource);

            var edgeHash = new HashSet<string>();

            // Генерируем комнаты в сетке
            var rooms = roomGenerator.GenerateRoomsInGrid();
            var mainRooms = rooms.Where(x => x != roomGenerator.StartRoom).ToArray();

            // Создаём узлы и рёбра комнат
            roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // Указание регионов карты
            foreach (var room in rooms)
            {
                var region = new MapRegion(room.Nodes.ToArray());
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
            return new HexMap();
        }
    }
}
