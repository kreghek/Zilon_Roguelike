using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class DungeonMapFactory : IMapFactory
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly IBotPlayer _botPlayer;
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;

        public StringBuilder Log { get; }

        public DungeonMapFactory(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ISectorGeneratorRandomSource randomSource,
            IBotPlayer botPlayer,
            ISchemeService schemeService,
            IDropResolver dropResolver)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _randomSource = randomSource;
            _botPlayer = botPlayer;
            _schemeService = schemeService;
            _dropResolver = dropResolver;

            Log = new StringBuilder();
        }

        public IMap Create()
        {
            var map = new HexMap();

            var roomGenerator = new RoomGenerator(_randomSource, Log);

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
            foreach (var room in rooms)
            {
                var region = new MapRegion(room.Nodes.ToArray());
                map.Regions.Add(region);
            }

            return map;
        }
    }
}
