using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.TextClient
{
    /// <summary>
    /// Main game screen handler.
    /// All game in this screen.
    /// </summary>
    internal class MainScreenHandler : IScreenHandler
    {
        private static ISectorNode GetPlayerSectorNode(IPlayer player, IGlobe globe)
        {
            return (from sectorNode in globe.SectorNodes
                    from actor in sectorNode.Sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).SingleOrDefault();
        }

        private static void HandleAttackCommand(string inputText, IServiceScope serviceScope, ISectorUiState uiState,
            ISectorNode playerActorSectorNode, ICommandPool commandManager)
        {
            var components = inputText.Split(' ');
            var targetId = int.Parse(components[1], CultureInfo.InvariantCulture);

            if (components.Length == 3)
            {
                var targetMarker = components[2].ToUpper(CultureInfo.InvariantCulture);
                switch (targetMarker)
                {
                    case "A":
                        SelectActor(uiState, playerActorSectorNode, targetId);
                        break;

                    case "S":
                        SelectStaticObject(uiState, playerActorSectorNode, targetId);
                        break;

                    default:
                        SelectActor(uiState, playerActorSectorNode, targetId);
                        break;
                }
            }
            else
            {
                SelectActor(uiState, playerActorSectorNode, targetId);
            }

            var acts = uiState.ActiveActor.Actor.Person.GetModule<ICombatActModule>().CalcCombatActs();
            uiState.TacticalAct = acts
                .OrderBy(x => x.Equipment is null)
                .First(x => x.Constrains is null);

            var command = serviceScope.ServiceProvider.GetRequiredService<AttackCommand>();
            PushCommandToExecution(commandManager, command);
        }

        private static void HandleDeadCommand(IPlayer player)
        {
            var survivalModule = player.MainPerson.GetModule<ISurvivalModule>();
            survivalModule.SetStatForce(Core.Persons.SurvivalStatType.Health, 0);
        }

        private static void HandleIdleCommand(IServiceScope serviceScope, ICommandPool commandManager)
        {
            var command = serviceScope.ServiceProvider.GetRequiredService<IdleCommand>();

            PushCommandToExecution(commandManager, command);
        }

        private static void HandleLookCommand(ISectorUiState uiState, ISectorNode playerActorSectorNode,
            string inputText)
        {
            var isDetailed = inputText.Equals("look2", StringComparison.InvariantCultureIgnoreCase);

            var nextMoveNodes = playerActorSectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node);
            var actorFow = uiState.ActiveActor.Actor.Person.GetModule<IFowData>();
            var fowNodes = actorFow.GetSectorFowData(playerActorSectorNode.Sector)
                .Nodes.Where(x => x.State == SectorMapNodeFowState.Observing)
                .Select(x => x.Node);

            var fowNodesAll = actorFow.GetSectorFowData(playerActorSectorNode.Sector)
                .Nodes.Where(x =>
                    x.State == SectorMapNodeFowState.Explored || x.State == SectorMapNodeFowState.Observing)
                .Select(x => x.Node);

            PrintLocationName(playerActorSectorNode);

            PrintLookLegend();
            Console.WriteLine();

            Console.WriteLine($"{UiResource.NodesLabel}:");
            Console.WriteLine();

            foreach (var node in fowNodes)
            {
                var sb = new StringBuilder(node.ToString());
                var poi = false;

                if (nextMoveNodes.Contains(node))
                {
                    poi = true;
                    sb.Append($" {UiResource.NextNodeMarker}");
                }

                if (playerActorSectorNode.Sector.Map.Transitions.TryGetValue(node, out var _))
                {
                    poi = true;
                    sb.Append($" {UiResource.TransitionNodeMarker}");
                }

                var undiscoveredNodes = playerActorSectorNode.Sector.Map.GetNext(node)
                    .Where(x => !fowNodesAll.Contains(x));
                if (undiscoveredNodes.Any())
                {
                    poi = true;
                    sb.Append($" {UiResource.UndiscoveredNextNodeMarker}");
                }

                var monsterInNode =
                    playerActorSectorNode.Sector.ActorManager.Items.SingleOrDefault(x => x.Node == node);
                if (monsterInNode != null && monsterInNode != uiState.ActiveActor.Actor)
                {
                    poi = true;
                    sb.Append(
                        $" {UiResource.MonsterNodeMarker} {monsterInNode.Person.Id}:{monsterInNode.Person}");
                }

                var staticObjectInNode =
                    playerActorSectorNode.Sector.StaticObjectManager.Items.SingleOrDefault(x => x.Node == node);
                if (staticObjectInNode != null)
                {
                    poi = true;
                    sb.Append(
                        $" {UiResource.StaticObjectNodeMarker} {staticObjectInNode.Id}:{staticObjectInNode.Purpose}");
                }

                if (isDetailed)
                {
                    Console.WriteLine(sb.ToString());
                }
                else if (poi)
                {
                    Console.WriteLine(sb.ToString());
                }
            }
        }

        private static void HandleMoveCommand(
            IServiceScope serviceScope,
            ISectorUiState uiState,
            ICommandPool commandManager,
            ISectorNode playerActorSectorNode,
            string inputText)
        {
            var components = inputText.Split(' ');
            var x = int.Parse(components[1], CultureInfo.InvariantCulture);
            var y = int.Parse(components[2], CultureInfo.InvariantCulture);
            var offsetCoords = new OffsetCoords(x, y);

            ISectorMap map = playerActorSectorNode.Sector.Map;

            var targetNode = map.Nodes.OfType<HexNode>()
                .SingleOrDefault(node => node.OffsetCoords == offsetCoords);

            var command = serviceScope.ServiceProvider.GetRequiredService<MoveCommand>();

            uiState.SelectedViewModel = new NodeViewModel { Node = targetNode };

            PushCommandToExecution(commandManager, command);
        }

        private static void HandleSectorTransitCommand(IServiceScope serviceScope, ICommandPool commandManager)
        {
            var command = serviceScope.ServiceProvider.GetRequiredService<SectorTransitionMoveCommand>();
            PushCommandToExecution(commandManager, command);
        }

        private static void PrintCommandDescriptions()
        {
            Console.WriteLine(UiResource.MoveCommandDescription);
            Console.WriteLine(UiResource.TransitionCommandDescription);
            Console.WriteLine(UiResource.LookCommandDescription);
            Console.WriteLine(UiResource.AttackCommandDescription);
            Console.WriteLine(UiResource.IdleCommandDescription);
            Console.WriteLine(UiResource.DeadCommandDescription);
            Console.WriteLine(UiResource.ExitCommandDescription);
        }

        private static void PrintLocationName(ISectorNode playerActorSectorNode)
        {
            var locationScheme = playerActorSectorNode.Biome.LocationScheme;
            var scheme = playerActorSectorNode.SectorScheme;

            var sectorName = $"{locationScheme.Name} {scheme.Name}".Trim();

            Console.WriteLine($"Current Level: {sectorName}");
        }

        private static void PrintLookLegend()
        {
            Console.WriteLine($"{UiResource.NextNodeMarker} - {UiResource.NextNodeMarkerDescription}");
            Console.WriteLine(
                $"{UiResource.UndiscoveredNextNodeMarker} - {UiResource.UndiscoveredNextNodeMarkerDescription}");
            Console.WriteLine($"{UiResource.TransitionNodeMarker} - {UiResource.TransitionNodeMarkerDescription}");
        }

        private static void PrintState(IActor actor)
        {
            Console.WriteLine(new string('=', 10));
            if (actor.Person.GetModule<IConditionModule>().Items.Any())
            {
                Console.WriteLine($"{UiResource.EffectsLabel}:");
                foreach (var сondition in actor.Person.GetModule<IConditionModule>().Items)
                {
                    Console.WriteLine(сondition);
                }
            }

            Console.WriteLine($"Position: {actor.Node}");
        }

        private static void PushCommandToExecution(ICommandPool commandManager, ICommand command)
        {
            if (command.CanExecute().IsSuccess)
            {
                commandManager.Push(command);
            }
            else
            {
                Console.WriteLine(UiResource.CommandCantExecuteMessage);
            }
        }

        private static void SelectActor(ISectorUiState uiState, ISectorNode playerActorSectorNode, int targetId)
        {
            var actorManager = playerActorSectorNode.Sector.ActorManager;

            var targetObject = actorManager.Items.SingleOrDefault(x => x.Id == targetId);

            uiState.SelectedViewModel = new ActorViewModel { Actor = targetObject };
        }

        private static void SelectStaticObject(ISectorUiState uiState, ISectorNode playerActorSectorNode, int targetId)
        {
            var entitiesManager = playerActorSectorNode.Sector.StaticObjectManager;

            var targetObject = entitiesManager.Items.SingleOrDefault(x => x.Id == targetId);

            uiState.SelectedViewModel = new StaticObjectViewModel { StaticObject = targetObject };
        }

        /// <inheritdoc />
        public async Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            var serviceScope = gameState.ServiceScope;
            var player = serviceScope.ServiceProvider.GetRequiredService<IPlayer>();
            var commandPool = serviceScope.ServiceProvider.GetRequiredService<ICommandPool>();
            var animationBlockerService = serviceScope.ServiceProvider.GetRequiredService<IAnimationBlockerService>();
            var humanTaskSource = serviceScope.ServiceProvider
                .GetRequiredService<IActorTaskSource<ISectorTaskSourceContext>>();

            var playerState = serviceScope.ServiceProvider.GetRequiredService<ISectorUiState>();
            var inventoryState = serviceScope.ServiceProvider.GetRequiredService<IInventoryState>();

            var globeLoopUpdateContext = new GlobeLoopContext(player);
            var globeLoop = new GlobeLoopUpdater(globeLoopUpdateContext, animationBlockerService);

            var humanTaskSourceCasted = (IHumanActorTaskSource<ISectorTaskSourceContext>)humanTaskSource;
            var commandLoopContext = new CommandLoopContext(player, humanTaskSourceCasted, animationBlockerService);
            var commandLoop = new CommandLoopUpdater(commandLoopContext, commandPool);

            var globe = player.Globe;

            // Play

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            globeLoop.ErrorOccured += (s, e) => { Console.WriteLine(e.Exception); };
            globeLoop.Start();

            commandLoop.ErrorOccured += (s, e) => { Console.WriteLine(e.Exception); };
            commandLoop.CommandAutoExecuted += (s, e) => { Console.WriteLine("Auto execute last command"); };
            commandLoop.CommandProcessed += (s, e) =>
            {
                inventoryState.SelectedProp = null;
                playerState.SelectedViewModel = null;
            };
            var commandLoopTask = commandLoop.StartAsync(cancellationToken);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopTask.ContinueWith(task => Console.WriteLine(task.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
            commandLoopTask.ContinueWith(task => Console.WriteLine("Game loop stopped."),
                TaskContinuationOptions.OnlyOnCanceled);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            do
            {
                var playerActor = (from sectorNode in globe.SectorNodes
                                   from actor in sectorNode.Sector.ActorManager.Items
                                   where actor.Person == player.MainPerson
                                   select actor).SingleOrDefault();

                playerState.ActiveActor = new ActorViewModel { Actor = playerActor };

                PrintState(playerState.ActiveActor.Actor);
                PrintCommandDescriptions();

                Console.WriteLine($"{UiResource.InputPrompt}:");
                var inputText = Console.ReadLine();
                if (inputText.StartsWith("m", StringComparison.InvariantCultureIgnoreCase))
                {
                    var playerActorSectorNode = GetPlayerSectorNode(player, globe);

                    HandleMoveCommand(serviceScope, playerState, commandPool, playerActorSectorNode, inputText);
                }
                else if (inputText.StartsWith("look", StringComparison.InvariantCultureIgnoreCase))
                {
                    var playerActorSectorNode = GetPlayerSectorNode(player, globe);

                    HandleLookCommand(playerState, playerActorSectorNode, inputText);
                }
                else if (inputText.StartsWith("idle", StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleIdleCommand(serviceScope, commandPool);
                }
                else if (inputText.StartsWith("dead", StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleDeadCommand(player);
                }
                else if (inputText.StartsWith("transit", StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleSectorTransitCommand(serviceScope, commandPool);
                }
                else if (inputText.StartsWith("attack", StringComparison.InvariantCultureIgnoreCase))
                {
                    var playerActorSectorNode = GetPlayerSectorNode(player, globe);

                    HandleAttackCommand(inputText, serviceScope, playerState, playerActorSectorNode, commandPool);
                }
                else if (inputText.StartsWith("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (((IHumanActorTaskSource<ISectorTaskSourceContext>)humanTaskSource).CanIntent()
                            && !commandLoop.HasPendingCommands())
                        {
                            break;
                        }
                    }
                }).ConfigureAwait(false);

                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            } while (!player.MainPerson.GetModule<ISurvivalModule>().IsDead);

            cancellationTokenSource.Cancel();
            return GameScreen.Scores;
        }
    }
}