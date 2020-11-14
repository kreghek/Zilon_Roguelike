using System;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.NetCore
{
    [BotRegistration]
    public static class RegisterManager
    {
        [ActorTaskSourceType]
        public static Type ActorTaskSourceType => typeof(HumanBotActorTaskSource<ISectorTaskSourceContext>);

        [RegisterAuxServices]
        public static void RegisterBot(IServiceCollection serviceRegistry)
        {
            serviceRegistry.RegisterLogicState();
            serviceRegistry.AddScoped<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceRegistry.AddScoped<LogicStateTreePatterns>();
        }

        [ConfigureAuxServices]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter",
            Justification = "Относится к параметру serviceFactory, потому что он используется через рефлексию.")]
        public static void ConfigureAuxServices(IServiceProvider serviceFactory)
        {
        }
    }
}