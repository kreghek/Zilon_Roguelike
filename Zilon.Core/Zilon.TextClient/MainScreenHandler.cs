using System;
using System.Linq;
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
    internal class MainScreenHandler : IScreenHandler
    {
        private static void PrintState(IActor actor)
        {
            Console.WriteLine(new string('=', 10));
            if (actor.Person.GetModule<IEffectsModule>().Items.Any())
            {
                Console.WriteLine("Effects:");
                foreach (var effect in actor.Person.GetModule<IEffectsModule>().Items)
                {
                    Console.WriteLine(effect);
                }
            }

            Console.WriteLine($"Position:{actor.Node}");
        }

        public async Task<GameScreen> StartProcessingAsync(IServiceScope serviceScope)
        {
            var player = serviceScope.ServiceProvider.GetRequiredService<IPlayer>();

            var gameLoop = new GameLoop(player.Globe);
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            gameLoop.StartProcessAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            do
            {
                PrintState(uiState.ActiveActor.Actor);

                Console.WriteLine("- \"move x y\" to move person.");
                Console.WriteLine("- \"look\" to get detailet info around.");
                Console.WriteLine("- \"idle\" to wait some time.");
                Console.WriteLine("- \"dead\" to dead :)");
                Console.WriteLine("- \"exit\" to quit the game.");

                Console.WriteLine("Input command:");
                var inputText = Console.ReadLine();
                if (inputText.StartsWith("m"))
                {
                    var components = inputText.Split(' ');
                    var x = int.Parse(components[1]);
                    var y = int.Parse(components[2]);
                    var offsetCoords = new OffsetCoords(x, y);

                    ISectorMap map = playerActorSectorNode.Sector.Map;

                    var targetNode = map.Nodes.OfType<HexNode>()
                        .SingleOrDefault(node => node.OffsetCoords == offsetCoords);

                    var command = serviceScope.ServiceProvider.GetRequiredService<MoveCommand>();

                    uiState.SelectedViewModel = new NodeViewModel { Node = targetNode };

                    command.Execute();
                }

                if (inputText.StartsWith("look"))
                {
                    var nextMoveNodes = playerActorSectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node);
                    var actorFow = uiState.ActiveActor.Actor.Person.GetModule<IFowData>();
                    var fowNodes = actorFow.GetSectorFowData(playerActorSectorNode.Sector).Nodes.Select(x => x.Node);

                    Console.WriteLine("Nodes:");
                    Console.WriteLine();
                    foreach (var node in fowNodes)
                    {
                        Console.Write(node);

                        if (nextMoveNodes.Contains(node))
                        {
                            Console.Write(" 1");
                        }

                        if (playerActorSectorNode.Sector.Map.Transitions.TryGetValue(node, out var _))
                        {
                            Console.Write(" t");
                        }

                        var monsterInNode =
                            playerActorSectorNode.Sector.ActorManager.Items.SingleOrDefault(x => x.Node == node);
                        if (monsterInNode != null && monsterInNode != uiState.ActiveActor.Actor)
                        {
                            Console.Write($" monster {monsterInNode.Person}");
                        }

                        var staticObjectInNode =
                            playerActorSectorNode.Sector.StaticObjectManager.Items.SingleOrDefault(x => x.Node == node);
                        if (staticObjectInNode != null)
                        {
                            Console.Write($" object in node {staticObjectInNode.Purpose}");
                        }

                        Console.WriteLine();
                    }
                }

                if (inputText.StartsWith("idle"))
                {
                    var command = serviceScope.ServiceProvider.GetRequiredService<NextTurnCommand>();
                    command.Execute();
                }

                if (inputText.StartsWith("dead"))
                {
                    var survivalModule = player.MainPerson.GetModule<ISurvivalModule>();
                    survivalModule.SetStatForce(Core.Persons.SurvivalStatType.Health, 0);
                }

                if (inputText.StartsWith("exit"))
                {
                    break;
                }
            } while (!player.MainPerson.GetModule<ISurvivalModule>().IsDead);

            return GameScreen.GlobeSelection;
        }
    }
}