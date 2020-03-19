using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

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
            var deathReasonService = _globalServiceProvider.GetRequiredService<DeathReasonService>();
            var lastEvent = playerEventLogService.GetPlayerEvent();

            if (lastEvent != null)
            {
                var deathReason = deathReasonService.GetDeathReasonSummary(
                    lastEvent,
                    Core.Localization.Language.En);

                Console.WriteLine($"Death Reason: {deathReason}");
            }
            else
            {
                // Это может быть в следующих случаях:
                // 1. Ошибка в регистрации или инициализации сервисов, в результате которой система не регистрирует события персонажа.
                // 2. Игра была завершена до наступления любого зарегистрированного события.
                // Эта ситуация может быть, если персонаж умер в результате события, которое не регистрируется.
                // Это считается ошибкой.

                throw new InvalidOperationException("Не удалось вычислить причину смерти персонажа.");
            }
        }
    }
}