using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Zilon.Bot.Sdk;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;
using Zilon.Core.World;

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
        public async Task GetActorTasksTestAsync(string mode)
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new Startup();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            var botSettings = new BotSettings { Mode = mode };

            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var player = serviceProvider.GetRequiredService<IPlayer>();

            var autoPlayEngine = new AutoplayEngine(
                startUp,
                botSettings,
                globeInitializer);

            var globe = await autoPlayEngine.CreateGlobeAsync().ConfigureAwait(false);
            var followedPerson = player.MainPerson;

            PrintPersonBacklog(followedPerson);

            await autoPlayEngine.StartAsync(globe, followedPerson).ConfigureAwait(false);

            PrintResult(serviceProvider);
        }

        private static void PrintPersonBacklog(IPerson humanPerson)
        {
            Console.WriteLine("Build In Traits:");
            var buildinTraits = humanPerson.GetModule<IEvolutionModule>().Perks.Where(x => x.Scheme.IsBuildIn).ToArray();
            foreach (var buildInTrait in buildinTraits)
            {
                Console.WriteLine(buildInTrait.Scheme.Name.En);
            }

            Console.WriteLine("Start Equipments:");
            var equipments = humanPerson.GetModule<IEquipmentModule>().ToArray();
            foreach (var equipment in equipments)
            {
                if (equipment is null)
                {
                    continue;
                }

                Console.WriteLine(equipment.Scheme.Name.En);
            }

            Console.WriteLine("Start Inventory:");
            var inventoryProps = humanPerson.GetModule<IInventoryModule>().CalcActualItems().ToArray();
            foreach (var prop in inventoryProps)
            {
                switch (prop)
                {
                    case Equipment equipment:
                        Console.WriteLine(equipment.Scheme.Name.En);
                        break;

                    case Resource resource:
                        Console.WriteLine($"{resource.Scheme.Name.En} x {resource.Count}");
                        break;

                    default:
                        Console.WriteLine(prop.Scheme.Name.En);
                        break;
                }
            }

            Console.WriteLine("Start attributes:");
            foreach (var attr in humanPerson.GetModule<IAttributesModule>().GetAttributes())
            {
                Console.WriteLine($"{attr.Type}: {attr.Value}");
            }
        }

        private static void PrintResult(ServiceProvider serviceProvider)
        {
            var scoreManager = serviceProvider.GetRequiredService<IScoreManager>();

            Console.WriteLine($"Scores: {scoreManager.BaseScores}");

            var scoreDetails = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);
            Console.WriteLine($"Details:  {scoreDetails}");

            var playerEventLogService = serviceProvider.GetRequiredService<IPlayerEventLogService>();
            var deathReasonService = serviceProvider.GetRequiredService<DeathReasonService>();
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