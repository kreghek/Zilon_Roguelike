using LightInject;

using Zilon.Bot.Players.LightInject;
using Zilon.Bot.Players.LightInject.DependencyInjection;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

namespace Zilon.Bot.Players.DevelopmentTests
{
    class Startup: InitialzationBase
    {
        public override void ConfigureAux(IServiceFactory serviceFactory)
        {
            LogicStateTreePatterns.Factory = serviceFactory.GetInstance<ILogicStateFactory>();
        }

        protected override void RegisterBot(IServiceRegistry container)
        {
            container.RegisterLogicState();
            container.Register<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory),
                new PerScopeLifetime());

            container.Register<IActorTaskSource, HumanBotActorTaskSource>("bot", new PerScopeLifetime());

        }
    }
}
