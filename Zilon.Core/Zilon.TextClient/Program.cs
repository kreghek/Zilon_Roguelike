using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.World;

namespace Zilon.TextClient
{
    internal static class Program
    {
        private static async Task Main()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);

            serviceContainer.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceContainer.AddSingleton<IGlobeExpander>(provider =>
                (BiomeInitializer)provider.GetRequiredService<IBiomeInitializer>());
            serviceContainer.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceContainer.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceContainer.AddSingleton<IPlayer, HumanPlayer>();
            serviceContainer.AddScoped<MoveCommand>();
            serviceContainer.AddScoped<NextTurnCommand>();

            using var serviceProvider = serviceContainer.BuildServiceProvider();

            // Create globe

            using var scope = serviceProvider.CreateScope();

            var gameState = new GameState
            {
                CurrentScreen = GameScreen.GlobeSelection,
                ServiceScope = scope,
                ServiceProvider = serviceProvider
            };

            var mainScreenHandler = new MainScreenHandler();
            var globeSelectionScreenHandler = new GlobeSelectionScreenHandler();
            var scoresScreenHandler = new ScoresScreenHandler();

            IScreenHandler screenHandler = globeSelectionScreenHandler;

            do
            {
                var nextScreen = await screenHandler.StartProcessingAsync(gameState).ConfigureAwait(false);

                screenHandler = nextScreen switch
                {
                    GameScreen.GlobeSelection => globeSelectionScreenHandler,
                    GameScreen.Main => mainScreenHandler,
                    GameScreen.Scores => scoresScreenHandler,
                    _ => throw new InvalidOperationException($"Unsupported screen {nextScreen}.")
                };
            } while (true);
        }
    }
}