using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class SectorProceduralGenerator
    {
        
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly IPlayer _botPlayer;
        private readonly ISchemeService _schemeService;
        private readonly IDice _dice;
        private readonly IPropFactory _propFactory;

        /// <summary>
        /// Стартовые узлы.
        /// Набор узлов, где могут располагаться актёры игрока
        /// на начало прохождения сектора.
        /// </summary>
        public IMapNode[] StartNodes { get; private set; }

        public List<IActor> MonsterActors { get; }

        public List<IPropContainer> Containers { get; }

        public Dictionary<IActor, IPatrolRoute> Patrols { get; }


        public StringBuilder Log { get; set; }

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource,
            IPlayer botPlayer,
            ISchemeService schemeService,
            //TODO Убрать эти зависимости, если возможно
            IDice dice,
            IPropFactory propFactory)
        {
            _randomSource = randomSource;
            _botPlayer = botPlayer;
            _schemeService = schemeService;
            _dice = dice;
            _propFactory = propFactory;
            Log = new StringBuilder();

            MonsterActors = new List<IActor>();
            Containers = new List<IPropContainer>();
            Patrols = new Dictionary<IActor, IPatrolRoute>();
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

            CreateRoomMonsters(mainRooms);

            CreateChests(mainRooms);

            SelectStartNodes(roomGenerator.StartRoom);

            SelectExitPoints(sector, roomGenerator.ExitRoom);
        }

        private void CreateChests(Room[] rooms)
        {
            var defaultDropTable = _schemeService.GetScheme<TrophyTableScheme>("default");

            foreach (var room in rooms)
            {
                var propScheme = _schemeService.GetScheme<PropScheme>("food-pack");
                var resource = new Resource(propScheme, 1);

                var containerNode = room.Nodes.FirstOrDefault();
                var container = new DropTablePropContainer(containerNode,
                    new[] { defaultDropTable },
                    _dice,
                    _schemeService,
                    _propFactory);
                Containers.Add(container);
            }
        }

        private void SelectExitPoints(ISector sector, Room exitRoom)
        {
            sector.ExitNodes = new[] { exitRoom.Nodes.Last() };
        }

        private void SelectStartNodes(Room startRoom)
        {
            StartNodes = startRoom.Nodes.Cast<IMapNode>().ToArray();
        }

        private void CreateRoomMonsters(IEnumerable<Room> rooms)
        {
            foreach (var room in rooms)
            {
                var person = new MonsterPerson();
                var startNode = room.Nodes.FirstOrDefault();
                var actor = new Actor(person, _botPlayer, startNode);
                MonsterActors.Add(actor);

                var finishPatrolNode = room.Nodes.Last();
                var patrolRoute = new PatrolRoute(new[] { startNode, finishPatrolNode });
                Patrols[actor] = patrolRoute;
            }
        }
    }
}
