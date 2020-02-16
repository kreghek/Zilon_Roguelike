using System;

using LightInject;

using Zilon.Bot.Players.LightInject.DependencyInjection;
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
        public static void RegisterBot(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterLogicState();
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
