using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.TextClient
{
    /// <summary>
    /// Main game screen handler.
    /// All game in this screen.
    /// </summary>
    internal class MainScreenHandler : IScreenHandler
    {
        private static void PrintState(IActor actor)
        {
            Console.WriteLine(new string('=', 10));
            if (actor.Person.GetModule<IEffectsModule>().Items.Any())
            {
                Console.WriteLine($"{UiResource.EffectsLabel}:");
                foreach (var effect in actor.Person.GetModule<IEffectsModule>().Items)
                {
                    Console.WriteLine(effect);
                }
            }

            Console.WriteLine($"Position:{actor.Node}");
        }

        /// <inheritdoc />
        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            var serviceScope = gameState.ServiceScope;
            var player = serviceScope.ServiceProvider.GetRequiredService<IPlayer>();

            var gameLoop = new GameLoop(player.Globe, player);
            var globe = player.Globe;
            var uiState = serviceScope.ServiceProvider.GetRequiredService<ISectorUiState>();
            var playerActor = (from sectorNode in globe.SectorNodes
                               from actor in sectorNode.Sector.ActorManager.Items
                               where actor.Person == player.MainPerson
                               select actor).SingleOrDefault();
            var playerActorSectorNode = (from sectorNode in globe.SectorNodes
                                         from actor in sectorNode.Sector.ActorManager.Items
                                         where actor.Person == player.MainPerson
                                         select sectorNode).SingleOrDefault();

            uiState.ActiveActor = new ActorViewModel { Actor = playerActor };

            // Play

            using var cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            gameLoop.StartProcessAsync(cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            do
            {
                PrintState(uiState.ActiveActor.Actor);

                Console.WriteLine(UiResource.MoveCommandDescription);
                Console.WriteLine(UiResource.LookCommandDescription);
                Console.WriteLine(UiResource.IdleCommandDescription);
                Console.WriteLine(UiResource.DeadCommandDescription);
                Console.WriteLine(UiResource.ExitCommandDescription);

                Console.WriteLine($"{UiResource.InputPrompt}:");
                var inputText = Console.ReadLine();
                if (inputText.StartsWith("m", StringComparison.InvariantCultureIgnoreCase))
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

                    if (command.CanExecute())
                    {
                        command.Execute();
                    }
                    else
                    {
                        Console.WriteLine(UiResource.CommandCantExecuteMessage);
                    }
                }

                if (inputText.StartsWith("look", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nextMoveNodes = playerActorSectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node);
                    var actorFow = uiState.ActiveActor.Actor.Person.GetModule<IFowData>();
                    var fowNodes = actorFow.GetSectorFowData(playerActorSectorNode.Sector)
                        .Nodes.Where(x => x.State == SectorMapNodeFowState.Observing)
                        .Select(x => x.Node);

                    Console.WriteLine($"{UiResource.NodesLabel}:");
                    Console.WriteLine();
                    foreach (var node in fowNodes)
                    {
                        Console.Write(node);

                        if (nextMoveNodes.Contains(node))
                        {
                            Console.Write($" {UiResource.NextNodeMarker}");
                        }

                        if (playerActorSectorNode.Sector.Map.Transitions.TryGetValue(node, out var _))
                        {
                            Console.Write($" {UiResource.TransitionNodeMarker}");
                        }

                        var monsterInNode =
                            playerActorSectorNode.Sector.ActorManager.Items.SingleOrDefault(x => x.Node == node);
                        if (monsterInNode != null && monsterInNode != uiState.ActiveActor.Actor)
                        {
                            Console.Write($" {UiResource.MonsterNodeMarker} {monsterInNode.Person.Id}:{monsterInNode.Person}");
                        }

                        var staticObjectInNode =
                            playerActorSectorNode.Sector.StaticObjectManager.Items.SingleOrDefault(x => x.Node == node);
                        if (staticObjectInNode != null)
                        {
                            Console.Write($" {UiResource.StaticObjectNodeMarker} {staticObjectInNode.Id}:{staticObjectInNode.Purpose}");
                        }

                        Console.WriteLine();
                    }
                }

                if (inputText.StartsWith("idle", StringComparison.InvariantCultureIgnoreCase))
                {
                    var command = serviceScope.ServiceProvider.GetRequiredService<IdleCommand>();

                    if (command.CanExecute())
                    {
                        command.Execute();
                    }
                    else
                    {
                        Console.WriteLine(UiResource.CommandCantExecuteMessage);
                    }
                }

                if (inputText.StartsWith("dead", StringComparison.InvariantCultureIgnoreCase))
                {
                    var survivalModule = player.MainPerson.GetModule<ISurvivalModule>();
                    survivalModule.SetStatForce(Core.Persons.SurvivalStatType.Health, 0);
                }

                if (inputText.StartsWith("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            } while (!player.MainPerson.GetModule<ISurvivalModule>().IsDead);

            cancellationTokenSource.Cancel();
            return Task.FromResult(GameScreen.Scores);
        }
    }
}