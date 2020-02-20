using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;

namespace Zilon.Bot.Players.DevelopmentTests
{
    [TestFixture]
    public class BotActorTaskSourceTests
    {
        [Test]
        [TestCase("joe")]
        [TestCase("duncan")]
        [TestCase("")]
        [TestCase("monster")]
        [Parallelizable(ParallelScope.All)]
        public async Task GetActorTasksTestAsync(string mode)
        {
            var _globalServiceContainer = new ServiceCollection();
            var startUp = new Startup();
            startUp.RegisterServices(_globalServiceContainer);
            var _globalServiceProvider = _globalServiceContainer.BuildServiceProvider();

            var botSettings = new BotSettings { Mode = mode };

            var autoPlayEngine = new AutoplayEngine<HumanBotActorTaskSource>(startUp, botSettings);
            await autoPlayEngine.StartAsync(_globalServiceProvider).ConfigureAwait(false);

            var scoreManager = _globalServiceProvider.GetRequiredService<IScoreManager>();
            Console.WriteLine($"Scores: {scoreManager.BaseScores}");

            var playerEventLogService = _globalServiceProvider.GetRequiredService<IPlayerEventLogService>();
            var lastEvent = playerEventLogService.GetPlayerEvents().LastOrDefault();
            string deathReason = GetDeathReasonString(lastEvent);

            Console.WriteLine($"Death Reason: {deathReason}");
        }

        private static string GetDeathReasonString(IPlayerEvent lastEvent)
        {
            if (lastEvent is null)
            {
                return null;
            }

            switch (lastEvent)
            {
                case PlayerDamagedEvent playerDamagedEvent:
                    return $"{playerDamagedEvent.Damager}";

                case SurvivalEffectDamageEvent survivalEffectDamageEvent:
                    return $"{survivalEffectDamageEvent.Effect.Type}";

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}