using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class SectorProceduralGenerator : ISectorProceduralGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly IBotPlayer _botPlayer;
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;
        private readonly IMapFactory _mapFactory;
        private readonly IChestGenerator _chestGenerator;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        public SectorProceduralGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IBotPlayer botPlayer,
            ISchemeService schemeService,
            IDropResolver dropResolver,
            IMapFactory mapFactory,
            IChestGenerator chestGenerator,
            ISurvivalRandomSource survivalRandomSource)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _botPlayer = botPlayer;
            _schemeService = schemeService;
            _dropResolver = dropResolver;
            _mapFactory = mapFactory;
            _chestGenerator = chestGenerator;
            _survivalRandomSource = survivalRandomSource;
        }

        public ISector Generate(ISectorGeneratorOptions options)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);

            var proceduralOptions = (SectorProceduralGeneratorOptions)options;

            var monsterRegions = map.Regions.Where(x => x != map.StartRegion);

            _monsterGenerator.CreateMonsters(sector,
                monsterRegions,
                proceduralOptions.MonsterGeneratorOptions);

            //var sector = new Sector(map,
            //    _actorManager,
            //    _propContainerManager,
            //    _dropResolver,
            //    _schemeService);

            
            //CreateRoomMonsters(sector,
            //    monsterRegions,
            //    (SectorProceduralGeneratorOptions)options);

            _chestGenerator.CreateChests(map, monsterRegions);

            return sector;
        }

        private void CreateRoomMonsters(ISector sector,
            IEnumerable<MapRegion> regions,
            SectorProceduralGeneratorOptions options)
        {
            var monsterScheme = _schemeService.GetScheme<IMonsterScheme>("rat");

            //TODO Учесть вероятность, что монстр может инстанцироваться на сундук
            foreach (var region in regions)
            {
                // В каждую комнату генерируем по 2 монстра
                // первый ходит по маршруту

                var startNode1 = (HexNode)region.Nodes.FirstOrDefault();
                var actor1 = CreateMonster(monsterScheme, startNode1);

                var finishPatrolNode = region.Nodes.Last();
                var patrolRoute = new PatrolRoute(startNode1, finishPatrolNode);
                sector.PatrolRoutes[actor1] = patrolRoute;

                // второй произвольно бродит

                var startNode2 = (HexNode)region.Nodes.Skip(3).FirstOrDefault();
                CreateMonster(monsterScheme, startNode2);
            }
        }

        private IActor CreateMonster(IMonsterScheme monsterScheme, HexNode startNode)
        {
            var person = new MonsterPerson(monsterScheme, _survivalRandomSource);
            var actor = new Actor(person, _botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }
    }
}
