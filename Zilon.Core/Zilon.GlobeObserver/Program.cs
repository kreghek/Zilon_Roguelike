using System;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Bot.Players;
using Zilon.Bot.Sdk;
using Zilon.Emulation.Common;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            var botSettings = new BotSettings();

            var autoPlayEngine = new AutoplayEngine<HumanBotActorTaskSource>(startUp, botSettings);

            var startPerson = PersonCreateHelper.CreateStartPerson(serviceProvider);

            await autoPlayEngine.StartAsync(startPerson, serviceProvider).ConfigureAwait(false);
        }
    }
}
