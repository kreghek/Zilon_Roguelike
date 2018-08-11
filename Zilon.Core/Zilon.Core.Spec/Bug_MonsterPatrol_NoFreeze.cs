using System;
using System.Configuration;
using System.Linq;
using FluentAssertions;
using LightInject;
using Moq;
using TechTalk.SpecFlow;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec
{
    [Binding]
    public class Bug_MonsterPatrol_NoFreeze
    {
        private ServiceContainer _container;
        private BotPlayer _botPlayer;
        private SectorProceduralGenerator _generator;
        private Sector _sector;

        [Given(@"Произвольные комнаты всегда размером (.*)")]
        public void GivenЕстьКвадратнаяКарта(int roomSize)
        {
            _container = new ServiceContainer();

            RegisterAuxServices();
            RegisterSchemeService();
            RegisterSectorGenerationServices(roomSize);

            var dice = _container.GetInstance<IDice>();

            _botPlayer = new BotPlayer();

            _generator = CreateGenerator(_botPlayer);

            var map = new HexMap();
            var actorManager = new ActorManager();
            var propContainerManager = new PropContainerManager();

            _sector = new Sector(map, actorManager, propContainerManager);

            _generator.Generate(_sector, map);

            actorManager.Add(_generator.MonsterActors);
        }
        
        [Given(@"Время простоя монстров в ключевой точке всегда (.*)")]
        public void GivenВремяПростояМонстровВКлючевойТочкеВсегда(int idleDuration)
        {
            var dice = _container.GetInstance<IDice>();

            // Подготовка источника поведения ботов

            var decisionSourceMock = new Mock<DecisionSource>(dice).As<IDecisionSource>();
            decisionSourceMock.CallBase = true;
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((min, max) => 1);
            var decisionSource = decisionSourceMock.Object;
            var perkResolver = new PerkResolver();
            var tacticalActUsageService = new TacticalActUsageService(decisionSource, perkResolver);
            var botTaskSource = new MonsterActorTaskSource(_botPlayer, _generator.Patrols, decisionSource, tacticalActUsageService);


            _sector.BehaviourSources = new IActorTaskSource[]
            {
                botTaskSource
            };
        }
        
        [When(@"Обновляем состояние сектора (.*) раз")]
        public void WhenОбновляемСостояниеСектораРаз(int sectorUpdateCount)
        {
            for (var round = 0; round <= sectorUpdateCount; round++)
            {
                _sector.Update();
            }
        }

        [Then(@"Текущее местоположение монстров не должно совпадать с конечной точкой маршрута")]
        public void ThenТекущееМестоположениеМонстровНеДолжноСовпадатьСКонечнойТочкойМаршрута()
        {
            var actorManager = _container.GetInstance<IActorManager>();

            var monsters = actorManager.Actors.Where(x => x.Person is MonsterPerson).ToArray();

            foreach (var monster in monsters)
            {
                var monsterRoute = _generator.Patrols[monster];

                var lastRouteNode = monsterRoute.Points.Last();

                monster.Node.Should().NotBe(lastRouteNode);
            }
        }


        private void RegisterSectorGenerationServices(int roomSize)
        {
            var dice = _container.GetInstance<IDice>();


            var randomSourceMock = new Mock<SectorGeneratorRandomSource>(dice)
                .As<ISectorGeneratorRandomSource>();
            randomSourceMock.CallBase = true;
            randomSourceMock.Setup(x => x.RollRoomSize(It.IsAny<int>()))
                .Returns<int>((maxSize) => new Size(roomSize, roomSize));
            var randomSource = randomSourceMock.Object;


            _container.Register(factory => randomSource, new PerContainerLifetime());
        }

        private void RegisterAuxServices()
        {
            _container.Register<IDice>(factory => new Dice(123), new PerContainerLifetime());
            _container.Register<IActorManager, ActorManager>();
            _container.Register<IPropContainerManager, PropContainerManager>();

            _container.Register<IDropResolver, DropResolver>();
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>();
            _container.Register<IPropFactory, PropFactory>();
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

        private SectorProceduralGenerator CreateGenerator(BotPlayer botPlayer)
        {
            var dropResolver = _container.GetInstance<IDropResolver>();
            var randomSource = _container.GetInstance<ISectorGeneratorRandomSource>();
            var schemeService = _container.GetInstance<ISchemeService>();

            return new SectorProceduralGenerator(randomSource,
                botPlayer,
                schemeService,
                dropResolver);
        }
    }
}
