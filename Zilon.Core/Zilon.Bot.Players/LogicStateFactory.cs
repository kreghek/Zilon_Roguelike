using LightInject;

namespace Zilon.Bot.Players
{
    public sealed class LogicStateFactory : ILogicStateFactory
    {
        private readonly IServiceFactory _serviceFactory;

        public LogicStateFactory(IServiceFactory serviceContainer)
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
