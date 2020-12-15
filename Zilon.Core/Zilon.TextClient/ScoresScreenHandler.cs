using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Players;
using Zilon.Core.Scoring;

namespace Zilon.TextClient
{
    /// <summary>
    /// The game screen to show final player scores and achievements.
    /// </summary>
    internal class ScoresScreenHandler : IScreenHandler
    {
        /// <inheritdoc/>
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