using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Players;
using Zilon.Core.Scoring;

namespace Zilon.TextClient
{
    internal class ScoresScreenHandler : IScreenHandler
    {
        public Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            var scoreManager = gameState.ServiceScope.ServiceProvider.GetRequiredService<IScoreManager>();

            var summary = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);

            Console.WriteLine(summary);

            var player = gameState.ServiceScope.ServiceProvider.GetRequiredService<IPlayer>();
            player.Reset();

            Console.WriteLine(UiResource.PressEnterToContinuePropmpt);
            Console.ReadLine();

            gameState.ServiceScope.Dispose();

            gameState.ServiceScope = gameState.ServiceProvider.CreateScope();

            return Task.FromResult(GameScreen.GlobeSelection);
        }
    }
}