using System;
using System.Configuration;
using System.Linq;

using JetBrains.Annotations;

using LightInject;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Commands.Tests
{
    [TestFixture]
    [Category("Timeout")]
    public class MoveCommandTests
    {
        private ServiceContainer _container;

        /// <summary>
        /// Тест нужен для того, чтобы замерять производительность одного шага перемещения игрока по карте.
        /// </summary>
        [Test]
        public void MoveCommandTest()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<IPlayerState>();
            var moveCommand = _container.GetInstance<ICommand>("move-command");
            var schemeService = _container.GetInstance<ISchemeService>();
            var humanPlayer = _container.GetInstance<HumanPlayer>();
            var actorManager = _container.GetInstance<IActorManager>();
            var humanActorTaskSource = _container.GetInstance<IHumanActorTaskSource>();
            var commandManger = _container.GetInstance<ICommandManager>();

            sectorManager.CreateSector();



            var personScheme = schemeService.GetScheme<IPersonScheme>("captain");

            var playerActorStartNode = sectorManager.CurrentSector.Map.StartNodes.First();
            var playerActorVm = CreateHumanActorVm(humanPlayer,
                personScheme,
                actorManager,
                playerActorStartNode);

            //Лучше централизовать переключение текущего актёра только в playerState
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActor(playerState.ActiveActor.Actor);

            var currentActorNode = (HexNode)playerState.ActiveActor.Actor.Node;
            var nextNodes = HexNodeHelper.GetSpatialNeighbors(currentActorNode, sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>());
            var moveTargetNode = nextNodes.First();

            playerState.HoverViewModel = new TestNodeViewModel
            {
                Node = moveTargetNode
            };

            commandManger.Push(moveCommand);

            ICommand command = null;
            do
            {
                command = commandManger.Pop();

                try
                {
                    command?.Execute();
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
                }
            } while (command != null);


        }

        private IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode)
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var survivalRandomSource = _container.GetInstance<ISurvivalRandomSource>();


            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                defaultActScheme,
                evolutionData,
                survivalRandomSource,
                inventory);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            var actorViewModel = new TestActorViewModel
            {
                Actor = actor
            };

            return actorViewModel;
        }

        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            _container.Register<IDice>(factory => new Dice(123), new PerContainerLifetime());
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(new PerContainerLifetime());
            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());
            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            _container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ISurvivalRandomSource, SurvivalRandomSource>(new PerContainerLifetime());
            _container.Register<IChestGenerator, ChestGenerator>(new PerContainerLifetime());
            _container.Register<IChestGeneratorRandomSource, ChestGeneratorRandomSource>(new PerContainerLifetime());

            _container.Register<HumanPlayer>(new PerContainerLifetime());
            _container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());

            _container.Register<ISchemeLocator>(factory => CreateSchemeLocator(), new PerContainerLifetime());

            _container.Register<IGameLoop, GameLoop>(new PerContainerLifetime());
            _container.Register<ICommandManager, QueueCommandManager>(new PerContainerLifetime());
            _container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            _container.Register<IActorTaskSource, MonsterActorTaskSource>(serviceName: "monster", lifetime: new PerContainerLifetime());
            _container.Register<ISectorProceduralGenerator, SectorProceduralGenerator>(new PerContainerLifetime());
            _container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
            _container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(new PerContainerLifetime());
            _container.Register<IMapFactory, RoomMapFactory>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(new PerContainerLifetime());

            _container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            //_container.Register<ISectorModalManager>(factory => GetSectorModalManager(), new PerContainerLifetime());


            // Специализированные сервисы для Ui.
            _container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());

            // Комманды актёра.
            _container.Register<ICommand, MoveCommand>(serviceName: "move-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, AttackCommand>(serviceName: "attack-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, OpenContainerCommand>(serviceName: "open-container-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, NextTurnCommand>(serviceName: "next-turn-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, UseSelfCommand>(serviceName: "use-self-command", lifetime: new PerContainerLifetime());

            // Комадны для UI.
            _container.Register<ICommand, ShowContainerModalCommand>(serviceName: "show-container-modal-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, ShowInventoryModalCommand>(serviceName: "show-inventory-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, ShowPerksModalCommand>(serviceName: "show-perks-command", lifetime: new PerContainerLifetime());

            // Специализированные команды для Ui.
            _container.Register<ICommand, EquipCommand>(serviceName: "show-container-modal-command");
            _container.Register<ICommand, PropTransferCommand>(serviceName: "show-container-modal-command");
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }
    }
}