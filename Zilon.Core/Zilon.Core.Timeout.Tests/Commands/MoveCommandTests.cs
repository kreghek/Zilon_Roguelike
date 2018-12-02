using NUnit.Framework;
using Zilon.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using Zilon.Core.Tactics;
using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.MapGenerators;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.Props;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tests.Common;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;
using JetBrains.Annotations;

namespace Zilon.Core.Commands.Tests
{
    [TestFixture]
    [Category("Timeout")]
    public class MoveCommandTests
    {
        private ServiceContainer _container;

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

            sectorManager.CreateSector();



            var personScheme = schemeService.GetScheme<IPersonScheme>("captain");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Nodes.First();
            var playerActorVm = CreateHumanActorVm(humanPlayer,
                personScheme,
                actorManager,
                playerActorStartNode);

            //Лучше централизовать переключение текущего актёра только в playerState
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActor(playerState.ActiveActor.Actor);



            var currentActorNode = (HexNode)playerState.ActiveActor.Actor.Node;
            var nextNodes = HexNodeHelper.GetNeighbors(currentActorNode, sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>());
            var moveTargetNode = nextNodes.First();

            playerState.HoverViewModel = new TestNodeViewModel {
                Node = moveTargetNode
            };
        }

        private IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode)
        {
            var schemeService = _container.GetInstance<ISchemeService>();


            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, inventory);




            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            var actorViewModel = new TestActorViewModel
            {
                Actor = actor
            };

            return actorViewModel;
        }

        public void SetUp()
        {
            _container.Register<IDice>(factory => new Dice(), new PerContainerLifetime()); // инстанцируем явно из-за 2-х конструкторов.
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<ISectorGeneratorRandomSource, SectorGeneratorRandomSource>(new PerContainerLifetime());
            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());
            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            _container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());

            _container.Register<HumanPlayer>(new PerContainerLifetime());
            _container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());

            _container.Register<ISchemeLocator, FileSchemeLocator>(new PerContainerLifetime());

            _container.Register<IGameLoop, GameLoop>(new PerContainerLifetime());
            _container.Register<ICommandManager, QueueCommandManager>(new PerContainerLifetime());
            _container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            _container.Register<IActorTaskSource, MonsterActorTaskSource>(serviceName: "monster", lifetime: new PerContainerLifetime());
            _container.Register<ISectorProceduralGenerator, SectorProceduralGenerator>(new PerContainerLifetime());
            _container.Register<IMapFactory, DungeonMapFactory>(new PerContainerLifetime());
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
    }
}