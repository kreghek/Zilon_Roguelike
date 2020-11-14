using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zilon.Bot.Players.NetCore
{
    public sealed class ContainerLogicStateFactory : ILogicStateFactory
    {
        private readonly IServiceProvider _serviceFactory;

        public ContainerLogicStateFactory(IServiceProvider serviceContainer)
        {
            _serviceFactory = serviceContainer;
        }

        public ILogicState CreateLogic<T>() where T : ILogicState
        {
            return _serviceFactory.GetRequiredService<T>();
        }

        public ILogicStateTrigger CreateTrigger<T>() where T : ILogicStateTrigger
        {
            return _serviceFactory.GetRequiredService<T>();
        }
    }
}