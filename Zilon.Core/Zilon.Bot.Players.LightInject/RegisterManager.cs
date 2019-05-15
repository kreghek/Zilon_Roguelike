using System;

using LightInject;

using Zilon.Bot.Players.LightInject.DependencyInjection;
using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;


namespace Zilon.Bot.Players.LightInject
{
    [BotRegistration]
    public static class RegisterManager
    {
        [ActorTaskSourceType]
        public static Type ActorTaskSourceType => typeof(BotActorTaskSource);

        [RegisterAuxServices]
        public static void RegisterBot(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterBot();
            serviceRegistry.Register<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory),
                new PerContainerLifetime());
        }

        [ConfigureAuxServices]
        public static void ConfigureAuxServices(IServiceFactory serviceFactory)
        {
            LogicStateTreePatterns.Factory = serviceFactory.GetInstance<ILogicStateFactory>();
        }
    }
}
