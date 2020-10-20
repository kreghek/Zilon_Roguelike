using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;

namespace Zilon.Bot.Players.NetCore
{
    [BotRegistration]
    public static class RegisterManager
    {
        [ActorTaskSourceType]
        public static Type ActorTaskSourceType => typeof(HumanBotActorTaskSource);

        [RegisterAuxServices]
        public static void RegisterBot(IServiceCollection serviceRegistry)
        {
            serviceRegistry.RegisterLogicState();
            serviceRegistry.AddScoped<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceRegistry.AddScoped<LogicStateTreePatterns>();
        }

        [ConfigureAuxServices]
        public static void ConfigureAuxServices(IServiceProvider serviceFactory)
        {
        }
    }
}