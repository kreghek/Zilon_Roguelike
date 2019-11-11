using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;

namespace Zilon.GlobeObserver
{
    public sealed class ContainerLogicStateFactory : ILogicStateFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ContainerLogicStateFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogicState CreateLogic<T>() where T : ILogicState
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public ILogicStateTrigger CreateTrigger<T>() where T : ILogicStateTrigger
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
