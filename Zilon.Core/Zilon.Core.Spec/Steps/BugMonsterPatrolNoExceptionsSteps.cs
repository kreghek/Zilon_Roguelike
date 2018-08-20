using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public class BugMonsterPatrolNoExceptionsSteps
    {
        private ServiceContainer _container;
        private Sector _sector;

        [Given(@"Одна большая комната с несколькими стенами")]
        public void GivenОднаБольшаяКомнатаСНесколькимиСтенами()
        {
            _container = new ServiceContainer();

            RegisterAuxServices();
            RegisterSchemeService();

            var actorManager = _container.GetInstance<IActorManager>();
            var propContainerManager = _container.GetInstance<IPropContainerManager>();

            var map = new TestGridGenMap(15);
            // Подготовка карты
            map.Edges.RemoveAt(10);
            map.Edges.RemoveAt(20);
            map.Edges.RemoveAt(30);

            _sector = new Sector(map, actorManager, propContainerManager);
        }

        [Given(@"Два монстра с параллельными маршрутами")]
        public void GivenДваМонстраСПараллельнымиМаршрутами()
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var actorManager = _container.GetInstance<IActorManager>();
            var tacticalActUsageService = _container.GetInstance<ITacticalActUsageService>();
            var decisionSource = _container.GetInstance<IDecisionSource>();

            // Подготовка игроков
            var map = _sector.Map;
            var botPlayer = new BotPlayer();

            var monsterScheme = schemeService.GetScheme<MonsterScheme>("default");

            // Подготовка актёров
            var enemy1StartNode = map.Nodes.Cast<HexNode>().SelectBy(5, 5);
            var enemy1Actor = CreateMonsterActor(botPlayer, monsterScheme, actorManager, enemy1StartNode);

            var enemy2StartNode = map.Nodes.Cast<HexNode>().SelectBy(9, 9);
            var enemy2Actor = CreateMonsterActor(botPlayer, monsterScheme, actorManager, enemy2StartNode);


            // Подготовка маршрутов патрулирования
            var patrolMapNodes1 = new IMapNode[] {
                            map.Nodes.Cast<HexNode>().SelectBy(2, 2),
                            map.Nodes.Cast<HexNode>().SelectBy(2, 10)
                        };
            var patrolRoute1 = CreateRoute(patrolMapNodes1);

            var patrolMapNodes2 = new IMapNode[] {
                            map.Nodes.Cast<HexNode>().SelectBy(10, 2),
                            map.Nodes.Cast<HexNode>().SelectBy(10, 10)
                        };
            var patrolRoute2 = CreateRoute(patrolMapNodes2);

            var patrols = new Dictionary<IActor, IPatrolRoute>
                        {
                            { enemy1Actor, patrolRoute1 },
                            { enemy2Actor, patrolRoute2 }
                        };

            

            // Подготовка источника поведения ботов

            var botTaskSource = new MonsterActorTaskSource(botPlayer, patrols, decisionSource, tacticalActUsageService);
            _sector.BehaviourSources = new IActorTaskSource[]
            {
                botTaskSource
            };
        }

        [When(@"Обновляем состояние однокомнатного сектора (.*) раз")]
        public void WhenОбновляемСостояниеОднокомнатногоСектораРаз(int sectorUpdateCount)
        {
            for (var round = 0; round <= sectorUpdateCount; round++)
            {
                _sector.Update();
            }
        }
        
        [Then(@"Не было выброшено исключений")]
        public void ThenНеБылоВыброшеноИсключений()
        {
            var actorManager = _container.GetInstance<IActorManager>();

            // Если не было исключений, то тест считается пройденным.
            // Иначе теряем читаемый стек вызовов, оборачивая Update в делегат.
            var monsters = actorManager.Actors.Where(x => x.Person is MonsterPerson).ToArray();
            monsters.Should().NotBeEmpty();
        }

        private void RegisterAuxServices()
        {
            _container.Register<IDice>(factory => new Dice(123), new PerContainerLifetime());
            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());

            _container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());

            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
        }

        private void RegisterSchemeService()
        {
            _container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private IPatrolRoute CreateRoute(IMapNode[] mapNodes)
        {
            var patrolRoute = new PatrolRoute(mapNodes);

            return patrolRoute;
        }

        private IActor CreateMonsterActor([NotNull] IPlayer player,
            [NotNull] MonsterScheme monsterScheme,
            [NotNull] IActorManager actorManager,
            [NotNull] IMapNode startNode)
        {
            var person = new MonsterPerson(monsterScheme);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);


            return actor;
        }
    }
}
