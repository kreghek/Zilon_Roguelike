using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class SectorProceduralGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly IPlayer _botPlayer;
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;

        public StringBuilder Log { get; }

        public SectorProceduralGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ISectorGeneratorRandomSource randomSource,
            IPlayer botPlayer,
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

        public void Generate(ISector sector, IMap map)
        {
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

            CreateRoomMonsters(sector, mainRooms);

            CreateChests(mainRooms);

            SelectStartNodes(sector, roomGenerator.StartRoom);

            SelectExitPoints(sector, roomGenerator.ExitRoom);
        }

        private void CreateChests(Room[] rooms)
        {
            var defaultDropTable = _schemeService.GetScheme<DropTableScheme>("default");
            var survivalDropTable = _schemeService.GetScheme<DropTableScheme>("survival");

            foreach (var room in rooms)
            {
                var containerNode = room.Nodes.FirstOrDefault();
                var container = new DropTablePropContainer(containerNode,
                    new[] { defaultDropTable, survivalDropTable },
                    _dropResolver);
                _propContainerManager.Add(container);
            }
        }

        private void SelectExitPoints(ISector sector, Room exitRoom)
        {
            sector.ExitNodes = new[] { exitRoom.Nodes.Last() };
        }

        private void SelectStartNodes(ISector sector, Room startRoom)
        {
            sector.StartNodes = startRoom.Nodes.Cast<IMapNode>().ToArray();
        }

        private void CreateRoomMonsters(ISector sector, IEnumerable<Room> rooms)
        {
            var monsterScheme = _schemeService.GetScheme<MonsterScheme>("default");

            foreach (var room in rooms)
            {
                var person = new MonsterPerson(monsterScheme);
                var startNode = room.Nodes.FirstOrDefault();
                var actor = new Actor(person, _botPlayer, startNode);
                _actorManager.Add(actor);

                var finishPatrolNode = room.Nodes.Last();
                var patrolRoute = new PatrolRoute(new[] { startNode, finishPatrolNode });
                sector.PatrolRoutes[actor] = patrolRoute;
            }
        }
    }
}
