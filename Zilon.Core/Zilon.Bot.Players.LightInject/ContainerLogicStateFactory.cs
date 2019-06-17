using LightInject;

namespace Zilon.Bot.Players.LightInject
{
    public sealed class ContainerLogicStateFactory : ILogicStateFactory
    {
        private readonly IServiceFactory _serviceFactory;

        public ContainerLogicStateFactory(IServiceFactory serviceContainer)
        {
            _serviceFactory = serviceContainer;
        }

        public ILogicState CreateLogic<T>() where T : ILogicState
        {
            return _serviceFactory.GetInstance<T>();
        }

        public ILogicStateTrigger CreateTrigger<T>() where T : ILogicStateTrigger
        {
            return _serviceFactory.GetInstance<T>();
        }
    }
}
