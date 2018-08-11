//TODO перевести на SpecFlow

//using FluentAssertions;

//using JetBrains.Annotations;

//using LightInject;

//using NUnit.Framework;

//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;

//using Zilon.Core.CommonServices.Dices;
//using Zilon.Core.Persons;
//using Zilon.Core.Players;
//using Zilon.Core.Schemes;
//using Zilon.Core.Spec;
//using Zilon.Core.Tactics.Behaviour;
//using Zilon.Core.Tactics.Behaviour.Bots;
//using Zilon.Core.Tactics.Spatial;
//using Zilon.Core.Tests.TestCommon;

//namespace Zilon.Core.Tactics.Tests
//{
//    [TestFixture]
//    [Category("Spec")]
//    public class PerkTests
//    {
//        private ServiceContainer _container;

//        /// <summary>
//        /// Тест проверяет, что при выполнении работы на уничтожение перк прокаивается.
//        /// </summary>
//        [Test(Description = "Ttc3")] // Tactic test case 3
//        public void ActorDefeats_PerkArchieved()
//        {
//            // ARRANGE
//            GenerateSectorTtc3Content();
//            var sector = _container.GetInstance<ISector>();
//            var humanTaskSource = _container.GetInstance<HumanActorTaskSource>();


//            // ACT
//            sector.Update();




//            // ASSERT
//            var perk = humanTaskSource.CurrentActor.Person.EvolutionData.Perks[0];
//            perk.CurrentLevel.Primary.Should().Be(0);
//            perk.CurrentLevel.Sub.Should().Be(0);
//        }

//        private void GenerateSectorTtc3Content()
//        {
//            var schemeService = _container.GetInstance<ISchemeService>();
//            var map = _container.GetInstance<IMap>();
//            var sector = _container.GetInstance<ISector>();


//            // Подготовка игроков
//            var humanPlayer = new HumanPlayer();
//            var botPlayer = new BotPlayer();

//            var personScheme = schemeService.GetScheme<PersonScheme>("captain");
//            var monsterScheme = schemeService.GetScheme<MonsterScheme>("default");

//            // Подготовка актёров
//            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);
//            var humanActor = CreateHumanActor(humanPlayer, personScheme, humanStartNode);

//            var enemy1StartNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);
//            var enemy1Actor = CreateMonsterActor(botPlayer, monsterScheme, enemy1StartNode);


//            // Подготовка маршрутов патрулирования
//            var patrolMapNodes1 = new IMapNode[] {
//                map.Nodes.Cast<HexNode>().SelectBy(2, 2),
//                map.Nodes.Cast<HexNode>().SelectBy(2, 10)
//            };
//            var patrolRoute1 = CreateRoute(patrolMapNodes1);

           
//            var routeDictionary = new Dictionary<IActor, IPatrolRoute>
//            {
//                { enemy1Actor, patrolRoute1 }
//            };


//            // Подготовка источника поведения ботов
//            var decisionSource = _container.GetInstance<IDecisionSource>();
//            var tacticalActUsageService = _container.GetInstance<ITacticalActUsageService>();

//            var humanTaskSource = new HumanActorTaskSource(decisionSource, tacticalActUsageService);
//            humanTaskSource.SwitchActor(humanActor);
//            humanTaskSource.IntentAttack(enemy1Actor);

//            var botTaskSource = new MonsterActorTaskSource(botPlayer, routeDictionary, decisionSource, tacticalActUsageService);

//            _container.Register(factory => humanTaskSource);


//            ((Sector)sector).BehaviourSources = new IActorTaskSource[]
//            {
//                humanTaskSource,
//                botTaskSource
//            };
//        }

//        private IActor CreateMonsterActor([NotNull] IPlayer player,
//            [NotNull] MonsterScheme monsterScheme,
//            [NotNull] IMapNode startNode)
//        {
//            var actorManager = _container.GetInstance<IActorManager>();

//            var person = new MonsterPerson(monsterScheme);

//            var actor = new Actor(person, player, startNode);

//            actorManager.Add(actor);

//            return actor;
//        }

//        private IActor CreateHumanActor([NotNull] IPlayer player,
//            [NotNull] PersonScheme personScheme,
//            [NotNull] IMapNode startNode)
//        {
//            var actorManager = _container.GetInstance<IActorManager>();
//            var schemeService = _container.GetInstance<ISchemeService>();
//            var propFactory = _container.GetInstance<IPropFactory>();

//            var perkScheme = schemeService.GetScheme<PerkScheme>("battle-dogmas");
//            var predefinedPerks = new IPerk[] {
//                new Perk{
//                    Scheme = perkScheme,
//                    CurrentJobs = new []{
//                        new PerkJob(perkScheme.Levels[0].Jobs[0]){
//                            Progress = 4
//                        }
//                    }
//                }
//            };
//            var evolutionData = new PresetEvolutionData(schemeService, predefinedPerks);

//            var person = new HumanPerson(personScheme, evolutionData);

//            var actor = new Actor(person, player, startNode);

//            actorManager.Add(actor);

//            // Указываем экипировку по умолчанию.
//            var propScheme = schemeService.GetScheme<PropScheme>("short-sword");
//            var equipment = propFactory.CreateEquipment(propScheme);
//            actor.Person.EquipmentCarrier.SetEquipment(equipment, 0);

//            return actor;
//        }

//        private IPatrolRoute CreateRoute(IMapNode[] mapNodes)
//        {
//            var patrolRoute = new PatrolRoute(mapNodes);

//            return patrolRoute;
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            _container = new ServiceContainer();

//            RegisterSchemeService();
//            RegisterMap();
//            RegisterAuxServices();

//            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
//            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
//            _container.Register<ISector, Sector>(new PerContainerLifetime());
//        }

//        /// <summary>
//        /// Подготовка дополнительных сервисов
//        /// </summary>
//        private void RegisterAuxServices()
//        {
//            _container.Register<IDice>(factory => new Dice(), new PerContainerLifetime());
//            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
//            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
//            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
//            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
//        }

//        private void RegisterMap()
//        {
//            _container.Register<IMap>(factory =>
//            {
//                var map = new TestGridGenMap(2);
//                return map;
//            });
//        }

//        private void RegisterSchemeService()
//        {
//            _container.Register<ISchemeLocator>(factory =>
//            {
//                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

//                var schemeLocator = new FileSchemeLocator(schemePath);

//                return schemeLocator;
//            }, new PerContainerLifetime());

//            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

//            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
//        }
//    }
//}