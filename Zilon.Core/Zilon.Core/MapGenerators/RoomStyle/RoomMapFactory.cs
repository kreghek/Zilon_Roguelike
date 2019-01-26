using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class RoomMapFactory : IMapFactory
    {
        private readonly IRoomGenerator _roomGenerator;

        [ExcludeFromCodeCoverage]
        public RoomMapFactory([NotNull] IRoomGenerator roomGenerator)
        {
            _roomGenerator = roomGenerator;
        }

        public IMap Create()
        {
            var map = CreateMapInstance();

            var edgeHash = new HashSet<string>();

            // Генерируем случайные координаты комнат
            var rooms = _roomGenerator.GenerateRoomsInGrid();

            // Создаём узлы и рёбра комнат
            _roomGenerator.CreateRoomNodes(map, rooms, edgeHash);

            // Соединяем комнаты
            _roomGenerator.BuildRoomCorridors(map, rooms, edgeHash);

            // разбиваем комнаты на группы по назначению.
            var startRoom = rooms.First();
            var exitRoom = rooms.Last();

            // Указание регионов карты
            var regionId = 1;
            foreach (var room in rooms)
            {
                var region = new MapRegion(regionId, room.Nodes.Cast<IMapNode>().ToArray());
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
