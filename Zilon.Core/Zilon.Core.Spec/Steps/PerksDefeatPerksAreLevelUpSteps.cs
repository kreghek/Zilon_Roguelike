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
    public class PerksDefeatPerksAreLevelUpSteps
    {
        private ServiceContainer _container;
        private IActor _humanActor;
        private IActor _enemy1Actor;
        private HumanActorTaskSource _humanTaskSource;

        [Given(@"Квадратная карта")]
        public void GivenКвадратнаяКарта()
        {
            _container = new ServiceContainer();

            RegisterSchemeService();
            RegisterMap();
            RegisterAuxServices();

            
            _container.Register<ISector, Sector>(new PerContainerLifetime());
        }

        [Given(@"Два актёра на расстоянии атаки")]
        public void GivenДваАктёраНаРасстоянииАтаки()
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var map = _container.GetInstance<IMap>();
            var sector = _container.GetInstance<ISector>();


            // Подготовка игроков
            var humanPlayer = new HumanPlayer();
            var botPlayer = new BotPlayer();

            var personScheme = schemeService.GetScheme<PersonScheme>("captain");
            var monsterScheme = schemeService.GetScheme<MonsterScheme>("default");

            // Подготовка актёров
            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);
            _humanActor = CreateHumanActor(humanPlayer, personScheme, humanStartNode);

            var enemy1StartNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);
            _enemy1Actor = CreateMonsterActor(botPlayer, monsterScheme, enemy1StartNode);


            // Подготовка маршрутов патрулирования
            var patrolMapNodes1 = new IMapNode[] {
                            map.Nodes.Cast<HexNode>().SelectBy(2, 2)
                        };
            var patrolRoute1 = CreateRoute(patrolMapNodes1);


            var routeDictionary = new Dictionary<IActor, IPatrolRoute>
                        {
                            { _enemy1Actor, patrolRoute1 }
                        };


            // Подготовка источника поведения ботов
            var decisionSource = _container.GetInstance<IDecisionSource>();
            var tacticalActUsageService = _container.GetInstance<ITacticalActUsageService>();

            _humanTaskSource = new HumanActorTaskSource();
            _humanTaskSource.SwitchActor(_humanActor);

            var botTaskSource = new MonsterActorTaskSource(botPlayer, routeDictionary, decisionSource, tacticalActUsageService);

            ((Sector)sector).BehaviourSources = new IActorTaskSource[]
            {
                _humanTaskSource,
                botTaskSource
            };
        }

        [Given(@"У актёра игрока прогресс перка на убийство (.*) из (.*)")]
        public void GivenУАктёраИгрокаПрогрессПеркаНаУбийствоИз(int p0, int p1)
        {
            const string perkSid = "battle-dogmas";

            var perk = _humanActor.Person.EvolutionData.Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentJobs[0].Progress = p0;
        }
        
        [When(@"Я атакую вражеского актёра")]
        public void WhenЯАтакуюВражескогоАктёра()
        {
            var useService = _container.GetInstance<ITacticalActUsageService>();

            var intention = new Intention<AttackTask>(a => new AttackTask(a, _enemy1Actor, useService));
            _humanTaskSource.Intent(intention);

            var sector = _container.GetInstance<ISector>();
            sector.Update();
        }
        
        [Then(@"перк на убийство должен быть прокачен")]
        public void ThenПеркНаУбийствоДолженБытьПрокачен()
        {
            var perk = _humanActor.Person.EvolutionData.Perks[0];
            perk.CurrentLevel.Primary.Should().Be(0);
            perk.CurrentLevel.Sub.Should().Be(0);
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices()
        {
            _container.Register<IDice>(factory => new Dice(398), new PerContainerLifetime());
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());

            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
        }

        private void RegisterMap()
        {
            _container.Register<IMap>(factory =>
            {
                var map = new TestGridGenMap(2);
                return map;
            });
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

        private IActor CreateMonsterActor([NotNull] IPlayer player,
            [NotNull] MonsterScheme monsterScheme,
            [NotNull] IMapNode startNode)
        {
            var actorManager = _container.GetInstance<IActorManager>();

            var person = new MonsterPerson(monsterScheme);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            return actor;
        }

        private IActor CreateHumanActor([NotNull] IPlayer player,
            [NotNull] PersonScheme personScheme,
            [NotNull] IMapNode startNode)
        {
            var actorManager = _container.GetInstance<IActorManager>();
            var schemeService = _container.GetInstance<ISchemeService>();
            var propFactory = _container.GetInstance<IPropFactory>();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<TacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            // Указываем экипировку по умолчанию.
            var propScheme = schemeService.GetScheme<PropScheme>("short-sword");
            var equipment = propFactory.CreateEquipment(propScheme);
            actor.Person.EquipmentCarrier.SetEquipment(equipment, 0);

            return actor;
        }

        private IPatrolRoute CreateRoute(IMapNode[] mapNodes)
        {
            var patrolRoute = new PatrolRoute(mapNodes);

            return patrolRoute;
        }
    }
}
